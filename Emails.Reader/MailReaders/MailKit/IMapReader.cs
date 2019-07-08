using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Emails.DAL.Repositories.Interfaces;
using Emails.Domain.Entities;
using MailKit.Search;
using MimeKit;
using static Emails.Domain.Enums.Enums;
using MailKit;
using Emails.Reader.Attachments;
using Emails.Reader.Utilities;
using FunctionalExtensionsLibrary.Exceptions;

namespace Emails.Reader.MailReaders.MailKit
{
    public class IMapReader : IIMapMailReader, IDisposable
    {
        private IMailStore _client;
        private IProtocolLogger _protocolLogger;
        private IIMapFolderRepository _imapFolderRepository;
        private IEmailsRepository _emailsRepository;
        private AttachmentDownloader _attachmentDownloader;

        public IMapReader(IMailStore imapClient, IProtocolLogger protocolLogger, IIMapFolderRepository imapFolderRepository, IEmailsRepository emailsRepository)
        {
            _client = imapClient ?? throw new ArgumentNullException(nameof(IMailStore));
            _protocolLogger = protocolLogger ?? throw new ArgumentNullException(nameof(IProtocolLogger));
            _imapFolderRepository = imapFolderRepository ?? throw new ArgumentNullException(nameof(IIMapFolderRepository));
            _emailsRepository = emailsRepository ?? throw new ArgumentNullException(nameof(IEmailsRepository));
            _attachmentDownloader = new AttachmentDownloader(new DirectoryManager(), new FileManager());
        }
        public async Task<IMailStore> ConnectAsync(EmailBox emailBox, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                _client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                cancellationToken.ThrowIfCancellationRequested();

                await _client.ConnectAsync(emailBox.Host, emailBox.Port, emailBox.UseSsl, cancellationToken);
                await _client.AuthenticateAsync(emailBox.Address, emailBox.Password, cancellationToken);

                return _client;

            }
            catch (ProtocolException)
            {
                return await ConnectAsync(emailBox, cancellationToken); //retry
            }
            catch (SocketException)
            {
                return await ConnectAsync(emailBox, cancellationToken); //retry
            }
        }

