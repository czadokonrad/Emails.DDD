using Emails.Domain.Domain_Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Domain_Events.EmailEvents
{
    public class EmailSentEventHandler : IHandler<EmailSentEvent>
    {
        public void Handle(EmailSentEvent domainEvent)
        {
            throw new NotImplementedException();
        }
    }
}
