using Emails.Reader.Utilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailsReader.UnitTests
{
    [TestFixture]
    public class PathCreatorTests
    {
        [Test]
        public void CreateSafePath_Should_Create_And_Return_Valid_Directory()
        {

            string invalidPath = "C:\\folder\\tadasdsa\bdasd";
            Mock<IDirectoryManager> _mockDirManager = new Mock<IDirectoryManager>();
        }
    }
}
