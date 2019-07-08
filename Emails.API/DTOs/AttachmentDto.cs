using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.DTOs
{
    public class AttachmentDto
    {
        public long Id { get; set; }
        public string FileName { get;  set; }
        public string DiskPath { get;  set; }
        public string FileExtension { get;  set; }
        public short SizeInKB { get; set; }
        public long EmailId { get; set; }
    }
}
