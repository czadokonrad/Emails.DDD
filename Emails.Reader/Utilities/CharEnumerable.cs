using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader.Utilities
{
    public class CharEnumerable
    {
        /// <summary>
        /// Gets range of char characters
        /// </summary>
        /// <param name="min">Indicates starting point of ASCII character</param>
        /// <param name="count">How many to fetch</param>
        /// <returns></returns>
        public static IEnumerable<char> Range(int min, int count)
        {
            char[] chars = new char[count + 1];
            int iterator = 0;
            for (int i = min; i <= count; i++)
            {
                chars[iterator++] = (char)i;
            }

            return chars;
        }

        public static IEnumerable<char> InvalidPathCharacters()
        {
            return new char[] { '*', '?', '"', '<', '>', '|' , '/' };
        }

        public static IEnumerable<char> InvalidFileNameCharacters()
        {
            return new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        }
    }
}
