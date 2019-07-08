using Emails.Domain.Aggregates.Base;
using Emails.Domain.Utils;
using Emails.Domain.Value_Objects;
using FunctionalExtensionsLibrary.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emails.Domain.Entities
{
    public class User : AggregateRoot
    {

        private User()
        {
            EmailBoxes = new List<EmailBox>();
        }


        [Index("IX_UserLogin", IsClustered = false, IsUnique = true)]
        [StringLength(30)]
        [Required]
        public string UserLogin { get; private set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; private set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; private set; }
        [EmailAddress]
        [Required]
        [StringLength(254)]
        public string EmailAddress { get; private set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; private set; }
        public Address Address { get; private set; }

        public ICollection<EmailBox> EmailBoxes { get; private set; }

        public static Result<User> Create(string userLogin, string firstName, string lastName, string emailAddress, string password, Address address)
        {
            User user = new User
            {
                UserLogin = userLogin,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                Password = password,
                Address = address
            };

            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)user).Validate();

            if (validationResult.IsSuccess)
            {
                return Result.Ok<User>(user);
            }
            else
                return Result.Fail<User>(validationResult.Error);
        }

        public Result AddEmailBox(string address, string host, string password, short port, bool useSsl)
        {
            Result<EmailBox> emailbox = EmailBox.Create(address, host, password, port, useSsl, this.Id);

            if (emailbox.IsSuccess)
            {
                EmailBoxes.Add(emailbox.Value);
                return Result.Ok();
            }
            else
                return Result.Fail(emailbox.Error);
        }
    }
}
