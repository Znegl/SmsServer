namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPositionAndNextPost : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts");
            AddColumn("dbo.PostAnswers", "Post_Id", c => c.Int());
            AddColumn("dbo.PostAnswers", "NextPost_Id", c => c.Int());
            AddColumn("dbo.Posts", "longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Posts", "lattitude", c => c.Double(nullable: false));
            CreateIndex("dbo.PostAnswers", "Post_Id");
            CreateIndex("dbo.PostAnswers", "NextPost_Id");
            AddForeignKey("dbo.PostAnswers", "NextPost_Id", "dbo.Posts", "Id");
            AddForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.PostAnswers", "NextPost_Id", "dbo.Posts");
            DropIndex("dbo.PostAnswers", new[] { "NextPost_Id" });
            DropIndex("dbo.PostAnswers", new[] { "Post_Id" });
            DropColumn("dbo.Posts", "lattitude");
            DropColumn("dbo.Posts", "longitude");
            DropColumn("dbo.PostAnswers", "NextPost_Id");
            DropColumn("dbo.PostAnswers", "Post_Id");
            AddForeignKey("dbo.PostAnswers", "PostID", "dbo.Posts", "Id", cascadeDelete: true);
        }
    }
}
