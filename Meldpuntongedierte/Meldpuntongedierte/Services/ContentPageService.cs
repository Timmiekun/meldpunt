using HtmlAgilityPack;
using Meldpunt.Models;
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

    public IEnumerable<ContentPageModel> GetChildPages(Guid id)
    {
      return db.ContentPages.Where(p => p.ParentId == id);
    }

    public ContentPageModel SavePage(ContentPageModel pageToSave)
    {
      var pageFromDb = db.ContentPages.Find(pageToSave.Id);

      //check if we should move the page      
      if (pageToSave.ParentId != pageFromDb.ParentId)
      {
        // calculate parent?
      }

      pageToSave.UrlPart = pageToSave.UrlPart.XmlSafe();
      pageToSave.LastModified = DateTimeOffset.Now;
      db.Entry(pageToSave).State = EntityState.Modified;
      db.SaveChanges();

      return pageToSave;
    }

    public void deletePage(Guid id)
    {
      ContentPageModel model = db.ContentPages.Find(id);
      db.ContentPages.Remove(model);
      db.SaveChanges();
    }

   
  }
}