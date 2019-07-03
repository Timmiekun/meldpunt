namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required_lastmodified : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContentPageModels", "LastModified", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.PlaatsPageModels", "LastModified", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlaatsPageModels", "LastModified", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.ContentPageModels", "LastModified", c => c.DateTimeOffset(precision: 7));
        }
    }
}
