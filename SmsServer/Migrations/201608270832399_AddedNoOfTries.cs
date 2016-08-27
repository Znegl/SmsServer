namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNoOfTries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "NoOfTriesPerPost", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "NoOfTriesPerPost");
        }
    }
}
