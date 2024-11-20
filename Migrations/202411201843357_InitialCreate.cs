namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiagnosticInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(),
                        IPAddress = c.String(),
                        UserName = c.String(),
                        SystemLanguage = c.String(),
                        Antivirus = c.String(),
                        CurrentTime = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        PID = c.Int(nullable: false),
                        StartTime = c.String(),
                        MemoryKB = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScreenshotInfoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(),
                        ImageData = c.Binary(),
                        CaptureTime = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VideoFrames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.String(),
                        FrameData = c.Binary(),
                        CaptureTime = c.DateTime(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VideoFrames");
            DropTable("dbo.Users");
            DropTable("dbo.ScreenshotInfoes");
            DropTable("dbo.ProcessInfoes");
            DropTable("dbo.DiagnosticInfoes");
        }
    }
}
