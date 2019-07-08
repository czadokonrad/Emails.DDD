using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public class FileManager : IFileManager
    {

        public void Copy(string filePathFrom, string filePathTo)
        {
            File.Copy(filePathFrom, filePathTo);
        }

        public void Create(string filePath)
        {
            File.Create(filePath);
        }

        public void WriteAllBytes(string fullSafePath, byte[] fileContent)
        {
            if(fullSafePath.Length > 248)
            {
                fullSafePath = fullSafePath.Substring(0, 247);
            } 

            File.WriteAllBytes(fullSafePath, fileContent);
        }
    }
}
