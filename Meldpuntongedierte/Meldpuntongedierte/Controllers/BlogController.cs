using Meldpunt.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{

  [RoutePrefix("blog")]
  public class BlogController : Controller
  {
    MeldpuntContext context;

    public BlogController()
    {
      context = new MeldpuntContext();
    }


    [Route]
    public ActionResult Index()
    {
      return View(context.BlogModels.ToList());
    }


    //[OutputCache(Duration = 10, VaryByParam = "none")]
    public ActionResult Details()
    {
      var id = Guid.Parse(RouteData.Values["id"].ToString());
      var model = context.BlogModels.Find(id);
      return View(model);
    }
  }
}
