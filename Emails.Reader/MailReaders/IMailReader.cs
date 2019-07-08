using Emails.Domain.Entities;
using MailKit;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.Reader.MailReaders
{
    public interface IIMapMailReader
    {
        Task<IMailStore> ConnectAsync(EmailBox emailBox, CancellationToken cancellationToken); 
        Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken);
        Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken, IProgress<double> progress);
        Task ReadAllAsync(IMailStore client, EmailBox emailBox, CancellationToken cancellationToken, IProgress<string> progress);
        Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken);
        Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken, IProgress<double> progress);
        Task ReadAsync(IMailStore client, EmailBox emailBox, SearchQuery searchQuery, CancellationToken cancellationToken, IProgress<string> progress);
    }
}
