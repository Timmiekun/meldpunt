using Meldpunt.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Meldpunt.Services
{
  public class ImageService
  {
    string imagesDir = HttpContext.Current.Server.MapPath("~/afbeeldingen");
    DirectoryInfo imageFolder;

    public ImageService()
    {
      imageFolder = new DirectoryInfo(imagesDir);
    }

    public IEnumerable<ImageModel> GetAllImages()
    {
      return imageFolder.GetFiles().Select(f => new ImageModel
      {
        Name = f.Name
      });
    }

    public byte[] GetImageStream(string fileName)
    {
      var fileInfo = imageFolder.GetFiles(fileName).FirstOrDefault();
      return File.ReadAllBytes(fileInfo.FullName);
    }
  }
}