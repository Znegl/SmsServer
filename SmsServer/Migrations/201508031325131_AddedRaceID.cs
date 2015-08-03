namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRaceID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Posts", "Race_Id", "dbo.Races");
            DropIndex("dbo.Posts", new[] { "Race_Id" });
            RenameColumn(table: "dbo.Posts", name: "Race_Id", newName: "RaceID");
            AlterColumn("dbo.Posts", "RaceID", c => c.Int(nullable: false));
            CreateIndex("dbo.Posts", "RaceID");
            AddForeignKey("dbo.Posts", "RaceID", "dbo.Races", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "RaceID", "dbo.Races");
            DropIndex("dbo.Posts", new[] { "RaceID" });
            AlterColumn("dbo.Posts", "RaceID", c => c.Int());
            RenameColumn(table: "dbo.Posts", name: "RaceID", newName: "Race_Id");
            CreateIndex("dbo.Posts", "Race_Id");
            AddForeignKey("dbo.Posts", "Race_Id", "dbo.Races", "Id");
        }
    }
}
