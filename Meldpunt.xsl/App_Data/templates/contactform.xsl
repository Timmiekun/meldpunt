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

  <xsl:template match="contactform">    
    <xsl:if test="$context/get/tnx = 'tnx'">
      <div class="message">Bedankt voor je mailtje!</div>
    </xsl:if>
    <xsl:if test="$context/get/error = 'error'">
      <div class="message">Vul a.u.b. een geldig email adres in.</div>
    </xsl:if>
    <form id="mailform" name="mailform" method="post" action="/sendmail.aspx" onsubmit="return validate(this)">
      <dl class="text contactform" >
        <dt>Je naam:</dt>
        <dd>
          <input name="naam" type="text" class="formfield" id="naam" />
        </dd>
        <dt>E-mail:</dt>
        <dd>
          <input name="email" type="text" class="formfield" id="email" />
        </dd>
        <dt>Telefoon:</dt>
        <dd>
          <input name="telefoon" type="text" class="formfield" id="telefoon" />
        </dd>
        <dt>Je vraag of opmerking:</dt>
        <dd>
          <textarea rows="5" cols="30" name="opmerking" class="formfieldmulti" id="opmerking" ></textarea>
        </dd>
      </dl>
      <input type="submit" name="Submit" value="Verzenden" class="submitForm"/>
    </form>
  </xsl:template>


</xsl:stylesheet>