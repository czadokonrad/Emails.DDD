using Emails.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.DAL.Repositories
{
    public class EmailBoxRepository : Repository<EmailBox>
    {
        public EmailBoxRepository(EmailsContext context) : base(context)
        {
        }
    }
}
