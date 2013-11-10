using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Meldpunt.Controllers
{
  public class ErrorController : Controller
  {
    public ErrorController()
    {
    
    }

    public ActionResult General(Exception exception)
    {
      Response.StatusCode = 500;
      return View();
    }

    public ActionResult Http404()
    {
      Response.StatusCode = 404;
      return View();
    }

    public ActionResult Http403()
    {
      Response.StatusCode = 403;
      return View();
    }
  }
}