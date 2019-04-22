<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"/>
    <xsl:strip-space elements="*" />
    
    <xsl:template match="/">
      <campaign version="PF" auto_indent="YES">
        <xsl:apply-templates/>
      </campaign>
    </xsl:template>
  
    <xsl:template match="pc">
      <pc>
        <xsl:apply-templates select="character"/>
      </pc>
    </xsl:template>
  
    <xsl:template match="npc">
      <npc>
        <xsl:apply-templates select="character"/>
      </npc>
    </xsl:template>

    <xsl:template match="character">
        <name><xsl:value-of select='name'/></name>
        <raceClass><xsl:value-of select='raceClass'/></raceClass>
        <alignment><xsl:value-of select='alignment'/></alignment>
        <size><xsl:value-of select='size'/></size>
        <type><xsl:value-of select='type'/></type>
        <ac><xsl:value-of select='ac'/></ac>
        <touch><xsl:value-of select='touch'/></touch>
        <flat><xsl:value-of select='flat'/></flat>
        <armor><xsl:value-of select='armor'/></armor>
        <hp><xsl:value-of select='hp'/></hp>
        <fort><xsl:value-of select='fort'/></fort>
        <ref><xsl:value-of select='ref'/></ref>
        <will><xsl:value-of select='will'/></will>
        <speed><xsl:value-of select='speed'/></speed>
        <melee><xsl:value-of select='melee'/></melee>
        <ranged><xsl:value-of select='ranged'/></ranged>
        
        <str><xsl:value-of select='str'/></str>
        <dex><xsl:value-of select='dex'/></dex>
        <con><xsl:value-of select='con'/></con>
        <int><xsl:value-of select='int'/></int>
        <wis><xsl:value-of select='wis'/></wis>
        <cha><xsl:value-of select='cha'/></cha>

        <init><xsl:value-of select='init'/></init>
        <bab><xsl:value-of select='bab'/></bab>
        <cmb><xsl:value-of select='cmb'/></cmb>
        <cmd><xsl:value-of select='cmd'/></cmd>
        
        <feats><xsl:value-of select='feats'/></feats>
        <skills><xsl:value-of select='skills'/></skills>
        <languages><xsl:value-of select='languages'/></languages>
        
        <xsl:for-each select="action">
          <action>
            <name><xsl:value-of select='actionName'/></name>
            <attack><xsl:value-of select='actionAttack'/></attack>
            <damage><xsl:value-of select='actionDamage'/></damage>
            <critical><xsl:value-of select='actionCritical'/></critical>
          </action>
        </xsl:for-each>
        <xsl:for-each select="spellLike">
          <spell-like><xsl:value-of select='spellLikeValue'/></spell-like>
        </xsl:for-each>
        <xsl:for-each select="spellList">
          <spells><xsl:value-of select='spellListValue'/></spells>
        </xsl:for-each>
        <sq>
          <xsl:value-of select='sq'/>
        </sq>
      </xsl:template>
    
</xsl:stylesheet>
