using Meldpunt.Models.Domain;
using Meldpunt.Services;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Meldpunt.Controllers
{
  public class ErrorController : Controller
  {
    private ISearchService searchservice;

    public ErrorController(ISearchService _searchservice)
    {
      searchservice = _searchservice;
    }

    public ActionResult General(Exception exception)
    {
      Response.StatusCode = 500;
      if (HttpContext.AllErrors.Any())
      {
        var error = HttpContext.AllErrors[0];
        ViewBag.Error = error.Message;
      }

      string filePath = HostingEnvironment.MapPath("/_logs/500-.txt");
      Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

      Log.Error(exception, "Something error happened");
      Log.CloseAndFlush();

      return View();
    }

    public ActionResult Http404()
    {
      string filePath = HostingEnvironment.MapPath("/_logs/404-.txt");
      Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

      Log.Information("-------------------------------------------------------------------");
      Log.Information("Page not found: {0}", Request.Url.PathAndQuery);
      if (Request.UrlReferrer != null)
        Log.Information("Referrer {0}", Request.UrlReferrer);
      if (Request.UserAgent != null)
        Log.Information("UserAgent {0}", Request.UserAgent);
      

      if (Request.Path.StartsWith("/ongediertebestrijding-"))
      {
        // get plaatsnaam. Should be after the first "-"
        string plaats = Request.Path.Substring(Request.Path.IndexOf('-') + 1);

        var results = searchservice.SearchPlaatsen(plaats);
        if (results.Total > 0)
        {
          // should be first result
          var plaatsresult = results.Results.First();
          Log.Information("--> Redirecting to: {0}", plaatsresult.Url);
          Log.CloseAndFlush();
          return RedirectPermanent(plaatsresult.Url);
        }
      }

      Log.CloseAndFlush();

      Response.StatusCode = 404;
      Response.ContentType = "text/html";

      return View();
    }

    public ActionResult Http403()
    {
      Response.StatusCode = 403;
      return View();
    }
  }
}