using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.DTOs.UpdateDTOs
{
    public class EmailForUpdateDto
    {

        [Required]
        [StringLength(512)]
        public string Subject { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Sender { get; set; }
        public string MessageText { get; set; }
        public string MessageHtml { get; set; }
        [Required]
        public DateTime ReceivedDate { get; set; } = DateTime.Now;
        [Required]
        public string Uid { get; set; }
        [Required]
        public string MessageId { get; set; }
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "UserId cannot be 0 or negative")]
        public long UserId { get; set; }

    }
}
