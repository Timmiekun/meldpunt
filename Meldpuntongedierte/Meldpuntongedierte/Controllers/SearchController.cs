using System;
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
    IImageService imageService;

    public SearchController(IPageService _pageService, ISearchService _searchService, IImageService _imageService)
    {
      pageService = _pageService;
      searchService = _searchService;
      imageService = _imageService;
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
      searchService.IndexItems(imageService.GetAllImages());

      return new EmptyResult();
    }

    public ActionResult SearchPages(String q)
    {
      if (LocationUtils.IsLocation(q))
        return Redirect("/ongediertebestrijding-" + q.XmlSafe());
      
      var results = searchService.Search(q, SearchTypes.Page);
      if (results.Total == 1)
        return Redirect(results.Results.First().Url);

      return View("index", results);
    }
  }
}
