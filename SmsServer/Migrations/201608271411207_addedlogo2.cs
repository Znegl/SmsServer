namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedlogo2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "Image2MimeType", c => c.String());
            AddColumn("dbo.Races", "IsImage2OnDisk", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "IsImage2OnDisk");
            DropColumn("dbo.Races", "Image2MimeType");
        }
    }
}
