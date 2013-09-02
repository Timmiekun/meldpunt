using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Drawing;

/// <summary>
/// Summary description for timmie
/// </summary>
namespace timmie
{
    public class timmie
    {
        public string nameSpacePrefix { get { return "timmie"; } }
        public string nameSpaceURI { get { return "http://timmie-europe.nl/timmie"; } }

        private HttpRequest request = HttpContext.Current.Request;

        public timmie()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string ToLower(string s)
        {
            return s.ToLower();
        }

        public string ToUpper(string s)
        {
            return s.ToUpper();
        }

        public string UrlEncode(string s)
        {
            return ToLower(Regex.Replace(s, @"\s", "-"));
        }

        public void SetContentType(string type)
        {
            HttpContext.Current.Response.ContentType = type;
        }

        public void SetStatusCode(int type)
        {
            HttpContext.Current.Response.StatusCode = type;
        }

        public string GetHeaderType()
        {
            return HttpContext.Current.Response.StatusCode.ToString();
        }

        public void SetSession(string name, string value)
        {
            HttpContext.Current.Session.Add(name, value);
        }

        public string GetSession(string name)
        {
            return HttpContext.Current.Session[name] != null ? HttpContext.Current.Session[name].ToString() : "";
        }

        public void PageRedirect(string location)
        {
            HttpContext.Current.Response.Redirect(location);
        }

        public string getNewId(XmlDocument doc)
        {
            XmlNodeList ids = doc.SelectNodes("//@id");
            int id = 0;
            foreach (XmlNode i in ids)
            {
                int idInt = Convert.ToInt32(i.Value);
                if (idInt > id)
                    id = idInt;
            }
            return (id + 1).ToString();
        }

        public void zoekPlaats()
        {
            string plaats = urlEncode(request.Form["plaats"]);
            PageRedirect("/" + plaats);            
        }

        public string urlEncode(string s)
        {
            return HttpContext.Current.Server.UrlEncode(s);
        }

        public string forPlaatsSearch(string s)
        {
            return xmlSafeValue(s).ToLower();
        }

        public string xmlSafeValue(string s)
        {
            return Regex.Replace(s, "\\W+", "-");
        }
    }
}
