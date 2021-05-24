namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        ProjectTaskId = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.ProjectTasks", t => t.ProjectTaskId, cascadeDelete: true)
                .Index(t => t.ProjectTaskId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ProjectTaskId", "dbo.ProjectTasks");
            DropForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "ApplicationUserId" });
            DropIndex("dbo.Comments", new[] { "ProjectTaskId" });
            DropTable("dbo.Comments");
        }
    }
}
