namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRaceInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "GatewayNumber", c => c.String());
            AddColumn("dbo.Races", "GatewayCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "GatewayCode");
            DropColumn("dbo.Races", "GatewayNumber");
        }
    }
}
