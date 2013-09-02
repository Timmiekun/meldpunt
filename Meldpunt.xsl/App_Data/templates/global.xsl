<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:timmie="http://timmie-europe.nl/timmie"
  exclude-result-prefixes="xsl timmie">

  <xsl:output method="html"
  doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN"
  doctype-system="http://www.w3.org/TR/html4/loose.dtd"
  encoding="utf-8"
  indent="yes"
  omit-xml-declaration="yes"/>

  <xsl:include href="html.xsl"/> 
  <xsl:include href="sitemap.xsl"/>
  <xsl:include href="contactform.xsl"/>

  <xsl:variable name="id" select="name(context/url-parts/*[last()])"/>
  <xsl:variable name="context" select="/context"/>
  <xsl:variable name="url-count" select="count(context/url-parts/*)"/>
  <xsl:variable name="editMode" select="timmie:GetSession('loggedIn')='true'"/>
  <xsl:variable name="plaatsen" select="document('../content/plaatsen.xml')"/>
  <xsl:variable name="pages" select="document('../content/pages.xml')"/>
  <xsl:variable name="plaats" select="$plaatsen/plaatsen/plaats[timmie:forPlaatsSearch(text())=timmie:forPlaatsSearch($id)]"/>
  <xsl:variable name="page" select="$pages//page[@id=$id]"/>
  
  


  <xsl:template match="context">    
    <xsl:apply-templates select="url"/>    
  </xsl:template>

  <xsl:template match="/context/url">
    <xsl:choose>
      <xsl:when test="$id='zoek'">
        <xsl:apply-templates select="timmie:zoekPlaats()"/>
      </xsl:when>
      <xsl:when test="/context/url/sitemap/xml">
        <xsl:apply-templates select="timmie:SetContentType('text/xml')"/>
        <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
                 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                 xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9
	              http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd">
          <!--<xsl:apply-templates select="$pages//page" mode="sitemapXml"/>-->
          <url>
            <loc>http://www.meldpuntongedierte.nl/sitemap</loc>
            <lastmod>2013-06-01</lastmod>
            <changefreq>yearly </changefreq>
            <priority>0.5</priority>
          </url>
          <xsl:apply-templates select="$plaatsen/plaatsen/plaats" mode="sitemapXml"/>
        </urlset>
      </xsl:when>
      <xsl:otherwise>
        <xsl:call-template name="html"/>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

</xsl:stylesheet>
