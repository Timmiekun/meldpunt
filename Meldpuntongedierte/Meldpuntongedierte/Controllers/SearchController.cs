using System;
using System.Linq;
using System.Web.Mvc;
using Meldpunt.Services;
using Meldpunt.Utils;
using Meldpunt.ActionFilters;
using System.Diagnostics;
using Meldpunt.Services.Interfaces;
using Meldpunt.Models.helpers;

namespace Meldpunt.Controllers
{
  public class SearchController : Controller
  {
    private ISearchService searchService;
    private IContentPageService contentPageService;
    private IPlaatsPageService plaatsPageeService;
    private IImageService imageService;
    private IReactionService reactionService;

    public SearchController(ISearchService _searchService, IContentPageService _contentPageService, IPlaatsPageService _plaatsPageeService, IImageService _imageService, IReactionService _reactionService)
    {
      searchService = _searchService;
      contentPageService = _contentPageService;
      plaatsPageeService = _plaatsPageeService;
      imageService = _imageService;
      reactionService = _reactionService;
    }

    [MustBeAdmin]
    [Route("search/index")]
    public ActionResult Index()
    {
      var sw = new Stopwatch();
      sw.Start();
      WriteLine("Start indexing..");

      WriteLine("Indexing pages.. ");
      var allPages = contentPageService.GetAllPages();
      searchService.IndexItems(allPages, true);
      WriteLine(allPages.Count().ToString());

      WriteLine("Indexing places.. ");
      var allPlaces = plaatsPageeService.GetAllPlaatsModels();
      searchService.IndexItems(allPlaces);
      WriteLine(allPlaces.Count().ToString());

      WriteLine("Indexing images.. ");
      var allImages = imageService.GetAllImages();
      searchService.IndexItems(allImages);
      WriteLine(allImages.Count().ToString());

      WriteLine("Indexing reactions.. ");
      var allReactions = reactionService.GetAllReactions();
      searchService.IndexItems(allReactions);
      WriteLine(allReactions.Count().ToString());

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
      SearchRequestOptions options = new SearchRequestOptions()
      {
        Q = q,
        Types = new[] { SearchTypes.Page, SearchTypes.Place }
      };

      var results = searchService.Search(options);
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
