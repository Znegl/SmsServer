namespace SmsServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSentAndDelayedSms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DelayedSms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reciever = c.String(),
                        Body = c.String(),
                        SendOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SentSms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reciever = c.String(),
                        Body = c.String(),
                        Sent = c.DateTime(nullable: false),
                        SentSuccessfully = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SentSms");
            DropTable("dbo.DelayedSms");
        }
    }
}
