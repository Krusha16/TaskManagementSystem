namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectAndProjectTaskClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.ProjectTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompletionPercentage = c.Int(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                        ProjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectTasks", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectTasks", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ProjectTasks", new[] { "ProjectId" });
            DropIndex("dbo.ProjectTasks", new[] { "ApplicationUserId" });
            DropIndex("dbo.Projects", new[] { "ApplicationUserId" });
            DropTable("dbo.ProjectTasks");
            DropTable("dbo.Projects");
        }
    }
}
