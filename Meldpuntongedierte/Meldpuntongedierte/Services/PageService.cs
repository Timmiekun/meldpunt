﻿using System;
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

    public List<PageModel> GetAllPagesTree()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("page");
      return XmlToModel(pages, true);
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

    public void SavePage(PageModel p)
    {
      XmlElement page = (XmlElement)pagesDoc.SelectSingleNode("//page[@id='" + p.Id + "']");

      string html = String.Format("<![CDATA[{0}]]>", p.Content);

      if (p != null)
      {
        page.SetAttribute("published", p.Published ? "true" : "false");
        page.SelectSingleNode("content").InnerXml = html;
        
        if (page.SelectSingleNode("metadescription") == null)
          page.AppendChild(pagesDoc.CreateElement("metadescription"));
        
        page.SelectSingleNode("metadescription").InnerText = p.MetaDescription;
        if (page.SelectSingleNode("title") == null)
          page.AppendChild(pagesDoc.CreateElement("title"));
        
        page.SelectSingleNode("title").InnerText = p.Title;
      }
      else
      {
        page = pagesDoc.CreateElement("plaats");        
        XmlElement content = pagesDoc.CreateElement("content");
        XmlElement metaDescription = pagesDoc.CreateElement("metadescription");
        XmlElement title = pagesDoc.CreateElement("title");
        content.InnerXml = html;
        metaDescription.InnerText = p.MetaDescription;
        title.InnerText = p.Title;
        page.AppendChild(content);
        page.AppendChild(metaDescription);
        page.AppendChild(title);
        pagesDoc.DocumentElement.AppendChild(page);
      }

      page.SetAttribute("tab", p.InTabMenu ? "true" : "false");
      page.SetAttribute("published", p.Published ? "true" : "false");
      page.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      pagesDoc.Save(pageFile);
    }

    public List<PageModel> SearchPages(string query)
    {
      query = query.Trim();
      XmlNodeList pages = pagesDoc.SelectNodes("//page[contains(content,'" + query + "') or contains(title,'" + query + "') or contains(@id,'" + query + "')]");
      return XmlToModel(pages);
    }

    private List<PageModel> XmlToModel(XmlNodeList pages, bool deep = false)
    {
      List<PageModel> pageModels = new List<PageModel>();
      foreach (XmlNode n in pages)
        pageModels.Add(XmlToModel(n, deep));
      return pageModels;
    }

    private PageModel XmlToModel(XmlNode page, bool deep = true)
    {
      List<PageModel> subpages = new List<PageModel>();

      XmlNodeList subPages = page.SelectNodes("pages/page");
      if (subPages != null)
      {
        foreach (XmlNode subPage in subPages)
          subpages.Add(XmlToModel(subPage, true));
      }

      string id = page.Attributes["id"].Value;
      string url = "/" + id;
      XmlNode parent = page.SelectSingleNode("../../.");
      string parentId = (parent != null && parent != page.OwnerDocument) ? parent.Attributes["id"].Value : "";
      while (parent != null && parent != page.OwnerDocument)
      {
        url = "/" + parent.Attributes["id"].Value + url;
        parent = parent.SelectSingleNode("../../.");
      }

      String text = "";
      foreach (XmlNode t in page.SelectNodes("content//p"))
        text += " " + t.InnerText;

      foreach (XmlNode t in page.SelectNodes("content//h1"))
        text += " " + t.InnerText;

      string headertitle = "";
      if (page.SelectSingleNode("content/h1") != null)
        headertitle = page.SelectSingleNode("content/h1").InnerText;

      string metadescription = null;
      if (page.SelectSingleNode("metadescription") != null)
        metadescription = page.SelectSingleNode("metadescription").InnerText;

      string html = page.SelectSingleNode("content") != null ? page.SelectSingleNode("content").InnerXml : "";
      if(html.StartsWith("<![CDATA["))
        html = html.Trim().Substring(9, html.Length - 12);

      PageModel p = new PageModel()
      {
        Id = id,
        Content = html,
        SubPages = subpages,
        Url = url.UrlEncode(),
        ParentId = parentId,
        HasSublingMenu = page.Attributes["haschildmenu"] != null && page.Attributes["haschildmenu"].Value == "true",
        FullText = text,
        HeaderTitle = headertitle,
        InTabMenu = page.Attributes["tab"] != null && page.Attributes["tab"].Value == "true",
        MetaDescription = metadescription
      };

      return p;
    }
  }
}