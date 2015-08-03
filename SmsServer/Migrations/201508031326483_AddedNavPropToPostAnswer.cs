namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNavPropToPostAnswer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "PostID", c => c.Int(nullable: false));
            CreateIndex("dbo.PostAnswers", "PostID");
            AddForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts");
            DropIndex("dbo.PostAnswers", new[] { "PostID" });
            DropColumn("dbo.PostAnswers", "PostID");
        }
    }
}
