namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_parentPath_from_page : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ContentPageModels", "ParentPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContentPageModels", "ParentPath", c => c.String());
        }
    }
}
