using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.UnitTests
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void Entities_ShouldNotBeEqual_When_Id_IsDifferent()
        {
            Result<Email> emailResult = Email.Create("a", "sender", "a", "a", DateTime.MaxValue, "a", "a", 1);

            emailResult.Value.GetType().BaseType.BaseType.GetProperty("Id").SetValue(emailResult.Value, 1, null);

            Result<Email> email2Result = Email.Create("a", "sender", "a", "a", DateTime.MaxValue, "a", "a", 1);
            email2Result.Value.GetType().BaseType.BaseType.GetProperty("Id").SetValue(email2Result.Value, 2, null);

            Assert.IsTrue(emailResult.IsSuccess);
            Assert.IsTrue(email2Result.IsSuccess);

            Assert.IsFalse(emailResult.Value.Equals(email2Result.Value));
        }


        [Test]
        public void Entities_ShouldBeEqual_When_Id_IsTheSame()
        {
            Result<Email> emailResult = Email.Create("a", "sender", "a", "a", DateTime.MaxValue, "a", "a", 1);

            emailResult.Value.GetType().BaseType.BaseType.GetProperty("Id").SetValue(emailResult.Value, 2, null);

            Result<Email> email2Result = Email.Create("a", "sender", "a", "a", DateTime.MaxValue, "a", "a", 1);
            email2Result.Value.GetType().BaseType.BaseType.GetProperty("Id").SetValue(email2Result.Value, 2, null);

            Assert.IsTrue(emailResult.IsSuccess);
            Assert.IsTrue(email2Result.IsSuccess);

            Assert.IsTrue(emailResult.Value.Equals(email2Result.Value));
        }
    }
}
