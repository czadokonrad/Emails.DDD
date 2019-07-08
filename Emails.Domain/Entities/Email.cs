using Emails.Domain.Aggregates.Base;
using Emails.Domain.Utils;
using FunctionalExtensionsLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Emails.Domain.Enums.Enums;

namespace Emails.Domain.Entities
{
    public class Email : AggregateRoot
    {

        protected Email()
        {
            EmailAddresses = new List<EmailAddress>();
            Attachments = new List<Attachment>();
        }

        [StringLength(512)]
        public string Subject { get; private set; }
        [StringLength(254)]
        [EmailAddress]
        public string Sender { get; private set; }
        public string MessageText { get; private set; }
        public string MessageHtml { get; private set; }
        [Required]
        public DateTime ReceivedDate { get; private set; }
        [Required]
        public DateTime DownloadDate { get; private set; } = DateTime.Now;
        [Required]
        public string Uid { get; private set; }
        [Required]
        public string MessageId { get; private set; }
        [ForeignKey(nameof(User))]
        public long UserId { get; private set; }
        public User User { get; private set; }
        public ICollection<EmailAddress> EmailAddresses { get; private set; }
        public ICollection<Attachment> Attachments { get; private set; }


        [NotMapped]
        public bool IsHtml => !string.IsNullOrWhiteSpace(MessageHtml);

        [NotMapped]
        public string MessageBody
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MessageHtml))
                    return MessageHtml;
                else
                    return MessageText;
            }
        }

        public static Result<Email> Create(string subject, string sender, string messageText, string messageHtml, DateTime receivedDate, string uid, string messageId, long userId)
        {
            Email email = new Email
            {
                Subject = subject,
                Sender = sender,
                MessageText = messageText,
                MessageHtml = messageHtml,
                ReceivedDate = receivedDate,
                DownloadDate = DateTime.Now,
                Uid = uid ?? Guid.NewGuid().ToString(),
                MessageId = messageId ?? Guid.NewGuid().ToString(),
                UserId = userId
            };

            Result<IDomainValidatableObject> validationResult = ((IDomainValidatableObject)email).Validate();

            if (validationResult.IsSuccess)
                return Result.Ok<Email>(email);
            else
                return Result.Fail<Email>(validationResult.Error);
        }

        public Result AddEmailAddress(string value, EmailAddressTypeEnum emailAddressType)
        {
            Result<EmailAddress> emailAddress = EmailAddress.Create(value, emailAddressType, this.Id);  

            if (emailAddress.IsSuccess)
            {
                EmailAddresses.Add(emailAddress.Value);
                return Result.Ok();
            }
            else
            {
                return Result.Fail(emailAddress.Error);
            }
        }

        public Result AddAttachment(string fileName, string diskPath, string fileExtension, short sizeInKb)
        {
            Result<Attachment> attachment = Attachment.Create(diskPath, fileName, fileExtension, sizeInKb, this.Id);

            if (attachment.IsSuccess)
            {
                Attachments.Add(attachment.Value);
                return Result.Ok();
            }
            else
                return Result.Fail(attachment.Error);
        }

    }
}
