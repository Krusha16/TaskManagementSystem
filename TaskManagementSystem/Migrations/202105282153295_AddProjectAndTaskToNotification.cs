namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectAndTaskToNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "ProjectId", c => c.Int());
            AddColumn("dbo.Notifications", "ProjectTaskId", c => c.Int());
            CreateIndex("dbo.Notifications", "ProjectId");
            CreateIndex("dbo.Notifications", "ProjectTaskId");
            AddForeignKey("dbo.Notifications", "ProjectId", "dbo.Projects", "Id");
            AddForeignKey("dbo.Notifications", "ProjectTaskId", "dbo.ProjectTasks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "ProjectTaskId", "dbo.ProjectTasks");
            DropForeignKey("dbo.Notifications", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Notifications", new[] { "ProjectTaskId" });
            DropIndex("dbo.Notifications", new[] { "ProjectId" });
            DropColumn("dbo.Notifications", "ProjectTaskId");
            DropColumn("dbo.Notifications", "ProjectId");
        }
    }
}
