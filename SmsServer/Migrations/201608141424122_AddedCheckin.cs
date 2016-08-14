namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCheckin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Checkins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckIn = c.DateTime(nullable: false),
                        CheckOut = c.DateTime(),
                        TeamId = c.Int(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Checkins", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Checkins", "PostId", "dbo.Posts");
            DropIndex("dbo.Checkins", new[] { "PostId" });
            DropIndex("dbo.Checkins", new[] { "TeamId" });
            DropTable("dbo.Checkins");
        }
    }
}
