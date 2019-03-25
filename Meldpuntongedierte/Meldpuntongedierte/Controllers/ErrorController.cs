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
      if (HttpContext.AllErrors.Any())
      {
        var error = HttpContext.AllErrors[0];
        ViewBag.Error = error.Message;
      }
      return View();
    }

    public ActionResult Http404()
    {
      Response.StatusCode = 404;

      if (HttpContext.AllErrors != null && HttpContext.AllErrors.Any())
      {
        var error = HttpContext.AllErrors[0];
        ViewBag.Error = error.Message;
      }

      return View();
    }

    public ActionResult Http403()
    {
      Response.StatusCode = 403;
      return View();
    }
  }
}