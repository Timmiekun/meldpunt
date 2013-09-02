using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Collections.Generic;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        XmlDocument plaatsen = new XmlDocument();
        string startWith = Request.QueryString["plaats"];
        string filename = Server.MapPath("App_data/content/plaatsen.xml");
        plaatsen.Load(filename);

        List<string> plaatsenList = new List<string>();
        foreach (XmlNode plaats in plaatsen.DocumentElement.ChildNodes)
            plaatsenList.Add(plaats.InnerText);

        List<string> foundPlaces = plaatsenList.FindAll(startsWith);
        bool first = true;
        foreach (string plaats in foundPlaces)
        {
            if (first)
            {
                Response.Write("\"" + plaats + "\"");
                first = false;
            }
            else
                Response.Write("," + "\"" + plaats + "\"");
        }
    }

    private bool startsWith(string s)
    {
        string value = Request.QueryString["plaats"].ToLower();
        if (String.IsNullOrEmpty(value))
            return false;
        else
            return s.ToLower().StartsWith(value);
    }
}
