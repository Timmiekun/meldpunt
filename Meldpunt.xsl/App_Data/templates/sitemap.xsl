<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:timmie="http://timmie-europe.nl/timmie"
exclude-result-prefixes="xsl timmie">

  <xsl:output method="html"
    doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN"
    doctype-system="http://www.w3.org/TR/html4/loose.dtd"
    encoding="utf-8"
    indent="yes"
    omit-xml-declaration="yes"/>

  <!--<?xml version="1.0"?>-->

  <!--<!DOCTYPE html SYSTEM "http://www.w3.org/TR/html4/loose.dtd" PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
  -<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd">-->

  <xsl:template name="sitemap">
    <div class="content">
      <h1 class="title">Sitemap</h1>
      <ul class="sitemap">
      </ul>
    </div>
  </xsl:template>

  <xsl:template match="plaats" mode="sitemapXml">
    <url>
      <loc>
        http://www.meldpuntongedierte.nl/<xsl:value-of select="timmie:urlEncode(.)"/>
      </loc>
      <lastmod>2009-03-20</lastmod>
      <changefreq>yearly </changefreq>
      <priority>0.5</priority>
    </url>
  </xsl:template>

  <xsl:template match="plaats" mode="sitemap">
    <li>
      <a href="{timmie:urlEncode(.)}">
        <xsl:value-of select="."/>
      </a>
    </li>
  </xsl:template>

</xsl:stylesheet>