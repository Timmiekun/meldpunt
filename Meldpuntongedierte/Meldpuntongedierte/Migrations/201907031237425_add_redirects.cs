namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_redirects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RedirectModels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        From = c.String(nullable: false),
                        To = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RedirectModels");
        }
    }
}
