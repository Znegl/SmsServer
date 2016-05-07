namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPointsToPostAnswer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "PointValue", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostAnswers", "PointValue");
        }
    }
}
