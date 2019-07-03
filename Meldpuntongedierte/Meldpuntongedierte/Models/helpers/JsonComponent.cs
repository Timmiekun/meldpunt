using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meldpunt.Models.helpers
{
  public class JsonComponent
  {
    /// <summary>
    /// Component Type. For example String or List<String>
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Actual content of type T
    /// </summary>
    public object Content  { get; set; }

    /// <summary>
    /// Property name. For example: Title or Image
    /// </summary>
    public string Name { get; set; }
  }

}