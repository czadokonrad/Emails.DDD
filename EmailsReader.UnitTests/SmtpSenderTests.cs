using Emails.Domain.Entities;
using Emails.Reader.MailSenders.MailKit;
using MailKit;
using MimeKit;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailsReader.UnitTests
{
    [TestFixture]
    public class SmtpSenderTests
    {
        private Mock<IMailTransport> _mockTransport;

        [SetUp]
        public void SetUp()
        {
            _mockTransport = new Mock<IMailTransport>();
        }

        [Test]
        public async Task SendMessageAsync_SendsMessage()
        {

            CancellationTokenSource cts = new CancellationTokenSource(10000);
            SmtpSender sut = new SmtpSender(_mockTransport.Object);


            await sut.SendMessageAsync(new EmailBox(), new MimeMessage(), new List<MailboxAddress>(), cts.Token);

            _mockTransport.Verify(MT => MT.SendAsync(It.IsAny<MimeMessage>(), cts.Token, null), Times.Once);
        }

        [Test]
        [Category("Exceptions")]
        public void SendMessageAsync_Throws_OperationCancelledException()
        {
            CancellationTokenSource cts = new CancellationTokenSource(1);
            SmtpSender sut = new SmtpSender(_mockTransport.Object);


            Assert.That(async() => await sut.SendMessageAsync(new EmailBox(), new MimeMessage(), new List<MailboxAddress>(), cts.Token), Throws.TypeOf<OperationCanceledException>());
        }


        [Test]
        public async Task SendMessageAsync_ShouldRaise_MessageSentEvent_WhenMessageSent()
        {
            SmtpSender sut = new SmtpSender(_mockTransport.Object);
            CancellationTokenSource cts = new CancellationTokenSource(10000);

            List<string> raisedEvents = new List<string>();

            Action<object, MessageSentEventArgs> eventHandler = (s, e) =>
            {
                raisedEvents.Add("fired");
            };

            await sut.SendMessageAsync(new EmailBox(), new MimeMessage(), new List<MailboxAddress>(), eventHandler, cts.Token);
            _mockTransport.Raise(MT => MT.MessageSent += null, new MessageSentEventArgs(new MimeMessage(), ""));


            Assert.AreEqual(raisedEvents.Count, 1);

        }
    }
}
