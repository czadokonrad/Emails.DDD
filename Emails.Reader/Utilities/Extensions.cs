using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public static class FileInfoExtensions
    {
        public static short GetFileSizeInKB(this FileInfo fileInfo)
        {
            return (short)(fileInfo.Length / 1024);
        }
    }
}
