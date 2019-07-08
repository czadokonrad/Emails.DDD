using Emails.API.DTOs.UpdateDTOs;
using Emails.API.Utils;
using Emails.API.Utils.ResourceParameters;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.EFCoreRepos.Interfaces
{
    public interface IEmailsRepository : IRepository<Email>
    {
        Task<Result> DeleteAttachmentAsync(long emailId, long attachmentId);
        Task<Result> UpdateAttachmentAsync(long emailId, long attachmentId, AttachmentForUpdateDto bodyToUpdate);
        Task<Result<Maybe<PagedList<Email>>>> GetPageAsync(EmailsResourceParameters emailsResourceParameters);
    }
}
