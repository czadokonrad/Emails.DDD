using Emails.Domain.Entities;
using Emails.Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Emails.Domain.Enums.Enums;

namespace Emails.DAL
{
    public class EmailsContext : DbContext
    {
        public EmailsContext() : base("Emails2016")
        {

        }

        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<EmailBox> EmailBoxes { get; set; }
        public DbSet<IMapFolder> IMapFolders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Emails");


            base.OnModelCreating(modelBuilder);
        }

    }
}
