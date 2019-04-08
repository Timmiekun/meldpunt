namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_image_to_blog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogModels", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BlogModels", "Image");
        }
    }
}
