using Emails.DAL.Repositories;
using Emails.Domain.Entities;
using Emails.Domain.Value_Objects;
using FluentAssertions;
using FunctionalExtensionsLibrary.Exceptions;
using FunctionalExtensionsLIbrary.Nulls;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Emails.DAL.IntegrationTests.Repositories
{
    [TestFixture]
    public class IMapFolderRepositoryTests
    {

        [Test]
        [Isolated]
        public async Task FindByKeyAsync_Returns_Exactly_One_Entity_Without_Errors()
        {
            var emailsContext = new EmailsContext();

            var imapFolderRepository = new IMapFolderRepository(emailsContext);
            var emailBoxFolderRepository = new EmailBoxRepository(emailsContext);
            var usersRepository = new UsersRepository(emailsContext);

            Result<User> user = User.Create("czader", "Konrad", "Czado", "konrad521@vp.pl", "13", new Address("423", "$324"));

            Result<User> addedUser = await usersRepository.InsertAsync(user.Value);

            Result<EmailBox> emailBox = EmailBox.Create("konrad521@vp.pl", "123", "123", 123, true, 1);



            Result<EmailBox> addedEmailBox = await emailBoxFolderRepository.InsertAsync(emailBox.Value);


            Result<IMapFolder> newFolder = IMapFolder.Create("fsdf", addedEmailBox.Value.Id);
            Result<IMapFolder> addedEntity =  await imapFolderRepository.InsertAsync(newFolder.Value);

            Result<Maybe<IMapFolder>> imapFolder = await imapFolderRepository.FindByKeyAsync(addedEntity.Value.Id);

            imapFolder.Should().NotBeNull();
            imapFolder.Value.Value.Name.Should().Be("fsdf");
            imapFolder.Value.Value.Read.Should().Be(true);
            imapFolder.Value.Value.DeleteAfterFetch.Should().Be(false);
        }

    }
}
