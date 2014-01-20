using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Models;
using Meldpunt.Utils;

namespace Meldpunt.Controllers
{
  public class LoginController : Controller
  {
    private PageService pageService;
    private PlaatsService plaatsService;

    public LoginController()
    {
      pageService = new PageService();
      plaatsService = new PlaatsService();
    }

    public ActionResult Login()
    {
      return View();
    }

    public ActionResult Logoff()
    {
      Session.Clear();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
      if (username == "admin" && password == "geheim")
        Session["LoggedIn"] = "loggedIn";
      return Redirect("/admin");
    }

  }
}
