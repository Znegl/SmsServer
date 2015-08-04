namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMimeType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "ImageMimeType", c => c.String());
            AddColumn("dbo.Posts", "ImageMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "ImageMimeType");
            DropColumn("dbo.PostAnswers", "ImageMimeType");
        }
    }
}
