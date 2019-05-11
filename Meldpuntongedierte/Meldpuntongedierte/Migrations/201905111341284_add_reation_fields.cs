namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_reation_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReactionModels", "AllowDisplayOnSite", c => c.Boolean(nullable: false));
            AddColumn("dbo.ReactionModels", "AllowContact", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReactionModels", "AllowContact");
            DropColumn("dbo.ReactionModels", "AllowDisplayOnSite");
        }
    }
}
