namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedforeignkey : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PostAnswers", name: "NextPost_Id", newName: "NextPostId");
            RenameIndex(table: "dbo.PostAnswers", name: "IX_NextPost_Id", newName: "IX_NextPostId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PostAnswers", name: "IX_NextPostId", newName: "IX_NextPost_Id");
            RenameColumn(table: "dbo.PostAnswers", name: "NextPostId", newName: "NextPost_Id");
        }
    }
}
