namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class urlpart_required_for_blog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BlogModels", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.BlogModels", "UrlPart", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BlogModels", "UrlPart", c => c.String());
            AlterColumn("dbo.BlogModels", "Title", c => c.String());
        }
    }
}
