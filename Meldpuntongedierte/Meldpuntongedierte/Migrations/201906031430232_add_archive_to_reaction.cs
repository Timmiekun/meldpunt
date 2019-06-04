namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_archive_to_reaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReactionModels", "Archived", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReactionModels", "Archived");
        }
    }
}
