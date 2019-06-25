namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_basepagemodel : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ContentPageModels");
            AlterColumn("dbo.ContentPageModels", "Id", c => c.Guid(nullable: false, identity: true, defaultValueSql: "newsequentialid()"));
            AlterColumn("dbo.ContentPageModels", "UrlPart", c => c.String());
            AddPrimaryKey("dbo.ContentPageModels", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ContentPageModels");
            AlterColumn("dbo.ContentPageModels", "UrlPart", c => c.String(nullable: false));
            AlterColumn("dbo.ContentPageModels", "Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.ContentPageModels", "Id");
        }
    }
}
