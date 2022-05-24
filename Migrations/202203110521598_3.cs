namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "FileName");
        }
    }
}
