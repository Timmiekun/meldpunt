using HtmlAgilityPack;
using Meldpunt.Models;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Xml;

namespace Meldpunt.Services
{
  public class XMLPageService : IPageService
  {
    private XmlDocument pagesDoc;
    string pageFile = "~/App_Data/pages.xml";

    public XMLPageService()
    {
      pageFile = HostingEnvironment.MapPath(pageFile);
      pagesDoc = new XmlDocument();
      pagesDoc.Load(pageFile);
    }


    public IEnumerable<PageModel> GetAllPages()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      return XmlToModel(pages);
    }

    public IEnumerable<PageModel> GetPagesForTabs()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page[@tab='true']");
      return XmlToModel(pages);
    }

    public IEnumerable<PageModel> GetPagesForHomeMenu()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page[@inhome='true']");
      return XmlToModel(pages);
    }

    public PageModel GetPageById(string guid)
    {
      XmlNode page = pagesDoc.DocumentElement.SelectSingleNode("//page[@guid='" + guid + "']");
      if (page == null)
        return null;

      return XmlToModel(page);
    }

    public PageModel GetPageByUrlPart(string urlPart)
    {
      XmlNode page = pagesDoc.DocumentElement.SelectSingleNode("//page[urlPart='" + urlPart + "']");
      if (page == null)
        return null;

      return XmlToModel(page);
    }


    public PageModel SavePage(PageModel p)
    {
      XmlElement page = (XmlElement)pagesDoc.SelectSingleNode("//page[@guid='" + p.Guid + "']");

      //check if we should move the page
      string parentpageId = page.Attributes["parentid"]?.Value;
      if (!String.Equals((p.ParentId ?? ""), (parentpageId ?? "")))
      {
        // parent changed, move element
        XmlElement parent = (XmlElement)pagesDoc.SelectSingleNode("//page[@guid='" + p.ParentId + "']");
        if (parent == null)
          parent = (XmlElement)pagesDoc.SelectSingleNode("//page[@id='home']");

        //remove page
        page.ParentNode.RemoveChild(page);

        //append to new parent
        if (parent.Attributes["id"]?.Value == "home")
        {
          pagesDoc.DocumentElement.AppendChild(page);
        }
        else
        {
          XmlNode pages = parent.SelectSingleNode("pages");
          if (pages != null)
            pages.AppendChild(page);
          else
          {
            pages = pagesDoc.CreateElement("pages");
            pages.AppendChild(page);
            parent.AppendChild(pages);
          }
        }
      }

      string html = String.Format("<![CDATA[{0}]]>", p.Content);
   
      page.SelectSingleNode("content").InnerXml = html;

      createOrUpdateElement(page, "metadescription", p.MetaDescription);
      createOrUpdateElement(page, "metatitle", p.MetaTitle);
      createOrUpdateElement(page, "title", p.Title);
      createOrUpdateElement(page, "image", p.Image);
      createOrUpdateElement(page, "sort", p.Sort.ToString());
      createOrUpdateElement(page, "urlPart", p.UrlPart);

      page.SetAttribute("id", p.UrlPart?.XmlSafe() ?? p.MetaTitle.XmlSafe());
      page.SetAttribute("tab", p.InTabMenu ? "true" : "false");
      page.SetAttribute("inhome", p.InHomeMenu ? "true" : "false");
      page.SetAttribute("parentid", p.ParentId);
      page.SetAttribute("published", p.Published.HasValue ? p.Published.Value.ToString() : "");
      page.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      pagesDoc.Save(pageFile);

      return XmlToModel(page);
    }

    private void createOrUpdateElement(XmlElement page, string nodeName, string value)
    {
      if (page.SelectSingleNode(nodeName) == null)
        page.AppendChild(pagesDoc.CreateElement(nodeName));

      page.SelectSingleNode(nodeName).InnerText = value;
    }

    private IEnumerable<PageModel> XmlToModel(XmlNodeList pages, bool deep = false)
    {
      List<PageModel> pageModels = new List<PageModel>();
      foreach (XmlNode n in pages)
        pageModels.Add(XmlToModel(n, deep));
      return pageModels;
    }

    public PageModel newPage()
    {
      var page = CreateNewPage();
      return XmlToModel(page);
    }

    public void deletePage(string guid)
    {
      var page = pagesDoc.SelectSingleNode("//page[@guid='" + guid + "']");
      if (page != null)
      {
        page.ParentNode.RemoveChild(page);
        pagesDoc.Save(pageFile);
      }
    }

    private XmlElement CreateNewPage()
    {
      var guid = Guid.NewGuid().ToString();
      var page = pagesDoc.CreateElement("page");
      page.SetAttribute("guid", guid);
      XmlElement content = pagesDoc.CreateElement("content");
      XmlElement metaDescription = pagesDoc.CreateElement("metadescription");
      XmlElement title = pagesDoc.CreateElement("title");
      content.InnerXml = "";
      metaDescription.InnerText = "";
      title.InnerText = "Nieuwe pagina";
      page.AppendChild(content);
      page.AppendChild(metaDescription);
      page.AppendChild(title);
      page.SetAttribute("id", guid);
      page.SetAttribute("lastmodified", DateTime.Now.ToString("dd-MM-yyyy hh:mm"));

      pagesDoc.DocumentElement.AppendChild(page);
      pagesDoc.Save(pageFile);

      return page;
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
      string parentId = (parent != null && parent != page.OwnerDocument) ? parent.Attributes["guid"].Value : "";
      while (parent != null && parent != page.OwnerDocument)
      {
        url = "/" + parent.Attributes["id"].Value + url;
        parent = parent.SelectSingleNode("../../.");
      }

      string metadescription = null;
      if (page.SelectSingleNode("metadescription") != null)
        metadescription = page.SelectSingleNode("metadescription").InnerText;

      string urlPart = null;
      if (page.SelectSingleNode("urlPart") != null)
        urlPart = page.SelectSingleNode("urlPart").InnerText;

      int sort = 0;
      if (page.SelectSingleNode("sort") != null)
      {
        Int32.TryParse(page.SelectSingleNode("sort").InnerText, out sort);
      }

      string html = page.SelectSingleNode("content") != null ? page.SelectSingleNode("content").InnerXml : "";
      if (html.StartsWith("<![CDATA["))
        html = html.Trim().Substring(9, html.Length - 12);

     
      string lastModifiedString = page.Attributes["lastmodified"]?.Value;
      DateTimeOffset? lastModified = null;
      if (!string.IsNullOrWhiteSpace(lastModifiedString))
      {
        DateTimeOffset lastModifiedParsed;
        if (DateTimeOffset.TryParse(lastModifiedString, out lastModifiedParsed))
        {
          lastModified = lastModifiedParsed;
        }
      }

      string title = page.SelectSingleNode("title")?.InnerText;
      string metaTitle = page.SelectSingleNode("metatitle")?.InnerText;
      string image = page.SelectSingleNode("image")?.InnerText;
      DateTimeOffset? published = null;
      if (DateTimeOffset.TryParse(page.Attributes["published"]?.Value, out DateTimeOffset publishedDate))
        published = publishedDate;

      HtmlDocument hh = new HtmlDocument();
      hh.LoadHtml("<html>" + html + "</html>");
      string text = hh.DocumentNode.InnerText;
      text += title + " " + metaTitle + " " + id + " " + url;

      PageModel p = new PageModel()
      {
        Id = id,
        Guid = Guid.Parse(page.Attributes["guid"].Value),
        Content = html,
        SubPages = subpages,
        Url = url.UrlEncode(),
        UrlPart = urlPart?.UrlEncode(),
        ParentPath = url.Substring(0, url.LastIndexOf('/') + 1),
        ParentId = parentId,
        HasSublingMenu = page.Attributes["haschildmenu"] != null && page.Attributes["haschildmenu"].Value == "true",
        FullText = text,
        InTabMenu = page.Attributes["tab"] != null && page.Attributes["tab"].Value == "true",
        InHomeMenu = page.Attributes["inhome"] != null && page.Attributes["inhome"].Value == "true",
        MetaDescription = metadescription,
        Published = published,
        Sort = sort,
        Title = page.SelectSingleNode("title")?.InnerText,
        MetaTitle = page.SelectSingleNode("metatitle")?.InnerText,
        Image = image,
        LastModified = lastModified
      };

      return p;
    }

    public void updateImages()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      foreach (XmlNode page in pages)
      {
        if (String.IsNullOrWhiteSpace(page.SelectSingleNode("content").InnerXml))
          continue;
        string htmlstring = page.SelectSingleNode("content").InnerXml.Substring(9);
        htmlstring = htmlstring.Substring(0, htmlstring.Length - 3);
        htmlstring = "<html><body>" + htmlstring + "</body></html>";
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlstring);

        var images = doc.DocumentNode.SelectNodes("//img");
        if (images == null || images.Count == 0)
          continue;

        foreach (var img in images)
        {
          var src = img.Attributes["src"].Value;
          if (src.ToLower().Contains("/afbeeldingen/"))
          {
            var width = img.Attributes["width"];
            var height = img.Attributes["height"];
            var newSource = "/image?name=" + src.Substring(14) + "&";
            if (width != null)
            {
              newSource += "width=" + width.Value + "&";
              img.Attributes.Remove(width);
            }
            if (height != null)
            {
              newSource += "height=" + height.Value;
              img.Attributes.Remove(height);
            }
            img.Attributes["src"].Value = newSource;
          }
        }

        string newHtml = doc.DocumentNode.SelectSingleNode("//body").InnerHtml;
        page.SelectSingleNode("content").InnerXml = String.Format("<![CDATA[{0}]]>", newHtml);

      }

      pagesDoc.Save(pageFile);
    }

    internal void AddGuids()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      foreach (XmlElement page in pages)
      {
        page.SetAttribute("guid", Guid.NewGuid().ToString());
      }
      pagesDoc.Save(pageFile);
    }

    public void AddMetaTitles()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      foreach (XmlElement page in pages)
      {
        string title = page.SelectSingleNode("title")?.InnerText;
        if (String.IsNullOrWhiteSpace(title))
        {
          if (page.SelectSingleNode("content/h1") != null)
            title = page.SelectSingleNode("content/h1").InnerText;
          else if (page.SelectSingleNode("content/h2") != null)
            title = page.SelectSingleNode("content/h2").InnerText;
          else
            title = page.Attributes["id"].Value;
        }

        createOrUpdateElement(page, "title", title);
        createOrUpdateElement(page, "metatitle", title);
      }
      pagesDoc.Save(pageFile);
    }

    public void UpdatePublished()
    {
      XmlNodeList pages = pagesDoc.DocumentElement.SelectNodes("//page");
      foreach (XmlElement page in pages)
      {
        // for variation and published date guess, get last modified date
        if (DateTimeOffset.TryParse(page.Attributes["lastmodified"]?.Value, out DateTimeOffset lastMod))
          page.SetAttribute("published", lastMod.AddDays(1).ToString());
        else
        {
          page.SetAttribute("published", DateTimeOffset.UtcNow.AddYears(-2).ToString());
          page.SetAttribute("lastmodified", DateTimeOffset.UtcNow.AddYears(-2).ToString());
        }
      }
      pagesDoc.Save(pageFile);
    }
  }
}