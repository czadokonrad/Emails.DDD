using Emails.Domain.Entities;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.Reader.MailSenders.MailKit
{
    public interface ISmtpSender
    { 
        Task SendMessageAsync(EmailBox emailBox, MimeMessage mimeMessage, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default);
        Task SendMessageAsync(EmailBox emailBox, MimeMessage mimeMessage, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default, ITransferProgress progress = null);
    }
}
