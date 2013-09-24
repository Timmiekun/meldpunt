using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Meldpunt.ActionFilters;

namespace Meldpunt
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        "Homepage", // Route name
        "", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
     );

      routes.MapRoute(
       "Search", // Route name
       "zoek", // URL with parameters
       new { controller = "Plaats", action = "Search" } // Parameter defaults
    );

      routes.MapRoute(
         "PlaceSuggest", // Route name
         "api/{action}", // URL with parameters
         new { controller = "Api" } // Parameter defaults
      );

      routes.MapRoute(
       "PLaats", // Route name
       "in/{plaats}", // URL with parameters
       new { controller = "Plaats", action = "PLaats", id = UrlParameter.Optional } // Parameter defaults
    );

      routes.MapRoute(
         "Page", // Route name
         "{id}", // URL with parameters
         new { controller = "Home", action = "GetPage", id = UrlParameter.Optional } // Parameter defaults
      );

      routes.MapRoute(
        "SubPage", // Route name
        "{a}/{id}", // URL with parameters
        new { controller = "Home", action = "GetPage", id = UrlParameter.Optional } // Parameter defaults
     );

      routes.MapRoute(
       "SubSubPage", // Route name
       "{a}/{b}/{id}", // URL with parameters
       new { controller = "Home", action = "GetPage", id = UrlParameter.Optional } // Parameter defaults
    );

      routes.MapRoute(
          "Default", // Route name
          "{controller}/{action}/{id}", // URL with parameters
          new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
      );

    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      GlobalFilters.Filters.Add(new GlobalFilterAttribute());
      RegisterGlobalFilters(GlobalFilters.Filters);

      RegisterRoutes(RouteTable.Routes);
    }
  }
}