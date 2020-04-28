using System;
using System.Collections.Generic;

namespace Meldpunt.Models.ViewModels
{
  public class NavMenu
  {
    public IEnumerable<NavItem> Items { get; set; }

    public Guid CurrentPageId { get; set; }
  }
  public class NavItem
  {
    public string Title { get; set; }
    public string Url { get; set; }
    public Guid Id { get; set; }

    /// <summary>
    /// arbitrary number for sorting
    /// </summary>
    public int Sort { get; set; }
  }
}