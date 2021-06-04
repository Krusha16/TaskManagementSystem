namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalCostToProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "TotalCost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "TotalCost");
        }
    }
}
