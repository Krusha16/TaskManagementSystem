namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBudgetProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Salary", c => c.Double(nullable: false));
            AddColumn("dbo.Projects", "StartDate", c => c.DateTime());
            AddColumn("dbo.Projects", "FinishDate", c => c.DateTime());
            AddColumn("dbo.Projects", "Budget", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Budget");
            DropColumn("dbo.Projects", "FinishDate");
            DropColumn("dbo.Projects", "StartDate");
            DropColumn("dbo.AspNetUsers", "Salary");
        }
    }
}
