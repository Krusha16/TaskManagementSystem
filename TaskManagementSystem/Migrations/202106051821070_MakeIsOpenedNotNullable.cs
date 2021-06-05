namespace TaskManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeIsOpenedNotNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notifications", "IsOpened", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notifications", "IsOpened", c => c.Boolean());
        }
    }
}
