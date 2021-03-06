﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Meldpunt.Models;
using System.Web.Routing;
using Meldpunt.Services.Interfaces;
using Meldpunt.Models.Domain;
using Meldpunt.Models.ViewModels;

namespace Meldpunt.Controllers
{

  public class HomeController : Controller
  {
    private IContentPageService pageService;
    private IPlaatsPageService placePageService;
    private MeldpuntContext db;

    public HomeController(IContentPageService _pageService, IPlaatsPageService _placePageService, MeldpuntContext _db)
    {
      pageService = _pageService;
      placePageService = _placePageService;
      db = _db;
    }

    [OutputCache(CacheProfile = "pageCache")]
    [Route]
    public ActionResult Index()
    {
      ContentPageModel model = pageService.GetPageByUrlPart("home");
      return View(model);
    }


    [OutputCache(CacheProfile = "pageCache")]
    public ActionResult GetPage()
    {
      Guid id = Guid.Parse(RouteData.Values["guid"].ToString());
      // content page?
      ContentPageModel model = pageService.GetPageById(id);
      if (model != null)
      {
        // correct url?
        if (Request.Path != model.Url)
        {
          return RedirectPermanent(model.Url);
        }

        if (model.UrlPart == "openbare-ruimte")
          ViewBag.HidePhoneNumber = true;

        var subPages = pageService.GetChildPages(model.Id);
        if (subPages.Any())
        {
          ViewBag.SubNav = subPages;
          return View("index", model);
        }

        ContentPageModel parent = pageService.GetPageById(model.ParentId);
        if (parent != null && parent.UrlPart == "home")
          ViewBag.SubNav = pageService.GetPagesForHomeMenu();

        else if (parent != null)
          ViewBag.SubNav = pageService.GetChildPages(parent.Id);

        return View("index", model);
      }

      throw new HttpException(404, "page not found");
    }

    [Route("sitemap")]
    public ActionResult SiteMap()
    {
      var model = new SitemapViewModel();
      model.Pages = pageService.GetAllPages();
      model.PlacePages = placePageService.GetAll();
      model.BlogItems = db.BlogModels.Where(b => b.LastModified != null && b.Published != null).ToList();

      Response.ContentType = "text/xml";
      return View(model);
    }
  }
}
