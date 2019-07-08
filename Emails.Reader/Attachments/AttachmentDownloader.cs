using Emails.Domain.Entities;
using Emails.Reader.Utilities;
using Emails.Reader.Utitlities;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.Reader.Attachments
{
    public class AttachmentDownloader
    {
        private readonly IDirectoryManager _directoryManager;
        private readonly IFileManager _fileManager;

        public AttachmentDownloader(IDirectoryManager directoryManager, IFileManager fileManager)
        {
            _directoryManager = directoryManager;
            _fileManager = fileManager;
        }

        public async Task SaveAttachmentsAsync(string path, MimeMessage mimeMessage, Email emailToPersistInDb, CancellationToken cancellationToken = default(CancellationToken))
        {
            string safePath = _directoryManager.GetSafePath(path);

            if(!_directoryManager.Exists(safePath))
            {
                _directoryManager.CreateDirectory(safePath);
            }

            if (!mimeMessage.Attachments.Any()) return;

            foreach(MimeEntity attachment in mimeMessage.Attachments)
            {
                if (!attachment.IsAttachment)
                    continue;

                string fileName = string.Empty;


                if (attachment.ContentDisposition != null && !string.IsNullOrWhiteSpace(attachment.ContentDisposition.FileName))
                {
                    fileName = attachment.ContentDisposition.FileName;
                }
                else if(attachment.ContentType != null && !string.IsNullOrWhiteSpace(attachment.ContentType.Name))
                {
                    fileName = attachment.ContentType.Name;
                }
                else
                {
                    fileName = "noname" + new string(CharEnumerable.Range(new Random().Next(0, 30), new Random().Next(0, 30)).ToArray());
                }


                using (MemoryStream ms = new MemoryStream())
                { 
                    if (attachment is MessagePart)
                    {
                        MessagePart mPart = attachment as MessagePart;
                        await mPart.Message.WriteToAsync(ms, cancellationToken);
                    }
                    else if(attachment is MimePart)
                    {
                        MimePart mPart = attachment as MimePart;
                        await mPart.Content.DecodeToAsync(ms, cancellationToken);
                    }

                    string fullSafePath = PathCreator.SafeCombine(safePath, fileName);

                    PathCreator.WriteAllBytesToSafeFileName(_fileManager, safePath, fileName, ms.ToArray());
                    emailToPersistInDb.AddAttachment(fileName, safePath, Path.GetExtension(fullSafePath), new FileInfo(fullSafePath).GetFileSizeInKB());
                }
    
               
            }
        }

        public async Task SaveBase64PicturesAsync(string path, MimeMessage mimeMessage, Email emailToPersistInDb, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!mimeMessage.BodyParts.Any()) return;

            string safePath = _directoryManager.GetSafePath(path);

            safePath = PathCreator.SafeCombine(safePath, "base64");

            if (!_directoryManager.Exists(safePath))
            {
                _directoryManager.CreateDirectory(safePath);
            }

            IEnumerable<MimeEntity> bodyPartAttachments = mimeMessage.BodyParts;

            string pictureBase64 = string.Empty;
            string fileName = string.Empty;
            foreach(var bodyPart in bodyPartAttachments)
            {
                if(bodyPart is MimePart)
                {
                    MimePart attachment = bodyPart as MimePart;

                    if(attachment.ContentId != null &&
                        attachment.Content != null &&
                        !string.IsNullOrWhiteSpace(mimeMessage.HtmlBody) &&
                        mimeMessage.HtmlBody.IndexOf("cid:" + attachment.ContentId) > -1)
                    {
                        using(MemoryStream ms = new MemoryStream())
                        {
                            await attachment.Content.DecodeToAsync(ms);
                            pictureBase64 = Convert.ToBase64String(ms.ToArray());
                        }

                        if (attachment.ContentDisposition != null && !string.IsNullOrWhiteSpace(attachment.ContentDisposition.FileName))
                        {
                            fileName = attachment.ContentDisposition.FileName;
                        }
                        else if (attachment.ContentType != null && !string.IsNullOrWhiteSpace(attachment.ContentType.Name))
                        {
                            fileName = attachment.ContentType.Name;
                        }
                        else
                        {
                            fileName = "noname" + new string(CharEnumerable.Range(new Random().Next(0, 30), new Random().Next(0, 30)).ToArray());
                        }

                        string fullSafePath = PathCreator.SafeCombine(safePath, fileName);

                        _fileManager.WriteAllBytes(PathCreator.SafeCombine(safePath, fileName), Convert.FromBase64String(pictureBase64));
                        emailToPersistInDb.AddAttachment(fileName, safePath, Path.GetExtension(fullSafePath), new FileInfo(fullSafePath).GetFileSizeInKB());
                    }
                }
            }

        }
    }
}
