using Emails.DAL.Repositories.Interfaces;
using Emails.Domain.Entities;
using Emails.Reader.MailReaders.MailKit;
using FunctionalExtensionsLibrary.Exceptions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



[TestFixture]
public class MailkitIMailReaderTests
{
    private Mock<IMailStore> _mockMailStore;
    private Mock<IProtocolLogger> _mockProtocolLogger;
    private Mock<IIMapFolderRepository> _mockImapRepository;
    private Mock<IEmailsRepository> _mockEmailsRepository;

    [SetUp]
    public void SetUp()
    {
        _mockMailStore = new Mock<IMailStore>();
        _mockProtocolLogger = new Mock<IProtocolLogger>();
        _mockImapRepository = new Mock<IIMapFolderRepository>();
        _mockEmailsRepository = new Mock<IEmailsRepository>();
    }

    [Test]
    [Category("Exceptions")]
    public void When_Null_Passed_InConstructor_Should_Throw_ArgumentNullException()
    {
        Assert.That(() => new IMapReader(null, null, null, null), Throws.ArgumentNullException);
    }

    [Test]
    [Category("Exceptions")]
    public void When_All_Dependecies_Injected_Via_Constructor_Do_Not_Throwing_Exceptions()
    {
        Assert.DoesNotThrow(() => new IMapReader(_mockMailStore.Object,
            _mockProtocolLogger.Object, _mockImapRepository.Object, _mockEmailsRepository.Object));
    }



    [Test]
    public async Task Connect_Async_Triggers_Connect_And_Authenticate()
    {
        IMapReader sut = new IMapReader(_mockMailStore.Object,
          _mockProtocolLogger.Object, _mockImapRepository.Object, _mockEmailsRepository.Object);

        CancellationTokenSource cts = new CancellationTokenSource(int.MaxValue);

        await sut.ConnectAsync(new EmailBox(), cts.Token);

        _mockMailStore.Verify(MS => MS.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), cts.Token), Times.Once);
        _mockMailStore.Verify(MS => MS.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), cts.Token), Times.Once);
    }

    [Test]
    public async Task Reads_All_Messages_From_Required_Folders([Values(10, 100_000)] int emailsCount)
    {
        Result<IMapFolder> imapFolder = IMapFolder.Create("test", 1);

        List<UniqueId> uids = new List<UniqueId>();

        for (int i = 0; i < emailsCount; i++)
        {
            uids.Add(UniqueId.MinValue); 
        }

        CancellationTokenSource cts = new CancellationTokenSource(1000000000);
        FolderNamespaceCollection folderNamespaces = new FolderNamespaceCollection();
        folderNamespaces.Add(new FolderNamespace(' ', ""));


        List<IMailFolder> mockMailFolders = new List<IMailFolder>();
        Mock<IMailFolder> mockFolder = new Mock<IMailFolder>();

        mockMailFolders.Add(mockFolder.Object);

        _mockMailStore.Setup(MS => MS.PersonalNamespaces).Returns(folderNamespaces);
        _mockMailStore.Setup(MS => MS.GetFoldersAsync(folderNamespaces[0], false, cts.Token))
            .ReturnsAsync(mockMailFolders);

        mockFolder.Setup(MF => MF.SearchAsync(It.IsAny<SearchQuery>(), cts.Token))
            .ReturnsAsync(uids);

        mockFolder.Setup(MF => MF.GetMessageAsync(It.IsAny<UniqueId>(), cts.Token, null))
            .ReturnsAsync(new Mock<MimeMessage>() { DefaultValue = DefaultValue.Mock }.Object).Verifiable();


        _mockImapRepository.Setup(IFR => IFR.ShouldReadFolderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockImapRepository.Setup(IFR => IFR.AddFolderIfNotExistsAsync(It.IsAny<int>(), mockFolder.Object.Name, cts.Token))
            .ReturnsAsync(imapFolder.Value);

        IMapReader sut = new IMapReader(_mockMailStore.Object,
            _mockProtocolLogger.Object, _mockImapRepository.Object, _mockEmailsRepository.Object);

        IMailStore mailStore = await sut.ConnectAsync(new EmailBox(), cts.Token);

        await sut.ReadAllAsync(mailStore, new EmailBox(), cts.Token);

        mockFolder.Verify(MF => MF.GetMessageAsync(It.IsAny<UniqueId>(), cts.Token, null), Times.Exactly(emailsCount));
    }
}