using Emails.Domain.Entities.Base;
using Emails.Domain.Utils;
using FunctionalExtensionsLibrary.Exceptions;
using System.ComponentModel.DataAnnotations;
using static Emails.Domain.Enums.Enums;

namespace Emails.Domain.Entities
{
    public class EmailAddress : Entity
    {

        private EmailAddress()
        {

        }

        [StringLength(254)]
        [Required]
        public string Value { get; private set; }
        [Required]
        public long EmailId { get; private set; }
        public byte EmailAddressTypeId { get; private set; }


        public static Result<EmailAddress> Create(string value, EmailAddressTypeEnum emailAddressType, long emailId)
        {
            EmailAddress emailAddress = new EmailAddress
            {
                Value = value,
                EmailAddressTypeId = (byte)emailAddressType,
                EmailId = emailId
            };

            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)emailAddress).Validate();

            if (validationResult.IsSuccess)
                return Result.Ok<EmailAddress>(emailAddress);
            else
                return Result.Fail<EmailAddress>(validationResult.Error);
        }


    }
}
