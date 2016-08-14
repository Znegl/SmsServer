namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedShowPostCheckinToRace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "ShowCheckinForPost", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "ShowCheckinForPost");
        }
    }
}
