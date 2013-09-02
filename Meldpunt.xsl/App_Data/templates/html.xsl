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

  <xsl:template name="html">
    <html>
      <head>
        <title>
          <xsl:if test="$plaats">
            Ongedierte bestrijding in <xsl:value-of select="$plaats"/> ][
          </xsl:if>
          <xsl:text>Meldpunt ongedierte</xsl:text>
        </title>
        <meta http-equiv="Content-Type" lang="nl" content="text/html; charset=utf-8" />
        <meta name="description" lang="nl" >
          <xsl:attribute name="content">
            <xsl:if test="$plaats">
              Ongedierte bestrijding in <xsl:value-of select="$plaats"/> ][
            </xsl:if>
            <xsl:text>Meldpunt ongedierte 0900-2556677</xsl:text>
          </xsl:attribute>
        </meta>
        <meta name="keywords" lang="nl" content="meldpunt ongedierte, wespen, houtworm, muizen, ratten, vlooien, papiervisjes, wespen bestrijding, wespen overlast, wespennest, wespenest, plaag, wespenplaag, vlooienplaag. " />
        <meta name="verify-v1" lang="nl" content="8aAymk/nYCxbP0Y9bqA+9BQWFV1ubxWH6L4ZCHAcoYA=" />
        <meta name="robots" content="index,follow" />

        <link type="text/css" rel="stylesheet" href="/css/reset.css"/>
        <link type="text/css" rel="stylesheet" href="/css/main.css"/>
        <link type="text/css" rel="stylesheet" href="/css/suggest.css"/>
        <script type="text/javascript" src="/js/mootools.js">;</script>
        <script type="text/javascript" src="/js/mootools-more.js">;</script>
        <script type="text/javascript" src="/js/xmlhttp.js">;</script>
        <script type="text/javascript" src="/js/timmie.js">;</script>
        <script type="text/javascript" src="/js/suggest.js">;</script>
		
		<script type="text/javascript">
		 
		  var _gaq = _gaq || [];
		  _gaq.push(['_setAccount', 'UA-24228769-1']);
		  _gaq.push(['_trackPageview']);
		 
		  (function() {
			var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
			ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
			var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
		  })();
		 
		</script>
      </head>
      <body>

        <div id="main" class="main">          
          <div id="header">
            <form method="post" action="/zoek" class="zoekForm">
              <input id="zoek" type="text" name="plaats" onkeyup="suggest.getSuggests(event,this);"/>
              <input type="submit" value="zoek" class="searchSubmit"/>
            </form>
            <span class="searchText">Ongedierte bestrijding in uw woonplaats?</span>
            <div id="suggests" class="suggest-hidden"/>
          </div>
          <div class="content">
            
            <xsl:choose>
              <xsl:when test="$url-count &gt; 0 and not($plaats) and not($page) and not($id='sitemap')">
                <p>
                  Helaas, in de gemeente <xsl:value-of select="$id"/> doen wij geen ongedierte bestrijding
                </p>
              </xsl:when>
              <xsl:when test="$id='sitemap'">
                <ul>
                  <xsl:apply-templates select="$plaatsen//plaats" mode="sitemap"/>
                </ul>
              </xsl:when>
              <xsl:when test="$page">
                <xsl:copy-of select="$page/*"/>
              </xsl:when>
              <xsl:when test="$plaats">
                <b>
                  Welkom op het meldpunt ongedierte voor de gemeente <xsl:value-of select="$plaats"/>
                </b>
                <p>
                  Voor de gemeente <xsl:value-of select="$plaats"/> kan het meldpunt ongedierte de onderstaande diensten verlenen:
                </p>
                <xsl:call-template name="bestrijdingLijst"/>
                <p>Indien een van de bovenstaande diensten overeen komt met uw wens kunt u bellen met:</p>
                <h1>
                  Meldpunt Ongedierte <xsl:value-of select="$plaats"/>: 0900-2556677
                </h1>
                <p>Kosten van bestrijding(en) kunnen ook via dit telefoonnummer opgevraagd worden.</p>
              </xsl:when>
              <xsl:otherwise>
                <h1>
                  Welkom bij Meldpunt Ongedierte. <xsl:value-of select="$plaats"/>
                </h1>
                <p>
                  Hoe werkt dit voor u het snelst?
                  Via de woonplaats check wordt u direct doorgelinkt 
                  naar de betreffende afdeling voor uw gemeente.
                </p>
                <p>
                  <em>Let op!</em><br/>
                  Bij het gebruik maken van dit meldpunt verbindt 
                  het telefoonnummer dat na de woonplaats verschijnt zich direct 
                  met de juiste afdeling voor de gemeente. 
                  Meldingen via dit meldpunt mogen alleen betrekking hebben op 
                  onderstaande soorten bestrijdingen, te weten;
                  <xsl:call-template name="bestrijdingLijst"/>
                </p>
                <p>Kosten van bestrijding(en) kunnen ook via dit telefoonnummer opgevraagd worden.</p>
                <p>Indien het één van bovenstaande acties betreft, kunt u nu uw plaatsnaam invullen.</p>
              </xsl:otherwise>
            </xsl:choose>

          </div>
          <!--<img src="/images/logoMOsmall.gif" class="MOlogo" alt="Logo Meldpunt Ongedierte" title="Logo Meldpunt Ongedierte"/>-->
          <div class="menu">
            <ul>
              <li>
                <a href="/">Home</a>
              </li>             
              <li>
                <a href="/werkwijze">Werkwijze</a>
              </li>
              <li>
                <a href="/contact">Contact</a>
              </li>
            </ul>
          </div>
          <div class="footer">
            <p class="disclaimer">
              0900-2556677
              <a href="/sitemap">sitemap</a>
            </p>
          </div>
        </div>

        <div class="topBar">
          <h2>
            <a href="/">Meldpunt Ongedierte</a>
          </h2>
          <div class="shadow"/>
        </div>


        <!--Google analytics-->
        <script type="text/javascript">
          var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
          document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
        </script>
        <script type="text/javascript">
          try {
          var pageTracker = _gat._getTracker("UA-2150493-3");
          pageTracker._trackPageview();
          } catch(err) {}
        </script>
      </body>

    </html>
  </xsl:template>

  <xsl:template name="bestrijdingLijst">
    <ul class="defaultList">
      <li id="mieren">
        <h3>Mierenbestrijding</h3>
        <div class="description">
          Mieren kunnen bestreden worden met een insecticide d.m.v. lage druktoepassing. De bestrijdingsdeskundige brengt dit middel in de door hem gevonden nestingangen, waardoor de kolonie in haar geheel geïnfecteerd wordt. Indien de openingen niet gevonden kunnen worden, kan er behandeld worden met een lokaasgel. Deze gel wordt uit een kitpatroon gedoseerd.
          Er zijn geen voorzorgsmaatregelen nodig bij een mierenbestrijding.
        </div>
      </li>
      <li id="wespen">
        <h3>Wespenbestrijding</h3>
        <div class="description">
          Wespen kunnen bestreden worden met een insecticide d.m.v. lage druktoepassing. De bestrijdingsdeskundige brengt dit middel in de door hem gevonden nestingang, waardoor de kolonie in haar geheel geïnfecteerd wordt. Er zijn geen voorzorgsmaatregelen nodig bij een wespenbestrijding.
        </div>
      </li>
      <li id="muizen">
        <h3>Muizenbestrijding</h3>
        <div class="description">
         Muizen kunnen op diverse manieren bestreden worden. 
         Om de beste methode te bepalen zal de bestrijdingsdeskundige eerst 
         een inspectie uitvoeren en de situatie in kaart brengen. 
         Vervolgens kan de bestrijding in gang worden gezet.
         Daarna worden er eventuele maatregelen besproken die kunnen voorkomen 
         dat er in de toekomst opnieuw sprake is van een muizenplaag.  
        </div>
      </li>
      <li id="ratten">
        <h3>Rattenbestrijding</h3>
        <div class="description">
          Ratten kunnen op diverse manieren bestreden worden.
          Om de beste methode te bepalen zal de bestrijdingsdeskundige eerst
          een inspectie uitvoeren en de situatie in kaart brengen.
          Vervolgens kan de bestrijding in gang worden gezet.
          Daarna worden er eventuele maatregelen besproken die kunnen voorkomen
          dat er in de toekomst opnieuw sprake is van een rattenplaag.
        </div>
      </li>
      <li id="zilvervisjes">
        <h3>Zilver- en papiervisjesbestrijding</h3>
        <div class="description">
          Zilver- en of Papiervisjes kunnen bestreden worden met een insecticide d.m.v. lage 
          druktoepassing. De bestrijdingsdeskundige brengt dit middel in alle naden en kieren. 
          U dient er zelf zorg voor te dragen dat alle kasten leeg geruimd worden, zodat vooral 
          daar in de kieren langs legplanken e.d. een residu van het middel achter kan blijven. 
          Tijdens de actie en twee uren aansluitend kunt u en de huisdieren de woning niet betreden.
         
        </div>
      </li>
      <li id="kakkerlakken">
        <h3>Kakkerlakkenbestrijding</h3>
        <div class="description">
          Kakkerlakken kunnen bestreden worden met een insecticide d.m.v. een lokaasgel. De bestrijdingsdeskundige brengt dit middel in de door hem gevonden strategische plaatsen, waardoor de kolonie in haar geheel geïnfecteerd wordt. Er zijn geen voorzorgsmaatregelen nodig bij een kakkerlakkenbestrijding.
        </div>
      </li>
    </ul>
  </xsl:template>

  <xsl:template match="tr">
    <xsl:apply-templates select="td[1]" />
  </xsl:template>

  <xsl:template match="td">
    <plaats>
      <xsl:value-of select="."/>
    </plaats>
  </xsl:template>

</xsl:stylesheet>
