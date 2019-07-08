using Emails.API.DTOs.UpdateDTOs;
using Emails.API.EFCoreContext;
using Emails.API.EFCoreRepos.Interfaces;
using Emails.API.Utils;
using Emails.API.Utils.ResourceParameters;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Emails.API.EFCoreRepos
{
    public class EmailsRepository : Repository<Email>, IEmailsRepository
    {
        public EmailsRepository(EmailsContext context) : base(context)
        {
        }

        public async Task<Result> DeleteAttachmentAsync(long emailId, long attachmentId)
        {
            try
            {
                Attachment toDelete = await Context.Attachments.SingleOrDefaultAsync(A => A.EmailId == emailId && A.Id == attachmentId);

                if (toDelete == null)
                    return Result.Fail($"{nameof(Attachment)} does not exists in database for " +
                        $"{nameof(Email)} = {emailId} and {nameof(Attachment)} : {nameof(Attachment.Id)} = {attachmentId}");

                Context.Attachments.Remove(toDelete);
                await Context.SaveChangesAsync();

                return Result.Ok();
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result<Maybe<PagedList<Email>>>> GetPageAsync(EmailsResourceParameters emailsResourceParameters)
        {
            try
            {
                return Result.Ok<Maybe<PagedList<Email>>>(
                    await PagedList<Email>.CreateAsync(Context.Emails, emailsResourceParameters.PageNumber, emailsResourceParameters.PageSize));

             
            }
            catch(Exception ex)
            {
                return Result.Fail<Maybe<PagedList<Email>>>(ex.Message, ex.StackTrace);
            }
        }

        public async Task<Result> UpdateAttachmentAsync(long emailId, long attachmentId, AttachmentForUpdateDto bodyToUpdate)
        {
            try
            {
                Attachment toUpdate = await Context.Attachments.SingleOrDefaultAsync(A => A.EmailId == emailId && A.Id == attachmentId);

                List<PropertyInfo> dtoProps = bodyToUpdate.GetType().GetProperties().ToList();

                foreach(PropertyInfo prop in dtoProps)
                {
                    object value = prop.GetValue(bodyToUpdate);

                    PropertyInfo propToUpdate = toUpdate.GetType().GetProperty(prop.Name);

                    if (propToUpdate == null)
                        throw new InvalidOperationException($"{prop.Name} does not match any property name in {nameof(Attachment)} entity");

                    propToUpdate.SetValue(toUpdate, value);
                }

                await Context.SaveChangesAsync();

                return Result.Ok();

            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message, ex.StackTrace);
            }
        }
    }
}
