using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public interface IDirectoryManager
    {
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
        void Move(string dirFrom, string dirTo);
        string GetSafePath(string path);
        string SafeCombine(string path, params string[] paths);
    }
}
