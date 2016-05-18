namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedimageondisk : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostAnswers", "IsImageOnDisk", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Posts", "IsImageOnDisk", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "IsImageOnDisk");
            DropColumn("dbo.PostAnswers", "IsImageOnDisk");
        }
    }
}
