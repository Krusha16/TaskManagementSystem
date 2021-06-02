namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsOpendFieldToNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "IsOpened", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "IsOpened");
        }
    }
}
