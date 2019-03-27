using Meldpunt.ActionFilters;
using Meldpunt.Services;
using System.Web;
using System.Web.Mvc;

namespace Meldpunt.Controllers
{
  [MustBeAdmin]
  [RoutePrefix("admin")]
  public class ImagesController : Controller
  {
    private ImageService imageService;
    private SearchService searchService;

    public ImagesController()
    {
      imageService = new ImageService();
      searchService = new SearchService();
    }

    [Route("images")]
    public ActionResult Images(string q)
    {
      var model = searchService.Search(q, SearchTypes.Image);
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
        imageService.saveImage(file);
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
      return RedirectToAction("Images");
    }
  }
}
