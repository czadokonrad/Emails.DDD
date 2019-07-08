using Emails.Domain.Entities.Base;
using Emails.Domain.Utils;
using FunctionalExtensionsLibrary.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emails.Domain.Entities
{
    public class EmailBox : Entity
    {
        public EmailBox()
        {
            IMapFolders = new List<IMapFolder>();
        }

        [StringLength(254)]
        [Index(IsClustered = false, IsUnique = false)]
        [Required]
        public string Address { get; private set; }
        [Required]
        public short Port { get; private set; }
        [Required]
        public bool UseSsl { get; private set; }
        [StringLength(50)]
        [Required]
        public string Host { get; private set; }
        [DataType(DataType.Password)]
        [StringLength(20)]
        [Required]
        public string Password { get; private set; }
        [Required]
        public long UserId { get; private set; }

        public ICollection<IMapFolder> IMapFolders { get; private set; }

        public static Result<EmailBox> Create(string emailAddress, string host, string password, short port, bool useSsl, long userId)
        {
            EmailBox emailbox = new EmailBox
            {
                Address = emailAddress,
                Host = host,
                Password = password,
                Port = port,
                UseSsl = useSsl,
                UserId = userId
            };


            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)emailbox).Validate();

            if (validationResult.IsSuccess)
            {
                return Result.Ok<EmailBox>(emailbox);
            }
            else
                return Result.Fail<EmailBox>(validationResult.Error);
        }

        public Result AddImapFolder(string name, bool read = true)
        {
            Result<IMapFolder> imapFolder = IMapFolder.Create(name, this.Id, read);

            if (imapFolder.IsSuccess)
            {
                IMapFolders.Add(imapFolder.Value);
                return Result.Ok();
            }
            else
                return Result.Fail(imapFolder.Error);
        }
    }
}
