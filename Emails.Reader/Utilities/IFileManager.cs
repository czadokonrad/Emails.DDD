using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public interface IFileManager
    {
        void Create(string filePath);
        void Copy(string filePathFrom, string filePathTo);

        void WriteAllBytes(string fullSafePath, byte[] fileContent);
    }
}
