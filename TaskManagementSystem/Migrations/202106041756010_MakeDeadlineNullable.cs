namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDeadlineNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Projects", "Deadline", c => c.DateTime());
            AlterColumn("dbo.ProjectTasks", "Deadline", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProjectTasks", "Deadline", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Projects", "Deadline", c => c.DateTime(nullable: false));
        }
    }
}
