namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contentPages_update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContentPageModels", "ParentId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContentPageModels", "ParentId", c => c.String());
        }
    }
}
