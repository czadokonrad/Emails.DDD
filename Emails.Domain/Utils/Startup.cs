using Emails.Domain.Domain_Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emails.Domain.Utils
{
    public static class Startup
    {
        public static void Init()
        {
            DomainEvents.Init();
        }
    }
}
