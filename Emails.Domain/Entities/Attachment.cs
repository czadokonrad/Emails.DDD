using Emails.Domain.Entities.Base;
using Emails.Domain.Utils;
using FunctionalExtensionsLibrary.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emails.Domain.Entities
{
    public class Attachment : Entity
    {
        private Attachment()
        {

        }

        [StringLength(248)]
        [Required]
        [Index(IsClustered = false, IsUnique = false)]
        public string FileName { get; private set; }
        [Required]
        [StringLength(248)]
        public string DiskPath { get; private set; }
        [StringLength(10)]
        [Required]
        public string FileExtension { get; private set; }
        [Required]
        public short SizeInKB { get; private set; }
        [Required]
        public long EmailId { get; private set; }
        public static Result<Attachment> Create(string diskPath, string fileName, string fileExtension, short fileSizeInKB, long emailId)
        {
            Attachment attachment = new Attachment
            {
                DiskPath = diskPath,
                FileName = fileName,
                FileExtension = fileExtension,
                SizeInKB = fileSizeInKB,
                EmailId = emailId
            };

            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)attachment).Validate();

            if (validationResult.IsSuccess)
                return Result.Ok<Attachment>(attachment);
            else
                return Result.Fail<Attachment>(validationResult.Error);
        }
    }
}
