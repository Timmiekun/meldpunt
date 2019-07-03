namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_date_to_redirects : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RedirectModels", "Created", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.RedirectModels", "LastModified", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RedirectModels", "LastModified");
            DropColumn("dbo.RedirectModels", "Created");
        }
    }
}
