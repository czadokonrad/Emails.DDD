using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public class DirectoryManager : IDirectoryManager
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public void Move(string dirFrom, string dirTo)
        {
            Directory.Move(dirFrom, dirTo);
        }

        public string GetSafePath(string path)
        {
            string safePath = CharEnumerable
              .InvalidPathCharacters()
              .Aggregate(path, (current, c) => current.Replace(c.ToString(), string.Empty));

            return safePath;
        }

        public string SafeCombine(string path, params string[] paths)
        {
            string safePath = CharEnumerable.InvalidPathCharacters()
                 .Aggregate(path, (current, c) => current.Replace(c.ToString(), string.Empty));


            foreach (string p in paths)
            {
                string safeSubPath = CharEnumerable.InvalidPathCharacters()
                 .Aggregate(p, (current, c) => current.Replace(c.ToString(), string.Empty));
                safePath = Path.Combine(safePath, safeSubPath);
            }

            return safePath;
        }

    }
}
