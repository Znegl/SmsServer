namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedShowNextPost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "ShowNextPost", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "ShowNextPost");
        }
    }
}
