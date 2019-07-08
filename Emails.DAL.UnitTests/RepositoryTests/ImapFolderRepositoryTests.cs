using Emails.DAL.Repositories;
using Emails.Domain.Entities;
using Emails.Tests.Shared;
using FunctionalExtensionsLibrary.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.DAL.UnitTests.RepositoryTests
{
    [TestClass]
    public class IMapFolderRespositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public async Task ShouldReadFolderAsync_ShouldReturnTrue_When_Folder_ShouldBeRead()
        {

            Result<IMapFolder> imapFolderToCheck = IMapFolder.Create("fsdf", 1);

            List<IMapFolder> inMemoryMockData = new List<IMapFolder>();
            inMemoryMockData.Add(imapFolderToCheck.Value);

            var emailsContext = new Mock<EmailsContext>();

            emailsContext.Object.IMapFolders = MockDbSet(inMemoryMockData);

            emailsContext.Setup(EC => EC.Set<IMapFolder>())
                .Returns(emailsContext.Object.IMapFolders);

            CancellationTokenSource cts = new CancellationTokenSource();

            var sut = new IMapFolderRepository(emailsContext.Object);


            Assert.IsTrue(await sut.ShouldReadFolderAsync(imapFolderToCheck.Value.Id, cts.Token));

        }


        [TestMethod]
        public async Task AddFolderIfNotExistsAsync_ShouldAddFolder_IfNotExists()
        {
            Result<IMapFolder> imapFolderToCheck = IMapFolder.Create("fsdf", 1);

            List<IMapFolder> inMemoryMockData = new List<IMapFolder>();

            var emailsContext = new Mock<EmailsContext>();

            emailsContext.Object.IMapFolders = MockDbSet(inMemoryMockData);

            emailsContext.Setup(EC => EC.Set<IMapFolder>())
               .Returns(emailsContext.Object.IMapFolders);

            CancellationTokenSource cts = new CancellationTokenSource();

            var sut = new IMapFolderRepository(emailsContext.Object);

            IMapFolder addedFolder = await sut.AddFolderIfNotExistsAsync(1, imapFolderToCheck.Value.Name, cts.Token);

            Assert.IsNotNull(addedFolder);

            emailsContext.Verify(EC => EC.SaveChangesAsync(), Times.Once);


        }


        private static DbSet<T> MockDbSet<T>(List<T> inMemoryData) where T : class
        {
            if (inMemoryData == null)
                inMemoryData = new List<T>();

            var mockDbSet = new Mock<DbSet<T>>();
            var inMemoryQueryableData = inMemoryData.AsQueryable();

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(inMemoryQueryableData.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(inMemoryQueryableData.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(inMemoryQueryableData.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(inMemoryQueryableData.GetEnumerator());

            mockDbSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(inMemoryQueryableData.GetEnumerator()));

            mockDbSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(inMemoryQueryableData.Provider));

            mockDbSet.Setup(m => m.AsNoTracking()).Returns(mockDbSet.Object);

            return mockDbSet.Object;
        }
    }
}
