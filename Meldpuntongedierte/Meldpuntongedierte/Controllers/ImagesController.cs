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

    public ImagesController()
    {
      imageService = new ImageService();
    }

    [Route("images")]
    public ActionResult Images()
    {
      return View("~/Views/Admin/Images.cshtml", imageService.GetAllImages());
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
    public ActionResult editImage(string image)
    {
      return View("~/Views/Admin/EditImage.cshtml", image);
    }
  }
}
