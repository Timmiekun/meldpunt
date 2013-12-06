using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;
using System.Text;

namespace Meldpunt.Utils
{
  public class LocationUtils
  {
    private static string _placesByMunicipality = ConfigurationManager.AppSettings["PlacesByMunicipality"];

    public static Dictionary<string, HashSet<string>> placesByMunicipality;
    
    public static List<String> allPlaces;

    static LocationUtils()
    {
      String file = HttpContext.Current.Server.MapPath("~/App_Data/2012_Woonplaatsen_in_Nederland.csv");

      // load places by municipality
      placesByMunicipality = new Dictionary<string, HashSet<string>>();
      allPlaces = new List<string>();
      using (TextReader tr = new StreamReader(file, Encoding.Default))
      {
        // dien eerste twee regels hebben we nie nodig
        tr.ReadLine();
        tr.ReadLine();
        string line;
        while (!String.IsNullOrWhiteSpace((line = tr.ReadLine())))
        {
          string[] values = line.Split(';');
          string plaats = values[0].ToLower().Trim('"');
          string gemeente = values[3].ToLower().Trim('"');
          allPlaces.Add(plaats);

          if (!placesByMunicipality.ContainsKey(gemeente))
          {
            var newList = new HashSet<string>();
            newList.Add(plaats);
            placesByMunicipality.Add(gemeente, newList);
          }
          else
          {
            placesByMunicipality[gemeente].Add(plaats);
          }
        }
      }    
    }

    public static bool IsLocation(string query)
    {
      bool isMunicipality = placesByMunicipality.ContainsKey(query);
      bool isPlace = placesByMunicipality.Any(m => m.Value.Any(p => p.Equals(query, StringComparison.CurrentCultureIgnoreCase)));

      return isMunicipality || isPlace;
    }
  }
}