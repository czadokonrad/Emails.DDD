namespace Emails.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Emails.EmailAddresses", "EmailAddressTypeId", "Emails.EmailAddressTypes");
            DropIndex("Emails.EmailAddresses", new[] { "EmailAddressTypeId" });
            DropTable("Emails.EmailAddressTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "Emails.EmailAddressTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("Emails.EmailAddresses", "EmailAddressTypeId");
            AddForeignKey("Emails.EmailAddresses", "EmailAddressTypeId", "Emails.EmailAddressTypes", "Id", cascadeDelete: true);
        }
    }
}
