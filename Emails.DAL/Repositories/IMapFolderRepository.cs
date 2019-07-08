using Emails.DAL.Repositories.Interfaces;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.DAL.Repositories
{
    public class IMapFolderRepository : Repository<IMapFolder>, IIMapFolderRepository
    {
        public IMapFolderRepository(EmailsContext context) : base(context)
        {
        }

        public async Task<bool> ShouldReadFolderAsync(long imapFolderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Result<Maybe<IMapFolder>> imapFolder = await this.FindByKeyAsync(imapFolderId, cancellationToken);

            return imapFolder.Value.Value.Read;
        }

        public async Task<IMapFolder> AddFolderIfNotExistsAsync(long emailBoxId, string imapFolderName, CancellationToken cancellationToken = default(CancellationToken))
        {
            Maybe<IMapFolder> imapFolder = await Context.IMapFolders.SingleOrDefaultAsync(IF => IF.EmailBoxId == emailBoxId &&
            IF.Name.Equals(imapFolderName, StringComparison.OrdinalIgnoreCase), cancellationToken);

            if (imapFolder.HasNoValue)
            {
                Result<IMapFolder> newFolder = IMapFolder.Create(imapFolderName, emailBoxId);

                await this.InsertAsync(newFolder.Value, cancellationToken);
                return newFolder.Value;
            }

            return imapFolder.Value;
        }

        public async Task<bool> ShouldRemoveEmailsAfterReadAsync(long imapFolderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            Result<Maybe<IMapFolder>> imapFolder = await this.FindByKeyAsync(imapFolderId, cancellationToken);

            return imapFolder.Value.Value.DeleteAfterFetch;
        }
    }
}
