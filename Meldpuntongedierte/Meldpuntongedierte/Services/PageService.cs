using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using Meldpunt.Models;
using System.Web;
using Meldpunt.Utils;

namespace Meldpunt.Services
{
  public class PageService
  {
    private XmlDocument pagesDoc;
    string pageFile = "~/App_Data/pages.xml";

    public PageService()
    {
      pageFile = HttpContext.Current.Server.MapPath(pageFile);
      pagesDoc = new XmlDocument();
      pagesDoc.Load(pageFile);
    }

    public List<PageModel> GetAllPages()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      return XmlToModel(pages);
    }

    public List<PageModel> GetPagesForTabs()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("page[@tab='true']");
      return XmlToModel(pages);
    }

    public PageModel GetPage(string pageId)
    {
      XmlNode page = pagesDoc.DocumentElement.SelectSingleNode("//page[@id='" + pageId + "']");
      if (page == null)
        return null;

      return XmlToModel(page);
    }

    public PageModel SavePage(FormCollection form)
    {
      PageModel page = this.GetPage(form["pageId"]);
      XmlNode pageNode = pagesDoc.SelectSingleNode("//page[@id='" + page.Id + "']");
      pageNode.SelectSingleNode("content").InnerXml = form["content"];
      pagesDoc.Save(pageFile);
      return page;
    }

    public List<PageModel> SearchPages(string query)
    {
      query = query.Trim();
      XmlNodeList pages = pagesDoc.SelectNodes("//page[contains(content,'" + query + "') or contains(title,'" + query + "') or contains(@id,'" + query + "')]");
      return XmlToModel(pages);
    }

    private List<PageModel> XmlToModel(XmlNodeList pages)
    {
      List<PageModel> pageModels = new List<PageModel>();
      foreach (XmlNode n in pages)
        pageModels.Add(XmlToModel(n));
      return pageModels;
    }

    private PageModel XmlToModel(XmlNode page)
    {
      PageModel p = new PageModel()
      {
        Id = page.Attributes["id"].Value,        
        Content = page.InnerXml,
        SubPages = new List<PageModel>()
      };

      XmlNodeList subPages = page.SelectNodes("pages/page");
      if (subPages != null){
          foreach(XmlNode subPage in subPages)
            p.SubPages.Add(XmlToModel(subPage));
      }

      return p;
    }
  }
}