        public async Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken)
        {
            List<IMailFolder> folders = await GetFoldersAsync(client, cancellationToken);
            List<Email> emailsToInsertIntoDb = new List<Email>();

            foreach (IMailFolder folder in folders)
            {
                IList<UniqueId> messageUidsToFetch = null;
                IMapFolder imapFolder = await _imapFolderRepository.AddFolderIfNotExistsAsync(emailBox.Id, folder.Name, cancellationToken);

                if (await _imapFolderRepository.ShouldReadFolderAsync(imapFolder.Id, cancellationToken))
                {
                    await folder.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
                    messageUidsToFetch = await folder.SearchAsync(SearchQuery.All, cancellationToken);

                    foreach (UniqueId uid in messageUidsToFetch)
                    {
                        try
                        {
                            MimeMessage mimeMessage = await folder.GetMessageAsync(uid, cancellationToken);
                            emailsToInsertIntoDb.Add(CreateEmailFromMimeMessage(mimeMessage, emailBox));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                if (await _imapFolderRepository.ShouldRemoveEmailsAfterReadAsync(imapFolder.Id, cancellationToken))
                {
                    await folder.AddFlagsAsync(messageUidsToFetch, MessageFlags.Deleted, true, cancellationToken);
                }

                await _emailsRepository.InsertRangeAsync(emailsToInsertIntoDb, cancellationToken);
                emailsToInsertIntoDb.Clear();
            }
        }

        public async Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken, IProgress<double> progress)
        {
            try
            {
                int i = 0;
                List<IMailFolder> folders = await GetFoldersAsync(client, cancellationToken);
                List<Email> emailsToInsertIntoDb = new List<Email>();
                foreach (IMailFolder folder in folders)
                {
                    IList<UniqueId> messageUidsToFetch = null;
                    IMapFolder imapFolder = await _imapFolderRepository.AddFolderIfNotExistsAsync(emailBox.Id, folder.Name, cancellationToken);

                    if (await _imapFolderRepository.ShouldReadFolderAsync(imapFolder.Id, cancellationToken))
                    {
                        await folder.OpenAsync(FolderAccess.ReadWrite, cancellationToken);
                        messageUidsToFetch = await folder.SearchAsync(SearchQuery.All, cancellationToken);

                        foreach (UniqueId uid in messageUidsToFetch)
                        {
                            try
                            {
                                progress?.Report(i++);
                                MimeMessage mimeMessage = await folder.GetMessageAsync(uid, cancellationToken);
                                Email email = CreateEmailFromMimeMessage(mimeMessage, emailBox);
                                await DownloadEmailAttachmentsAsync(mimeMessage, emailBox, email);
                                await _emailsRepository.InsertAsync(email, cancellationToken);

                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }
                    }

                    if (await _imapFolderRepository.ShouldRemoveEmailsAfterReadAsync(imapFolder.Id, cancellationToken))
                    {
                        await folder.AddFlagsAsync(messageUidsToFetch, MessageFlags.Deleted, true, cancellationToken);
                    }

                    //await _emailsRepository.InsertRangeAsync(emailsToInsertIntoDb, cancellationToken);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken, IProgress<string> progress)
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken, IProgress<double> progress)
        {
            throw new NotImplementedException();
        }

        public Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken, IProgress<string> progress)
        {
            throw new NotImplementedException();
        }

        private async Task<List<IMailFolder>> GetFoldersAsync(IMailStore client, CancellationToken cancellationToken = default(CancellationToken))
        {
            FolderNamespaceCollection folderNamespaces = client.PersonalNamespaces;

            List<IMailFolder> folders = new List<IMailFolder>();

            foreach (var item in folderNamespaces)
            {
                folders.AddRange(await client.GetFoldersAsync(item, false, cancellationToken));
            }

            return folders;
        }

        private async Task DownloadEmailAttachmentsAsync(MimeMessage mimeMessage, EmailBox emailBox, Email email)
        {
            if (!mimeMessage.Attachments.Any()) return;

            await _attachmentDownloader.SaveAttachmentsAsync($@"C:\Attachments\{emailBox.Address}\{mimeMessage.Date.Year}\{mimeMessage.Date.Month}\{mimeMessage.Date.Day}", mimeMessage, email);
            await _attachmentDownloader.SaveBase64PicturesAsync($@"C:\Attachments\{emailBox.Address}\{mimeMessage.Date.Year}\{mimeMessage.Date.Month}\{mimeMessage.Date.Day}", mimeMessage, email);
        }

        private Email CreateEmailFromMimeMessage(MimeMessage mimeMessage, EmailBox emailBox)
        {
            Result<Email> newEmailResult = Email.Create(mimeMessage.Subject, mimeMessage.From?.Mailboxes?.FirstOrDefault()?.Address,
                mimeMessage.TextBody, mimeMessage.HtmlBody, mimeMessage.Date.DateTime, Guid.NewGuid().ToString(), mimeMessage.MessageId, emailBox.UserId);

            Email newEmail = newEmailResult.Value;


            GetSenderDetails(mimeMessage, newEmail);
            GetReceiverDetails(mimeMessage, newEmail);
            GetCCDetails(mimeMessage, newEmail);
            GetBCCDetails(mimeMessage, newEmail);
            GetResentCCDetails(mimeMessage, newEmail);
            GetResentBCCDetails(mimeMessage, newEmail);
            GetReplyToDetails(mimeMessage, newEmail);
            GetFromDetails(mimeMessage, newEmail);
            GetInReplyToDetails(mimeMessage, newEmail);
            GetResentFromDetails(mimeMessage, newEmail);
            GetResentReplyToDetails(mimeMessage, newEmail);
            GetResentSenderDetails(mimeMessage, newEmail);
            GetResentToDetails(mimeMessage, newEmail);

            return newEmail;
        }

        private void GetSenderDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.Sender != null)
            {
                if (!string.IsNullOrWhiteSpace(mimeMessage.Sender.Address))
                {
                    email.AddEmailAddress(mimeMessage.Sender.Address, EmailAddressTypeEnum.SenderEmail);
                }
                if (!string.IsNullOrWhiteSpace(mimeMessage.Sender.Name))
                {
                    email.AddEmailAddress(mimeMessage.Sender.Name, EmailAddressTypeEnum.SenderName);
                }
            }
        }

        private void GetReceiverDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.To.Mailboxes != null)
            {
                mimeMessage.To.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ReceiverEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ReceiverName);
                    }
                });
            }
        }

        private void GetCCDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.Cc.Mailboxes != null)
            {
                mimeMessage.Cc.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.CCEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.CCName);
                    }
                });
            }
        }



        private void GetBCCDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.Bcc.Mailboxes != null)
            {
                mimeMessage.Bcc.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.BCCEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.BCCName);
                    }
                });
            }
        }

        private void GetResentCCDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentCc.Mailboxes != null)
            {
                mimeMessage.ResentCc.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ResentCCEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ResentCCName);
                    }
                });
            }
        }


        private void GetResentBCCDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentBcc.Mailboxes != null)
            {
                mimeMessage.ResentBcc.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ResentBCCEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ResentBCCName);
                    }
                });
            }
        }

        private void GetReplyToDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ReplyTo.Mailboxes != null)
            {
                mimeMessage.ReplyTo.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ReplyToEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ReplyToName);
                    }
                });
            }
        }


        private void GetFromDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.From.Mailboxes != null)
            {
                mimeMessage.From.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.FromEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.FromName);
                    }
                });
            }
        }

        private void GetInReplyToDetails(MimeMessage mimeMessage, Email email)
        {
            if (!string.IsNullOrWhiteSpace(mimeMessage.InReplyTo))
            {
                //TODO it is unique id of mesage not name or email address
               email.AddEmailAddress(mimeMessage.InReplyTo, EmailAddressTypeEnum.InReplyToEmail);
            }
        }

        private void GetResentFromDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentFrom.Mailboxes != null)
            {
                mimeMessage.ResentFrom.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ResentFromEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ResentFromName);
                    }
                });
            }
        }

        private void GetResentReplyToDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentReplyTo.Mailboxes != null)
            {
                mimeMessage.ResentReplyTo.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ResentReplyToEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ResentReplyToName);
                    }
                });
            }
        }

        private void GetResentSenderDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentSender != null)
            {
                if (!string.IsNullOrWhiteSpace(mimeMessage.ResentSender.Address))
                {
                    email.AddEmailAddress(mimeMessage.ResentSender.Address, EmailAddressTypeEnum.ResentSenderEmail);
                }

                if (!string.IsNullOrWhiteSpace(mimeMessage.ResentSender.Name))
                {
                    email.AddEmailAddress(mimeMessage.ResentSender.Name, EmailAddressTypeEnum.ResentSenderName);
                }
            }
        }


        private void GetResentToDetails(MimeMessage mimeMessage, Email email)
        {
            if (mimeMessage.ResentTo.Mailboxes != null)
            {
                mimeMessage.ResentReplyTo.Mailboxes.ToList().ForEach(T =>
                {
                    if (!string.IsNullOrWhiteSpace(T.Address))
                    {
                        email.AddEmailAddress(T.Address, EmailAddressTypeEnum.ResentToEmail);
                    }
                    if (!string.IsNullOrWhiteSpace(T.Name))
                    {
                        email.AddEmailAddress(T.Name, EmailAddressTypeEnum.ResentToName);
                    }
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
                if (_protocolLogger != null)
                {
                    _protocolLogger.Dispose();
                    _protocolLogger = null;
                }
            }
        }


    }
}
