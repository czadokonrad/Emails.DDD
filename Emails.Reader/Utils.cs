using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Reader
{
    internal class Utils
    { 
        internal enum ProtocolLoggerType
        {
            POP3,
            IMAP,
            SMTP
        }
        internal static string CreateFileForProtocolLogger(ProtocolLoggerType protocolLoggerType)
        {
            return CreatePathIfNotExists(protocolLoggerType);
        }

        private static string CreatePathIfNotExists(ProtocolLoggerType protocolLoggerType)
        {
            string path = $@"D:\ProtocolLogger\{protocolLoggerType.ToString()}\{DateTime.Now.Year}\{DateTime.Now.Month}\{DateTime.Now.Day}";

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch(UnauthorizedAccessException)
                {
                    return string.Empty;
                }
            }
            string filePath = Path.Combine(path, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.log");
            File.Create(filePath);

            return filePath;
        }
    }
}
