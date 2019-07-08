namespace Emails.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Emails.Attachments", "EmailId");
            CreateIndex("Emails.EmailAddresses", "EmailId");
            AddForeignKey("Emails.Attachments", "EmailId", "Emails.Emails", "Id", cascadeDelete: true);
            AddForeignKey("Emails.EmailAddresses", "EmailId", "Emails.Emails", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Emails.EmailAddresses", "EmailId", "Emails.Emails");
            DropForeignKey("Emails.Attachments", "EmailId", "Emails.Emails");
            DropIndex("Emails.EmailAddresses", new[] { "EmailId" });
            DropIndex("Emails.Attachments", new[] { "EmailId" });
        }
    }
}
