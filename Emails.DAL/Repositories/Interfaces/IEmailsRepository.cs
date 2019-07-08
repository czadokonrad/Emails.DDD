using Emails.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.DAL.Repositories.Interfaces
{
    public interface IEmailsRepository : IRepository<Email>
    {
    }
}
