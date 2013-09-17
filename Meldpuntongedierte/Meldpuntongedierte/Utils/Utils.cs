using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Meldpunt.Utils
{
  public static class Utils
  {
    public static string UrlEncode(string s)
    {
      return Regex.Replace(s, @"\s", "-").ToLower();
    }

    public static string TruncateAtWord(this string value, int length)
    {
      if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
        return value;

      return value.Substring(0, value.IndexOf(" ", length));
    }
  }
}