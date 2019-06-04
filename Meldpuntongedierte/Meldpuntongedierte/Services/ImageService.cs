using ImageResizer;
using Meldpunt.Models;
using Meldpunt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Meldpunt.Services
{
  public class ImageService : IImageService
  {
#pragma warning disable CS0169 // The field 'ImageService.imagesDir' is never used
    string imagesDir;
#pragma warning restore CS0169 // The field 'ImageService.imagesDir' is never used
#pragma warning disable CS0169 // The field 'ImageService.cacheDir' is never used
    string cacheDir;
#pragma warning restore CS0169 // The field 'ImageService.cacheDir' is never used

    DirectoryInfo imageFolder;
    DirectoryInfo cacheFolder;

    private ISearchService searchService;

    public ImageService(ISearchService _searchService)
    {
      string imagesDir = HostingEnvironment.MapPath("~/afbeeldingen");
      string cacheDir = HostingEnvironment.MapPath("~/imagecache");
      imageFolder = new DirectoryInfo(imagesDir);
      cacheFolder = new DirectoryInfo(cacheDir);
      searchService = _searchService;

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

    public void DeleteImage(string fileName)
    {
      FileInfo file = new FileInfo(Path.Combine(imageFolder.FullName, fileName));
      if(!file.Exists)
        throw new HttpException(404, "file not found: " + fileName);

      file.Delete();
    }

    public string saveImage(HttpPostedFileBase file)
    {
      // get filename without path
      var fileName = Path.GetFileName(file.FileName);

      // store the file
      var fullFilePath = Path.Combine(imageFolder.FullName, fileName);
      file.SaveAs(fullFilePath);
      return fullFilePath;
    }
  }
}