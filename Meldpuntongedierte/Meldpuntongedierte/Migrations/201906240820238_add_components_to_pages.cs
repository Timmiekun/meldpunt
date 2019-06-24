namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_components_to_pages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentPageModels", "Components", c => c.String());
            AddColumn("dbo.PlaatsPageModels", "Components", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaatsPageModels", "Components");
            DropColumn("dbo.ContentPageModels", "Components");
        }
    }
}
