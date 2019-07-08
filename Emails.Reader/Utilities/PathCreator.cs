
using Emails.Reader.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utitlities
{
    public static class PathCreator
    {
        public static string CreateSafePath(IDirectoryManager directoryManager, string path)
        {

            string safePath = CharEnumerable
                .InvalidPathCharacters()
                .Aggregate(path, (current, c) => current.Replace(c.ToString(), string.Empty));

            DirectoryInfo dirInfo = directoryManager.
                CreateDirectory(safePath);

            return dirInfo.FullName;
        }

        public static void CreateFileWithSafeFileName(IFileManager fileManager, string safePath, string fileName)
        {
            
            string safeFileName = CharEnumerable.
                InvalidFileNameCharacters()
                .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));

            string fullSafePath = Path.Combine(safePath, safeFileName);

            fileManager.Create(fullSafePath);
        }


        public static void WriteAllBytesToSafeFileName(IFileManager fileManager, string safePath, string fileName, byte[] fileContent)
        {

            string safeFileName = CharEnumerable.
                InvalidFileNameCharacters()
                .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));


            string fullSafePath = Path.Combine(safePath, safeFileName);

            if (fullSafePath.Length > 248)
            {
                fullSafePath = fullSafePath.Substring(0, 247);
            }

            fileManager.WriteAllBytes(fullSafePath, fileContent);
        }


        public static string SafeCombine(string path1, params string[] paths)
        {
            string safePath = CharEnumerable.InvalidPathCharacters()
                 .Aggregate(path1, (current, c) => current.Replace(c.ToString(), string.Empty));


            foreach(string path in paths)
            {
                string safeSubPath = CharEnumerable.InvalidPathCharacters()
                 .Aggregate(path, (current, c) => current.Replace(c.ToString(), string.Empty));
                safePath = Path.Combine(safePath, safeSubPath);
            }

            return safePath;
        }


    }
}
