namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShowHideWebQR : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Races", "ShowWebAnswerQR", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Races", "ShowWebAnswerQR");
        }
    }
}
