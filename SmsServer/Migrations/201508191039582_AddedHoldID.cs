namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHoldID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "HoldID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "HoldID");
        }
    }
}
