using System;
using System.IO;
using Emails.Reader.Utilities;
using Emails.Reader.Utitlities;
using NUnit.Framework;

namespace EmailsReader.IntegrationTests
{
    [TestFixture]
    public class PathCreatorTests
    {
        private string _testPath;

        [SetUp]
        public void Initialize()
        {
            _testPath = @"F:\Testdsadsadsada";
            if(Directory.Exists(_testPath))
            {
                Directory.Delete(_testPath, true);
            }
        }

        [Test]
        public void When_Invalid_Path_Characters_Are_Passed_Then_Should_Fix_And_Create_Proper()
        {
            string invalidPath = PathCreator.SafeCombine(_testPath, "@#$%^&*()#$!<>?'\\]|", "$324");
            var dirManager = new DirectoryManager();

            string safePath = PathCreator.CreateSafePath(dirManager, invalidPath);

            Assert.IsTrue(Directory.Exists(safePath));
        }

        [Test]
        public void When_Invalid_FileName_Characters_Are_Passed_Then_Should_Fix_And_Create_Proper_File_Path()
        {
            string invalidPath = PathCreator.SafeCombine(_testPath, "!@#$%^&*()*&^%$#@!!$%^&*(/**-*-+,0?:'~`1!<>");
            var dirManager = new DirectoryManager();
            var fileManager = new FileManager();

            string safeDir = PathCreator.CreateSafePath(dirManager, invalidPath);

            string unsafeFileName = "*&^%*^&/\\%^<>42343jhsdgbv.txt";


            PathCreator.CreateFileWithSafeFileName(fileManager, safeDir, unsafeFileName);

            Assert.IsTrue(Directory.GetFiles(safeDir).Length == 1);
        }
    }
}
