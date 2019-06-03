using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Meldpunt.Utils
{
  public class LocationUtils
  {
    public static Dictionary<string, HashSet<string>> placesByMunicipality;
    
    public static List<String> allPlaces;

    static LocationUtils()
    {
      // https://statline.cbs.nl/Statweb/selection/?DM=SLNL&PA=84489NED&VW=T
      String file = HostingEnvironment.MapPath("~/App_Data/Woonplaatsen_in_Nede_20190520.csv");

      // load places by municipality
      placesByMunicipality = new Dictionary<string, HashSet<string>>();
      allPlaces = new List<string>();
      using (TextReader tr = new StreamReader(file, Encoding.Default))
      {
        // dien eerste twee regels hebben we nie nodig
        tr.ReadLine();
        tr.ReadLine();
        tr.ReadLine();
        tr.ReadLine();
        string line;
        while (!String.IsNullOrWhiteSpace((line = tr.ReadLine())))
        {
          string[] values = line.Split(';');

          if (values.Length <= 1)
          {
            continue;
          }
          // "Onderwerpen";"Woonplaatscode";"Gemeente";"Gemeente";"Provincie";"Provincie";"Landsdeel";"Landsdeel"
          string plaats = values[0].Trim('"');
          string gemeente = values[2].Trim('"');
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
      query = query.XmlSafe();
      bool isMunicipality = placesByMunicipality.Any(g=> g.Key.XmlSafe().Equals(query));
      bool isPlace = placesByMunicipality.Any(m => m.Value.Any(p => p.XmlSafe().Equals(query, StringComparison.CurrentCultureIgnoreCase)));

      return isMunicipality || isPlace;
    }
  }
}