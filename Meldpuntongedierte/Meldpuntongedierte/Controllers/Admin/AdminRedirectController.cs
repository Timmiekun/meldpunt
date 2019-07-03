using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class AdminRedirectController : Controller
  {

    private XmlRedirectService redirectXmlService;
    IRedirectService redirectService;

    public AdminRedirectController(IRedirectService _redirectService)
    {
      redirectXmlService = new XmlRedirectService();
      redirectService = _redirectService;
    }

    [Route("Redirects")]
    public ActionResult Redirects()
    {
      return View(redirectService.GetAllRedirects());
    }

    [HttpPost]
    [Route("Redirects")]
    public ActionResult Redirects(RedirectModel redirect)
    {
      if (!ModelState.IsValid)
        return View(redirectService.GetAllRedirects());

      var existingRoute = RouteTable.Routes
        .OfType<Route>()
        .Select(r => (Route)r)
        .FirstOrDefault(r => r.Url == redirect.From.TrimStart('/'));

      if (existingRoute != null)
      {
        ModelState.AddModelError("alreadyExists", "Deze url wordt gebruikt door een bestaande pagina");
        return View(redirectService.GetAllRedirects());
      }

      var existingRedirect = redirectService.FindByFrom(redirect.From);
      if (existingRedirect != null && existingRedirect.Id != redirect.Id)
      {
        ModelState.AddModelError("alreadyExists", "Er bestaat al een redirect van deze url (" + redirect.From + ")");
        return View(redirectService.GetAllRedirects());
      }

      redirectService.SaveRedirect(redirect);

      Response.RemoveOutputCacheItem(redirect.From);

      return View(redirectService.GetAllRedirects());
    }  

    [Route("RemoveRedirect")]
    public ActionResult RemoveRedirect(Guid id)
    {
      redirectService.DeleteRedirect(id);
      return RedirectToAction("Redirects");
    }

    [Route("MigrateRedirects")]
    public ActionResult MigrateRedirects()
    {

      foreach (var redirect in redirectXmlService.GetAllRedirects())
      {
        if (String.IsNullOrWhiteSpace(redirect.From))
        {
          continue;
        }

        if (String.IsNullOrWhiteSpace(redirect.To))
        {
          continue;
        }

        var newRedirect = new RedirectModel();

        newRedirect.From = redirect.From;
        newRedirect.To = redirect.To;

        

        WriteLine(String.Format("Added redirect from [{0}] to [{1}]", redirect.From, redirect.To));

        redirectService.SaveRedirect(newRedirect);
      }

      return new EmptyResult();

    }

    private void WriteLine(string text, string color = "blue")
    {
      Response.Write(String.Format("<p style=\"margin:0;color:{1}\">{0}</p>", text, color));
      Response.Flush();
    }
  }
}
