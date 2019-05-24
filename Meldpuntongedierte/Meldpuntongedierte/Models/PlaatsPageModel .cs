﻿using Lucene.Net.Documents;
using Meldpunt.Services;
using Meldpunt.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Meldpunt.Models
{
  public class PlaatsPageModel : IndexableItem
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string UrlPart { get; set; }
    public string Gemeentenaam { get; set; }
    public DateTimeOffset? Published { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string PhoneNumber { get; set; }
    public string Content { get; set; }

    public DateTimeOffset? LastModified { get; set; }



    /// <summary>
    /// plaatsen die bij de gemeente horen
    /// </summary>
    [NotMapped]
    public List<string> Plaatsen
    {
      get { return _plaatsen; }
      set { _plaatsen = value; }
    }

    private List<String> _plaatsen { get; set; }

    public string PlaatsenAsString
    {
      get {
        if (_plaatsen == null)
          return "";

        return String.Join(",", _plaatsen);
      }
      set
      {
        if (value == null) { _plaatsen = new List<string>(); }
        else { _plaatsen = value.Split(',').ToList(); }
      }
    }

    [NotMapped]
    public string Url { get { return "/ongediertebestrijding-" + UrlPart; } }

    [NotMapped]
    public string FullText
    {
      get
      {
        string contentstring = Meldpunt.Utils.Utils.GetStringFromHTML(Content);

        return string.Join(" ", new { Gemeentenaam, MetaTitle, MetaDescription, PlaatsenAsString, contentstring });
      }
    }



    public Document ToLuceneDocument()
    {
      Document doc = new Document();
      doc.Add(new Field("type", SearchTypes.Place, Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("id", Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("title", Gemeentenaam, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("sortableTitle", Gemeentenaam.XmlSafe(), Field.Store.NO, Field.Index.NOT_ANALYZED));
      doc.Add(new Field("lastModified", DateTools.DateToString(LastModified.Value.UtcDateTime, DateTools.Resolution.SECOND), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("hasplaatsen", (!String.IsNullOrWhiteSpace(PlaatsenAsString)).ToString().ToLower(), Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("text", FullText, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("url", Url, Field.Store.YES, Field.Index.ANALYZED));
      doc.Add(new Field("all", "all", Field.Store.NO, Field.Index.ANALYZED));
      return doc;
    }
  }
}
