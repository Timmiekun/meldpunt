using Serilog;
using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

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
      Response.ContentType = "text/html";

      string filePath = HostingEnvironment.MapPath("/_logs/404-.txt");
      Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

      Log.Information("Page not found: {0}", Request.Url.PathAndQuery);
      Log.CloseAndFlush();

      return View();
    }

    public ActionResult Http403()
    {
      Response.StatusCode = 403;
      return View();
    }
  }
}