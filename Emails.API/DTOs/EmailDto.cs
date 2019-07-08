using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.DTOs
{
    public class EmailDto
    {

        public EmailDto()
        {
            Attachments = new List<AttachmentDto>();
        }

        public long Id { get; set; }
        public string Subject { get;  set; }
        public string Sender { get;  set; }
        public string MessageText { get;  set; }
        public string MessageHtml { get;  set; }
        public DateTime ReceivedDate { get;  set; }
        public DateTime DownloadDate { get;  set; }
        public string Uid { get;  set; }
        public string MessageId { get;  set; }
        public long UserId { get;  set; }

        public ICollection<AttachmentDto> Attachments { get; set; }

    }
}
