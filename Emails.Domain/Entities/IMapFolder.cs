using Emails.Domain.Entities.Base;
using Emails.Domain.Utils;
using FunctionalExtensionsLibrary.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Emails.Domain.Entities
{
    public class IMapFolder : Entity
    {
        private IMapFolder()
        {

        }


        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        [Required]
        public long EmailBoxId { get; set; }
        public bool Read { get; set; }
        public bool DeleteAfterFetch { get; set; }


        public static Result<IMapFolder> Create(string name, long emailBoxId, bool read = true)
        {
            IMapFolder imapFolder = new IMapFolder
            {
                Name = name,
                EmailBoxId = emailBoxId,
                Read = read
            };

            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)imapFolder).Validate();

            if (validationResult.IsSuccess)
            {
                return Result.Ok<IMapFolder>(imapFolder);
            }
            else
                return Result.Fail<IMapFolder>(validationResult.Error);
        }
    }
}
