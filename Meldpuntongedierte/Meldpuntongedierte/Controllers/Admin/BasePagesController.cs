using Meldpunt.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  public class BasePagesController : Controller
  {
    public BasePagesController()
    {
    }

    public void UpdateRouteForPages(IEnumerable<RouteableItem> items)
    {
      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        //get last route (default).  ** by convention, it is the standard route.
        var defaultRoute = routes.Last();
        routes.Remove(defaultRoute);

        var defaultRouteOld = routes.Last();
        routes.Remove(defaultRouteOld);

        foreach (var routeItem in items)
        {
          // remove old route
          var oldRoute = routes[routeItem.RouteName];
          routes.Remove(oldRoute);

          //add some new route for a cms page
          routes.MapRoute(
            routeItem.RouteName, // Route name
            routeItem.Url.TrimStart('/'), // URL with parameters
            new { controller = routeItem.Controller, action = routeItem.Action, guid = routeItem.RouteName } // Parameter defaults
          );
        }

        //add back default routes
        routes.Add(defaultRouteOld);
        routes.Add(defaultRoute);
      }
    }

    public void DeleteRouteById(Guid id)
    {
      var routes = RouteTable.Routes;
      using (routes.GetWriteLock())
      {
        // remove route
        var oldRoute = routes[id.ToString()];
        routes.Remove(oldRoute);
      }
    }
  }
}
