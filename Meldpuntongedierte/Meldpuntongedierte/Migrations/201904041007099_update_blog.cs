namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_blog : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BlogModels", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BlogModels", "Url", c => c.String());
        }
    }
}
