namespace Meldpunt.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class add_blog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        MetaTitle = c.String(),
                        MetaDescription = c.String(),
                        Content = c.String(),
                        Url = c.String(),
                        UrlPart = c.String(),
                        ParentId = c.String(),
                        LastModified = c.DateTimeOffset(precision: 7),
                        Published = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.ImageModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ImageModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.BlogModels");
        }
    }
}
