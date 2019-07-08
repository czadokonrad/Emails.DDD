namespace Emails.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Emails.Attachments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(nullable: false, maxLength: 248),
                        DiskPath = c.String(nullable: false, maxLength: 248),
                        FileExtension = c.String(nullable: false, maxLength: 10),
                        SizeInKB = c.Short(nullable: false),
                        EmailId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.FileName);
            
        }
        
        public override void Down()
        {
            DropIndex("Emails.Attachments", new[] { "FileName" });
            DropTable("Emails.Attachments");
        }
    }
}
