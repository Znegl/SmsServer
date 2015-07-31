namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChagedToList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "Post_Id", c => c.Int());
            AddColumn("dbo.Posts", "Race_Id", c => c.Int());
            AddColumn("dbo.TeamMembers", "Team_Id", c => c.Int());
            CreateIndex("dbo.PostAnswers", "Post_Id");
            CreateIndex("dbo.Posts", "Race_Id");
            CreateIndex("dbo.TeamMembers", "Team_Id");
            AddForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts", "Id");
            AddForeignKey("dbo.TeamMembers", "Team_Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Posts", "Race_Id", "dbo.Races", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "Race_Id", "dbo.Races");
            DropForeignKey("dbo.TeamMembers", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.PostAnswers", "Post_Id", "dbo.Posts");
            DropIndex("dbo.TeamMembers", new[] { "Team_Id" });
            DropIndex("dbo.Posts", new[] { "Race_Id" });
            DropIndex("dbo.PostAnswers", new[] { "Post_Id" });
            DropColumn("dbo.TeamMembers", "Team_Id");
            DropColumn("dbo.Posts", "Race_Id");
            DropColumn("dbo.PostAnswers", "Post_Id");
        }
    }
}
