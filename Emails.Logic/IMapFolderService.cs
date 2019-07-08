using Emails.Domain.Entities;
using Highway.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.Logic
{
    public class IMapFolderService
    {
        private IRepository _repository;


        public IMapFolderService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IMapFolder> AddFolderIfNotExistsAsync(int emailBoxId, string imapFolderName, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var imapFolder = await _repository.FindAsync(new FolderExists(imapFolderName, emailBoxId));

            //if(imapFolder == null)
            //{
            //    imapFolder = _repository.Context.Add(IMapFolder.Create(imapFolderName, emailBoxId));
            //    await _repository.Context.CommitAsync();
            //}

            return null;
        }
    }
}
