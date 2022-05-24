namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Author_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Ideas", "Author_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Comments", "Author_Id");
            CreateIndex("dbo.Ideas", "Author_Id");
            AddForeignKey("dbo.Comments", "Author_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Ideas", "Author_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ideas", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Ideas", new[] { "Author_Id" });
            DropIndex("dbo.Comments", new[] { "Author_Id" });
            DropColumn("dbo.Ideas", "Author_Id");
            DropColumn("dbo.Comments", "Author_Id");
        }
    }
}
