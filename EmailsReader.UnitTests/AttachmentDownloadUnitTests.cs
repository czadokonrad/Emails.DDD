using Emails.Domain.Entities;
using Emails.Reader.Attachments;
using Emails.Reader.Utilities;
using Microsoft.QualityTools.Testing.Fakes;
using MimeKit;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailsReader.UnitTests
{
    [TestFixture]
    public class AttachmentDownloadUnitTests
    {
        private Mock<IDirectoryManager> _mockDirManager;
        private Mock<IFileManager> _mockFileManager;

        [SetUp]
        public void Initialize()
        {
            _mockDirManager = new Mock<IDirectoryManager>();
            _mockFileManager = new Mock<IFileManager>();

        }

        [Test]
        public async Task SaveAttachments_Should_CreateDirectory_When_NotExists()
        {

            using (ShimsContext.Create())
            {
                MimeKit.Fakes.ShimMimeMessage fakeMimeMessage = new MimeKit.Fakes.ShimMimeMessage();
                List<MimeEntity> fakeAttachments = new List<MimeEntity>();

                MimeKit.Fakes.ShimContentDisposition fakeContentDisposition = new MimeKit.Fakes.ShimContentDisposition();
                fakeContentDisposition.FileNameGet = () => "test.txt";

                MimeKit.Fakes.ShimMimeEntity fakeAttachment = new MimeKit.Fakes.ShimMimeEntity(new MessagePart());
                fakeAttachment.ContentDispositionGet = () => fakeContentDisposition;

                fakeAttachments.Add(fakeAttachment);

                fakeMimeMessage.AttachmentsGet = () => fakeAttachments;

                _mockDirManager.Setup(x => x.Exists(It.IsAny<string>()))
                     .Returns(false);

                AttachmentDownloader attachmentDownloader = new AttachmentDownloader(_mockDirManager.Object, _mockFileManager.Object);

                await attachmentDownloader.SaveAttachmentsAsync(It.IsAny<string>(), fakeMimeMessage,
                    Email.Create("543", "konrad521@vp.pl","321", "312", DateTime.MinValue, "312","321",1).Value);

                _mockDirManager.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);

            }

        }

        [Test]
        public async Task SaveAttachments_Should_Call_Create_WhenAttachmentPassed()
        {
            using (ShimsContext.Create())
            {
                MimeKit.Fakes.ShimMimeMessage fakeMimeMessage = new MimeKit.Fakes.ShimMimeMessage();
                List<MimeEntity> fakeAttachments = new List<MimeEntity>();

                MimeKit.Fakes.ShimContentDisposition fakeContentDisposition = new MimeKit.Fakes.ShimContentDisposition();
                fakeContentDisposition.FileNameGet = () => "test.txt";

                MimeKit.Fakes.ShimMimeEntity fakeAttachment = new MimeKit.Fakes.ShimMimeEntity(new MessagePart());
                fakeAttachment.ContentDispositionGet = () => fakeContentDisposition;
                fakeAttachment.IsAttachmentGet = () => true;
                fakeAttachments.Add(fakeAttachment);

                fakeMimeMessage.AttachmentsGet = () => fakeAttachments;

                _mockDirManager.Setup(x => x.Exists(It.IsAny<string>()))
                     .Returns(false);

                AttachmentDownloader attachmentDownloader = new AttachmentDownloader(_mockDirManager.Object, _mockFileManager.Object);

                await attachmentDownloader.SaveAttachmentsAsync(It.IsAny<string>(), fakeMimeMessage, Email.Create("32", "konrad521@vp.pl", "432", "4234", 
                    DateTime.MinValue, "534", "534", 1).Value);

                _mockDirManager.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);

            }
        }
    }
}
