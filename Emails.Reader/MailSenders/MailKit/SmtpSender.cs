using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emails.Domain.Entities;
using MailKit;
using MailKit.Security;
using MimeKit;

namespace Emails.Reader.MailSenders.MailKit
{
    public class SmtpSender : ISmtpSender, IDisposable
    { 
        private IMailTransport _mailTransport;

        public SmtpSender(IMailTransport mailTransport)
        {
            _mailTransport = mailTransport;
        }

        public async Task SendMessageAsync(EmailBox emailBox, MimeMessage mimeMessage, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default)
        {

            cancellationToken.ThrowIfCancellationRequested(); 

            await ConnectAsync(emailBox, cancellationToken);
            await _mailTransport.SendAsync(mimeMessage, cancellationToken);
        }

        public async Task SendMessageAsync(EmailBox emailBox, MimeMessage mimeMessage, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await ConnectAsync(emailBox, cancellationToken);
            await _mailTransport.SendAsync(mimeMessage, cancellationToken, progress);
        }

        public async Task SendMessageAsync(EmailBox emailBox, MimeMessage mimeMessage, IEnumerable<MailboxAddress> recipients, Action<object, MessageSentEventArgs> onMessageSent, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await ConnectAsync(emailBox, cancellationToken);
            await _mailTransport.SendAsync(mimeMessage, cancellationToken);

            _mailTransport.MessageSent += onMessageSent.Invoke;
        }



        private async Task ConnectAsync(EmailBox emailBox, CancellationToken cancellationToken)
        {
            await _mailTransport.ConnectAsync(emailBox.Host, emailBox.Port, SecureSocketOptions.SslOnConnect, cancellationToken);
            await _mailTransport.AuthenticateAsync(emailBox.Address, emailBox.Password, cancellationToken);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(_mailTransport != null)
                {
                    _mailTransport.Dispose();
                    _mailTransport = null;
                }
            }
        }

    }
}
