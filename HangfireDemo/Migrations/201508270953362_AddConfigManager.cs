namespace HangfireDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfigManager : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfigManagers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestUrl = c.String(nullable: false),
                        Times = c.String(),
                        CreatePerson = c.String(),
                        CreateTime = c.DateTime(),
                        IsValid = c.Boolean(nullable: false),
                        IsRuning = c.Boolean(nullable: false),
                        JobName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JobName = c.String(nullable: false),
                        RequestTime = c.DateTime(nullable: false),
                        Status = c.String(),
                        CreatTime = c.DateTime(nullable: false),
                        ErrorMsg = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.CodeSnippets");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.CodeSnippets",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            SourceCode = c.String(nullable: false),
            //            HighlightedCode = c.String(),
            //            CreatedAt = c.DateTime(nullable: false),
            //            HighlightedAt = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            DropTable("dbo.LogMessages");
            DropTable("dbo.ConfigManagers");
        }
    }
}
