namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedCorrectAnswerToPostAnswer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Posts", "CorrectAnswer_Id", "dbo.PostAnswers");
            DropForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts");
            DropIndex("dbo.PostAnswers", new[] { "PostID" });
            DropIndex("dbo.PostAnswers", new[] { "Post_Id" });
            DropIndex("dbo.Posts", new[] { "CorrectAnswer_Id" });
            DropColumn("dbo.PostAnswers", "PostID");
            RenameColumn(table: "dbo.PostAnswers", name: "Post_Id", newName: "PostID");
            AddColumn("dbo.PostAnswers", "CorrectAnswer", c => c.Boolean(nullable: false));
            AlterColumn("dbo.PostAnswers", "PostID", c => c.Int(nullable: false));
            CreateIndex("dbo.PostAnswers", "PostID");
            AddForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts", "Id", cascadeDelete: true);
            DropColumn("dbo.Posts", "CorrectAnswer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Posts", "CorrectAnswer_Id", c => c.Int());
            DropForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts");
            DropIndex("dbo.PostAnswers", new[] { "PostID" });
            AlterColumn("dbo.PostAnswers", "PostID", c => c.Int());
            DropColumn("dbo.PostAnswers", "CorrectAnswer");
            RenameColumn(table: "dbo.PostAnswers", name: "PostID", newName: "Post_Id");
            AddColumn("dbo.PostAnswers", "PostID", c => c.Int(nullable: false));
            CreateIndex("dbo.Posts", "CorrectAnswer_Id");
            CreateIndex("dbo.PostAnswers", "Post_Id");
            CreateIndex("dbo.PostAnswers", "PostID");
            AddForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts", "Id");
            AddForeignKey("dbo.Posts", "CorrectAnswer_Id", "dbo.PostAnswers", "Id");
        }
    }
}
