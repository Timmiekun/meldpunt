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
    private ISearchService searchService;
    IImageService imageService;

    public SearchController(ISearchService _searchService, IImageService _imageService)
    {
      searchService = _searchService;
      imageService = _imageService;
    }

    [MustBeAdmin]
    [Route("search/index")]
    public ActionResult Index()
    {
      searchService.Index();

      return new EmptyResult();
    }

    [MustBeAdmin]
    [Route("search/indeximages")]
    public ActionResult IndexImages()
    {
      searchService.IndexItems(imageService.GetAllImages());

      return new EmptyResult();
    }

    [Route("zoek")]
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
