namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_contentPages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentPageModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        MetaTitle = c.String(),
                        Url = c.String(),
                        UrlPart = c.String(nullable: false),
                        Image = c.String(),
                        Content = c.String(),
                        SideContent = c.String(),
                        ParentPath = c.String(),
                        FullText = c.String(),
                        HasSublingMenu = c.Boolean(nullable: false),
                        ParentId = c.String(),
                        LastModified = c.DateTimeOffset(precision: 7),
                        Published = c.DateTimeOffset(precision: 7),
                        Sort = c.Int(nullable: false),
                        InTabMenu = c.Boolean(nullable: false),
                        InHomeMenu = c.Boolean(nullable: false),
                        MetaDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContentPageModels");
        }
    }
}
