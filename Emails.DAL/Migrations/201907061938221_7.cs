namespace Emails.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Emails.UserSettings", "UserId", "Emails.Users");
            DropIndex("Emails.UserSettings", new[] { "UserId" });
            DropColumn("Emails.Users", "UserSettingId");
            DropTable("Emails.UserSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "Emails.UserSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("Emails.Users", "UserSettingId", c => c.Long(nullable: false));
            CreateIndex("Emails.UserSettings", "UserId");
            AddForeignKey("Emails.UserSettings", "UserId", "Emails.Users", "Id");
        }
    }
}
