using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Meldpunt.ActionFilters;
using Meldpunt.Utils;
using Meldpunt.Controllers;

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
        "Http404", // Route name
        "Error/404", // URL with parameters
        new { controller = "Error", action = "http404" } // Parameter defaults
      );

      routes.MapRoute(
        "Homepage", // Route name
        "", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
     );

     

      routes.MapRoute(
       "Index", // Route name
       "index", // URL with parameters
       new { controller = "Search", action = "Index" } // Parameter defaults
    );

      routes.MapRoute(
      "Search", // Route name
      "zoek", // URL with parameters
      new { controller = "Plaats", action = "Search" } // Parameter defaults
   );

      routes.MapRoute(
      "SearchPages", // Route name
      "searchpages", // URL with parameters
      new { controller = "Search", action = "SearchPages" } // Parameter defaults
   );

      routes.MapRoute(
         "PlaceSuggest", // Route name
         "api/{action}", // URL with parameters
         new { controller = "Api" } // Parameter defaults
      );

      routes.MapRoute(
       "Plaats", // Route name
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
      "SubSubSubPage", // Route name
      "{a}/{b}/{c}/{id}", // URL with parameters
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

    protected void Application_Error(Object sender, System.EventArgs e)
    {
      var exception = Server.GetLastError();
      var httpException = exception as HttpException;
      
      var routeData = new RouteData();
      routeData.Values["controller"] = "Error";      
      if (httpException != null)
      {
        Response.Clear();
        Server.ClearError();

        Response.StatusCode = httpException.GetHttpCode();
        switch (Response.StatusCode)
        {
          case 400:
            routeData.Values["action"] = "Http404";
            break;
          case 403:
            routeData.Values["action"] = "Http403";
            break;
          case 404:
            routeData.Values["action"] = "Http404";
            break;        
        }

        IController errorsController = new ErrorController();
        var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
        errorsController.Execute(rc);
      }

    
    }
  }
}