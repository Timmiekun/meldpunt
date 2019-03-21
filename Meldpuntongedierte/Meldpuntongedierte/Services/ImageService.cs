using ImageResizer;
using Meldpunt.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Meldpunt.Services
{
  public class ImageService
  {
    string imagesDir = HttpContext.Current.Server.MapPath("~/afbeeldingen");
    string cacheDir = HttpContext.Current.Server.MapPath("~/imagecache");
    DirectoryInfo imageFolder;
    DirectoryInfo cacheFolder;

    public ImageService()
    {
      imageFolder = new DirectoryInfo(imagesDir);
      cacheFolder = new DirectoryInfo(cacheDir);

      if (!cacheFolder.Exists)
        cacheFolder.Create();
    }

    public IEnumerable<ImageModel> GetAllImages()
    {
      return imageFolder.GetFiles("*", SearchOption.AllDirectories).Select(f => new ImageModel
      {
        Name = f.FullName.Substring(f.FullName.IndexOf("afbeeldingen") + 13)
      });
    }

    public byte[] GetImageBytes(string fileName)
    {
      var fileInfo = new FileInfo(Path.Combine(imageFolder.FullName, fileName));
      if (!fileInfo.Exists)
        throw new HttpException(404, "file not found: " + fileName);

      return File.ReadAllBytes(fileInfo.FullName);

    }

    public FileInfo GetCachedFileInfo(string fileName)
    {
      return new FileInfo(Path.Combine(cacheFolder.FullName, fileName));
    }

    public string BuildResizedFilenameFromParams(string source, int height, int width, FitMode mode)
    {
      source = source.Split('\\').Last();
      return string.Format(CultureInfo.InvariantCulture,
        "{0}_{1}-{2}-{3}", height, width, mode, source.Replace("/", string.Empty));
    }

    public void saveImage(HttpPostedFileBase file)
    {
      // get filename without path
      var fileName = Path.GetFileName(file.FileName);

      // store the file
      var path = Path.Combine(imageFolder.FullName, fileName);
      file.SaveAs(path);
    }
  }
}