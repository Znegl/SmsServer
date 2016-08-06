namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLogoImageToRace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "ImageMimeType", c => c.String());
            AddColumn("dbo.Races", "IsImageOnDisk", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "IsImageOnDisk");
            DropColumn("dbo.Races", "ImageMimeType");
        }
    }
}
