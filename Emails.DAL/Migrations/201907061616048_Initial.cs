namespace Emails.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Emails.EmailAddresses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 254),
                        EmailAddressTypeId = c.Byte(nullable: false),
                        EmailId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Emails.EmailAddressTypes", t => t.EmailAddressTypeId, cascadeDelete: true)
                .Index(t => t.EmailAddressTypeId);
            
            CreateTable(
                "Emails.EmailAddressTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Emails.EmailBoxes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(nullable: false, maxLength: 254),
                        Port = c.Short(nullable: false),
                        UseSsl = c.Boolean(nullable: false),
                        Host = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 20),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Emails.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.Address)
                .Index(t => t.UserId);
            
            CreateTable(
                "Emails.IMapFolders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128),
                        EmailBoxId = c.Long(nullable: false),
                        Read = c.Boolean(nullable: false),
                        DeleteAfterFetch = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Emails.EmailBoxes", t => t.EmailBoxId, cascadeDelete: true)
                .Index(t => t.EmailBoxId);
            
            CreateTable(
                "Emails.Emails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Subject = c.String(maxLength: 512),
                        Sender = c.String(maxLength: 254),
                        MessageText = c.String(),
                        MessageHtml = c.String(),
                        ReceivedDate = c.DateTime(nullable: false),
                        DownloadDate = c.DateTime(nullable: false),
                        Uid = c.String(nullable: false),
                        MessageId = c.String(nullable: false),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Emails.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "Emails.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserSettingId = c.Long(nullable: false),
                        UserLogin = c.String(nullable: false, maxLength: 30),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        EmailAddress = c.String(nullable: false, maxLength: 254),
                        Password = c.String(nullable: false),
                        Street = c.String(nullable: false, maxLength: 256),
                        PostalCode = c.String(nullable: false, maxLength: 6),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserLogin, unique: true);
            
            CreateTable(
                "Emails.UserSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Emails.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Emails.Emails", "UserId", "Emails.Users");
            DropForeignKey("Emails.UserSettings", "UserId", "Emails.Users");
            DropForeignKey("Emails.EmailBoxes", "UserId", "Emails.Users");
            DropForeignKey("Emails.IMapFolders", "EmailBoxId", "Emails.EmailBoxes");
            DropForeignKey("Emails.EmailAddresses", "EmailAddressTypeId", "Emails.EmailAddressTypes");
            DropIndex("Emails.UserSettings", new[] { "UserId" });
            DropIndex("Emails.Users", new[] { "UserLogin" });
            DropIndex("Emails.Emails", new[] { "UserId" });
            DropIndex("Emails.IMapFolders", new[] { "EmailBoxId" });
            DropIndex("Emails.EmailBoxes", new[] { "UserId" });
            DropIndex("Emails.EmailBoxes", new[] { "Address" });
            DropIndex("Emails.EmailAddresses", new[] { "EmailAddressTypeId" });
            DropTable("Emails.UserSettings");
            DropTable("Emails.Users");
            DropTable("Emails.Emails");
            DropTable("Emails.IMapFolders");
            DropTable("Emails.EmailBoxes");
            DropTable("Emails.EmailAddressTypes");
            DropTable("Emails.EmailAddresses");
        }
    }
}
