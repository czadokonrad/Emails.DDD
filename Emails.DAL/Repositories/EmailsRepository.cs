using Emails.DAL.Repositories.Interfaces;
using Emails.Domain.Entities;
using FunctionalExtensionsLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.DAL.Repositories
{
    public class EmailsRepository : Repository<Email>, IEmailsRepository
    {
        public EmailsRepository(EmailsContext context) : base(context)
        {
        }

    
    }
}
