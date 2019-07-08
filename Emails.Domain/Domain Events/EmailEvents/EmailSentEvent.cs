using Emails.Domain.Domain_Events.Base;
using Emails.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Domain_Events.EmailEvents
{
    public class EmailSentEvent : IDomainEvent
    {
        public long EmailId { get; private set; }

        public EmailSentEvent(long emailId)
        {
            EmailId = emailId; 
        }
    }
}
