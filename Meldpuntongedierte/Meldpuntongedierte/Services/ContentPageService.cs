using HtmlAgilityPack;
using Meldpunt.Models;
using Meldpunt.Services.Interfaces;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml;

namespace Meldpunt.Services
{
  public class ContentPageService : IContentPageService
  {
    private MeldpuntContext db;

    public ContentPageService(MeldpuntContext _db)
    {
      db = _db;
    }

    public IEnumerable<ContentPageModel> GetAllPages()
    {
      return db.ContentPages;
    }


    public ContentPageModel GetPageById(Guid id)
    {
      return db.ContentPages.Find(id);
    }

    public ContentPageModel GetByIdUntracked(Guid id)
    {
      return db.ContentPages.AsNoTracking().FirstOrDefault(p=> p.Id == id);
    }

    public ContentPageModel GetPageByUrlPart(string urlPart)
    {
      return db.ContentPages.FirstOrDefault(p => p.UrlPart == urlPart);
    }

    public IEnumerable<ContentPageModel> GetPagesForTabs()
    {
      return db.ContentPages.Where(p => p.InTabMenu);
    }

    public IEnumerable<ContentPageModel> GetPagesForHomeMenu()
    {
      return db.ContentPages.Where(p => p.InHomeMenu);
    }

    public IEnumerable<ContentPageModel> GetChildPages(Guid id, bool deep)
    {
      var childpages = GetChildPages(id).ToList();

      if (!deep || !childpages.Any())
        return childpages;

      var allPages = new List<ContentPageModel>();
      allPages.AddRange(childpages);
      foreach(var child in childpages)
      {
        var c = GetChildPages(child.Id, true).ToList();
        allPages.AddRange(c);
      }

      return allPages;
    }

    public IEnumerable<ContentPageModel> GetChildPages(Guid id)
    {
      return db.ContentPages.Where(p => p.ParentId == id);
    }


    public ContentPageModel SavePage(ContentPageModel pageToSave)
    {
      pageToSave.UrlPart = pageToSave.UrlPart.XmlSafe();
      pageToSave.Url = "/" + generateUrl(pageToSave);
      pageToSave.LastModified = DateTimeOffset.Now;
      db.Entry(pageToSave).State = EntityState.Modified;
      db.SaveChanges();

      return pageToSave;
    }

    private string generateUrl(ContentPageModel page)
    {
      var parents = GetParentPath(page, new List<ContentPageModel>());    
      return string.Join("/", parents.Select(p => p.UrlPart));
    }

    private List<ContentPageModel> GetParentPath(ContentPageModel page, List<ContentPageModel> parents)
    {
      var parent = GetPageById(page.ParentId);
      if (parent.UrlPart != "home")
        GetParentPath(parent, parents);

      parents.Add(page);
      return parents;
    }

    public void deletePage(Guid id)
    {
      ContentPageModel model = db.ContentPages.Find(id);
      db.ContentPages.Remove(model);
      db.SaveChanges();
    }


  }
}