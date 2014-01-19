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
  public class AdminController : Controller
  {
    private PageService pageService;    

    public AdminController()
    {
      pageService = new PageService();      
    }

    public ActionResult Index()
    {
      List<PageModel> allPages = pageService.GetAllPagesTree();
      ViewBag.Locations = LocationUtils.placesByMunicipality.OrderBy(m => m.Key);
      return View(allPages);
    }   
  }
}
