namespace Emails.DAL.Migrations
{
    using Emails.Domain.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using static Emails.Domain.Enums.Enums;

    public sealed class Configuration : DbMigrationsConfiguration<Emails.DAL.EmailsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Emails.DAL.EmailsContext context)
        {

        }
    }
}
