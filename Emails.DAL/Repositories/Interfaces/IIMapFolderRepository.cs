using System.Threading;
using System.Threading.Tasks;
using Emails.Domain.Entities;

namespace Emails.DAL.Repositories.Interfaces
{
    public interface IIMapFolderRepository : IRepository<IMapFolder>
    {
        Task<IMapFolder> AddFolderIfNotExistsAsync(long emailBoxId, string imapFolderName, CancellationToken cancellationToken = default);
        Task<bool> ShouldReadFolderAsync(long imapFolderId, CancellationToken cancellationToken = default);
        Task<bool> ShouldRemoveEmailsAfterReadAsync(long imapFolderId, CancellationToken cancellationToken = default);
    }
}