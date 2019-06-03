namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_PlaatsList_to_PlaatsPage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlaatsPageModels", "PlaatsenAsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlaatsPageModels", "PlaatsenAsString");
        }
    }
}
