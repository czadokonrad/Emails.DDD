using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.DTOs.CreationDTOs
{
    public class AttachmentForCreationDto
    {

        [StringLength(248)]
        [Required]
        public string FileName { get; set; }
        [Required]
        [StringLength(248)]
        public string DiskPath { get; set; }
        [StringLength(10)]
        [Required]
        public string FileExtension { get; set; }
        [Required]
        public short SizeInKB { get; set; }
        [Required]
        public long EmailId { get; set; }
    }
}
