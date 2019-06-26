using Meldpunt.ActionFilters;
using Meldpunt.Models;
using Meldpunt.Models.helpers;
using Meldpunt.Services;
using Meldpunt.Services.Interfaces;
using Meldpunt.Utils;
using System.Web;
using System.Web.Mvc;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class ImagesController : Controller
  {
    private IImageService imageService;
    private ISearchService searchService;

    public ImagesController(ISearchService _searchService, IImageService _imageService)
    {
      imageService = _imageService;
      searchService = _searchService;
    }

    [Route("images")]
    public ActionResult Images(string q, int page = 0)
    {
      var model = searchService.Search(new SearchRequestOptions() { Q = q, Page = page, Filters = { { "type", SearchTypes.Image } } });
      return View("~/Views/Admin/Images.cshtml", model);
    }

    // This action handles the form POST and the upload
    [HttpPost]
    [Route("images")]
    public ActionResult Images(HttpPostedFileBase file)
    {
      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {
        string fullFileName = imageService.saveImage(file);
        var imageModel = new ImageModel
        {
          Name = fullFileName.Substring(fullFileName.IndexOf("afbeeldingen") + 13),
          Url = ""
        };

        searchService.IndexDocument(imageModel.ToLuceneDocument(), imageModel.Name.XmlSafe());
      }

      return RedirectToAction("Images");
    }

    [Route("editImage")]
    public ActionResult EditImage(string image)
    {
      return View("~/Views/Admin/EditImage.cshtml", image);
    }

    [Route("DeleteImage")]
    public ActionResult DeleteImage(string filename)
    {
      imageService.DeleteImage(filename);
      searchService.DeleteDocument(filename.XmlSafe());
      return RedirectToAction("Images");
    }
  }
}
