using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Utils;
using Meldpunt.ActionFilters;
using System.Diagnostics;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private ISearchService searchService;
    private IContentPageService contentPageService;
    private IPlaatsPageService plaatsPageeService;
    private IImageService imageService;

    public SearchController(ISearchService _searchService, IContentPageService _contentPageService, IPlaatsPageService _plaatsPageeService, IImageService _imageService)
    {
      searchService = _searchService;
      contentPageService = _contentPageService;
      plaatsPageeService = _plaatsPageeService;
      imageService = _imageService;
    }

    [MustBeAdmin]
    [Route("search/index")]
    public ActionResult Index()
    {
      var sw = new Stopwatch();
      sw.Start();
      WriteLine("Start indexing..");

      WriteLine("Indexing pages..");
      var allPages = contentPageService.GetAllPages();
      searchService.IndexItems(allPages, true);

      WriteLine("Indexing places..");
      var allPlaces = plaatsPageeService.GetAllPlaatsModels();
      searchService.IndexItems(allPlaces);

      WriteLine("Indexing images..");
      var allImages = imageService.GetAllImages();
      searchService.IndexItems(allImages);

      sw.Stop();
      WriteLine("Finished in " + sw.Elapsed.ToString("c"));

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

    private void WriteLine(string text, string color = "blue")
    {
      Response.Write(String.Format("<p style=\"margin:0;color:{1}\">{0}</p>", text, color));
      Response.Flush();
    }
  }
}
