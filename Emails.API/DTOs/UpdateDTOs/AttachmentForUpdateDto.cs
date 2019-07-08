using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.DTOs.UpdateDTOs
{
    public class AttachmentForUpdateDto
    {

        [StringLength(248)]
        [Required]
        public string FileName { get; set; }
        [Required]
        [StringLength(248)]
        public string DiskPath { get; set; }
    }
}
