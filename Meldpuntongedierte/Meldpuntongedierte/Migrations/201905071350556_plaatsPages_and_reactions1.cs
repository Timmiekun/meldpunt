namespace Meldpunt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class plaatsPages_and_reactions1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReactionModels", "SenderPhone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReactionModels", "SenderPhone");
        }
    }
}
