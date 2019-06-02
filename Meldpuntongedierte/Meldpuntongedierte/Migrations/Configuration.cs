using System;
using System.Configuration;
using System.Data.Entity.Migrations;

namespace Meldpunt.Migrations
{
  internal sealed class Configuration : DbMigrationsConfiguration<Meldpunt.Models.MeldpuntContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = Boolean.Parse(ConfigurationManager.AppSettings["AutomaticMigrationsEnabled"] ?? "false");
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
