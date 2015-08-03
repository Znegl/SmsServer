namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageToModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "Image", c => c.Binary());
            AddColumn("dbo.Posts", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Image");
            DropColumn("dbo.PostAnswers", "Image");
        }
    }
}
