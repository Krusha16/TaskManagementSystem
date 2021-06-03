namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFlagFieldToComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Flag", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Flag");
        }
    }
}
