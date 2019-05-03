namespace Meldpunt.Migrations
{
  using Meldpunt.Models;
  using Meldpunt.Services;
  using Meldpunt.Utils;
  using System;
  using System.Data.Entity.Migrations;
  using System.Linq;
  using System.Web.Mvc;

  internal sealed class Configuration : DbMigrationsConfiguration<Meldpunt.Models.MeldpuntContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
      ContextKey = "Meldpunt.Models.MeldpuntContext";
    }

    protected override void Seed(Meldpunt.Models.MeldpuntContext context)
    {
      //  This method will be called after migrating to the latest version.

    
      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data.
    }
  }
}
