namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class plaatsPages_and_reactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlaatsPageModels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UrlPart = c.String(),
                        Gemeentenaam = c.String(),
                        Published = c.DateTimeOffset(precision: 7),
                        MetaTitle = c.String(),
                        MetaDescription = c.String(),
                        PhoneNumber = c.String(),
                        Content = c.String(),
                        LastModified = c.DateTimeOffset(precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReactionModels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        GemeentePage = c.Guid(),
                        GemeenteNaam = c.String(),
                        Sender = c.String(),
                        SenderEmail = c.String(),
                        SenderDescription = c.String(),
                        Approved = c.DateTimeOffset(precision: 7),
                        Created = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReactionModels");
            DropTable("dbo.PlaatsPageModels");
        }
    }
}
