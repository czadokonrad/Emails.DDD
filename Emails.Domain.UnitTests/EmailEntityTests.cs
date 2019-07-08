using System;
using Emails.Domain.Entities;
using FluentAssertions;
using FunctionalExtensionsLibrary.Exceptions;
using NUnit.Framework;
using static Emails.Domain.Enums.Enums;

namespace Emails.Domain.UnitTests
{
    [TestFixture]
    public class EmailEntityTests
    {
        [Test]
        public void EmailAddresses_Are_Initially_Empty()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            emailResult.Value.EmailAddresses.Count.Should().Be(0);
        }

        [Test]
        public void Attachments_Are_Initially_Empty()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            emailResult.Value.Attachments.Count.Should().Be(0);
        }


        [Test]
        public void Can_Add_Address_To_EmailAddresses()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            Result<EmailAddress> address = EmailAddress.Create("konrad521@vp.pl", EmailAddressTypeEnum.BCCEmail, 1);

            emailResult.Value.AddEmailAddress(address.Value.Value, EmailAddressTypeEnum.BCCEmail);

            emailResult.Value.EmailAddresses.Count.Should().Be(1);
        }

        [Test]
        public void When_Create_Bad_Entity_Should_Return_ResultFail_AndErrorMessage()
        {
            Result<Email> emailResult = Email.Create("", "", "", "", DateTime.Now, "", "", 1);

            Assert.IsTrue(emailResult.IsFailure);
            Assert.IsFalse(emailResult.IsSuccess);

            emailResult.Error.Should().NotBeNullOrWhiteSpace(emailResult.Error);
        }

        [Test]
        public void When_Create_Good_Entity_Should_Return_ResultOK_And_CreatedEntity()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            emailResult.IsFailure.Should().BeFalse();
            emailResult.IsSuccess.Should().BeTrue();

            emailResult.Value.Should().NotBeNull();
        }

        [Test]
        public void When_Added_Good_AttachmentEntity_Should_Add_It_To_Attachments_Collection()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            Result result = emailResult.Value.AddAttachment("32", "321", ".txt", 234);

            result.IsFailure.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();

            emailResult.Value.Attachments.Count.Should().Be(1);
        }

        [Test]
        public void When_Added_Bad_AttachmentEntity_Should_Return_Result_Fail_With_ErrorMessage()
        {
            Result<Email> emailResult = Email.Create("2", "2", "2", "2", DateTime.Now, "2", "2", 1);

            Result result = emailResult.Value.AddAttachment("", "", "", 234);

            result.Error.Should().NotBeNullOrWhiteSpace();
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();

            emailResult.Value.Attachments.Count.Should().Be(0);
        }
    }
}
