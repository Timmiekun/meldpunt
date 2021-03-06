﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Meldpunt.Models.Domain;

namespace Meldpunt.Utils
{
  public static class Utils
  {
    public static string UrlEncode(this string s)
    {
      return Regex.Replace(s, @"\s", "-").ToLower();
    }
    
    public static string XmlSafe(this string s)
    {
      String re = Regex.Replace(s, "[^\\p{L}A-Z0-9]+", "-").Trim('-').ToLower();
      return re;
    }

    public static string TruncateAtWord(this String value, int length)
    {
      if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
        return value;

      return value.Substring(0, value.IndexOf(" ", length));
    }

    public static string Capitalize(this string s)
    {
      if (s.Length > 1)
        return s.Substring(0, 1).ToUpper() + s.Substring(1);
      else
        return s.ToUpper();
    }

    public static IEnumerable<IEnumerable<ContentPageModel>> Split(IEnumerable<ContentPageModel> source)
    {
      int numOfItemsPerList = (int)Math.Ceiling(source.Count() / 3f);
      return source
          .Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / numOfItemsPerList)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }

    public static string GetStringFromHTML(string html)
    {
      HtmlDocument hh = new HtmlDocument();
      hh.LoadHtml("<html>" + html + "</html>");
      return hh.DocumentNode.InnerText;
    }
  }
}