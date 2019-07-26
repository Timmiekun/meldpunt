namespace Meldpunt.Models.Domain
{
  public class RouteableItem
  {
    public string RouteName { get; set;  }
    public string Url { get; set; }
    public string Controller { get; set; }
    public string Action { get; set; }
  }
}
