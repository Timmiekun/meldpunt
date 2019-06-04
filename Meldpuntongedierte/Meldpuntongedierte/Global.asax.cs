using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services.Interfaces;
using Meldpunt.Utils;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;


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

      routes.MapMvcAttributeRoutes();

      var pageService = DependencyResolver.Current.GetService<IContentPageService>();
      var plaatsPageService = DependencyResolver.Current.GetService<IPlaatsPageService>();

      foreach (var page in pageService.GetAllPages().Where(p => p.Url != null))
      {
        routes.MapRoute(
          page.Id.ToString(), // Route name
          page.Url.TrimStart('/'), // URL with parameters
          new { controller = "Home", action = "GetPage", guid = page.Id } // Parameter defaults
        );
      }

      foreach (var plaatsPage in plaatsPageService.GetAllPlaatsModels())
      {
        // gemeente zelf
        routes.MapRoute(
             plaatsPage.Id.ToString(), // Route name
             plaatsPage.Url.TrimStart('/'), // URL with parameters
             new { controller = "PlaatsPage", action = "GetPlace", guid = plaatsPage.Id } // Parameter defaults
         );

        foreach (var plaats in plaatsPage.Plaatsen.Where(p => !String.IsNullOrWhiteSpace(p)))
        {
          routes.MapRoute(
              plaatsPage.Id.ToString() + plaats.XmlSafe(), // Route name
              "ongediertebestrijding-" + plaats.XmlSafe(), // URL with parameters
              new { controller = "PlaatsPage", action = "GetPlace", guid = plaatsPage.Id } // Parameter defaults
          );
        }
      }

      foreach (var blog in new MeldpuntContext().BlogModels.Where(b => b.UrlPart != null && b.Published.HasValue))
      {
        routes.MapRoute(
        "Blog-" + blog.Id.ToString(), // Route name
        "blog/" + blog.UrlPart.XmlSafe(), // URL with parameters
        new { controller = "Blog", action = "Details", id = blog.Id }
        ); // Parameter defaults
      }

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
      );

      routes.MapRoute(
        "Error",
        "{*url}",
        new { controller = "Error", action = "http404" }
      );

    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      GlobalFilters.Filters.Add(new GlobalFilterAttribute());
      GlobalFilters.Filters.Add(new RedirectFilterAttribute());

      RegisterGlobalFilters(GlobalFilters.Filters);

      RegisterRoutes(RouteTable.Routes);

      ViewEngines.Engines.Add(new CustomViewEngine());

    }

    protected void Application_Error(Object sender, System.EventArgs e)
    {
      //var exception = Server.GetLastError();
      //if (!(exception is HttpException))
      //  throw exception;

      //var httpException = exception as HttpException;

      //var routeData = new RouteData();
      //routeData.Values["controller"] = "Error";
      //if (httpException != null)
      //{
      //  Response.Clear();
      //  Server.ClearError();

      //  Response.StatusCode = httpException.GetHttpCode();
      //  switch (Response.StatusCode)
      //  {
      //    case 400:
      //      routeData.Values["action"] = "Http404";
      //      break;
      //    case 403:
      //      routeData.Values["action"] = "Http403";
      //      break;
      //    case 404:
      //      routeData.Values["action"] = "Http404";
      //      break;
      //    default:
      //      routeData.Values["action"] = "General";
      //      break;
      //  }

      //  IController errorsController = new ErrorController();
      //  var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
      //  errorsController.Execute(rc);
      //}
    }
  }
}