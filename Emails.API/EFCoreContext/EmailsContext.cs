using Emails.Domain.Entities;
using Emails.Domain.Value_Objects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emails.API.EFCoreContext
{
    public class EmailsContext : DbContext
    {

        public EmailsContext(DbContextOptions<EmailsContext> options) : base(options)
        {

        }

        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<EmailBox> EmailBoxes { get; set; }
        public DbSet<IMapFolder> IMapFolders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attachment> Attachments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Emails");

            modelBuilder.Entity<User>(u =>
            u.OwnsOne(typeof(Address), "Address"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
