﻿using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Utils;
using Meldpunt.ActionFilters;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private IPageService pageService;
    private ISearchService searchService;

    public SearchController(IPageService _pageService, ISearchService _searchService)
    {
      pageService = new PageService();
      searchService = _searchService;
    }

    [MustBeAdmin]
    public ActionResult Index()
    {
      searchService.Index();
      
      return new EmptyResult();
    }

    [MustBeAdmin]
    public ActionResult IndexImages()
    {
      searchService.IndexImages();

      return new EmptyResult();
    }

    public ActionResult SearchPages(String q)
    {
      if (LocationUtils.IsLocation(q))
        return Redirect("/ongediertebestrijding-" + q.XmlSafe());
      
      var results = searchService.Search(q, SearchTypes.Page);
      if (results.Count == 1)

        return Redirect(results.First().Url);
      return View("index", results);
    }
  }
}
