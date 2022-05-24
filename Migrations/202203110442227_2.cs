namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Files", newName: "Documents");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Documents", newName: "Files");
        }
    }
}
