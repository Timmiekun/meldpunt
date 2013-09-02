<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%
    XmlEngine n = new XmlEngine();
    string[] urlParts = n.GetUrlParts();
    n.TransformXml(urlParts);
%>
