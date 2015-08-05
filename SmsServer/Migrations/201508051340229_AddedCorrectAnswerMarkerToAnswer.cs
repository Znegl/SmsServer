namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCorrectAnswerMarkerToAnswer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "CorrectAnswerChosen", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answers", "CorrectAnswerChosen");
        }
    }
}
