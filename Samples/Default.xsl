<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="utf-8" />
	<xsl:variable name="columnCount" select="count(OperationsResearch/Simplex/Tableaux/Tableau[@index = 0]/Columns/Column)" />
	<xsl:template match="/">
		<html>
<!--
		<head>
			<title>Operations Research - Simplex Algorithm</title>
		</head>
-->		
		<body>
			<div align="center">
				<h2>Simplex Algorithm (<xsl:value-of select="OperationsResearch/Simplex/@mode" />)</h2>
				<table border="0">
				<xsl:for-each select="OperationsResearch/Simplex/Tableaux/Tableau">
					<tr><td colspan="{$columnCount + 4}"><b>Tableau <xsl:value-of select="@index" /></b></td></tr>
					<!-- Row for column names -->
					<tr>
						<td colspan="2"></td>
						<xsl:for-each select="Columns/Column">
							<th><xsl:value-of select="text()" /></th>
						</xsl:for-each>
						<td></td>
						<td>Ratio Test</td>
					</tr>
					<tr><td colspan="{$columnCount + 4}"><hr /></td></tr>
					<xsl:for-each select="Rows/Row">
						<xsl:if test="position() = count(../Row)">
							<tr><td colspan="{$columnCount + 4}"><hr /></td></tr>
						</xsl:if>
						<tr>
							<td><xsl:value-of select="@transform" /></td>
							<th><xsl:value-of select="@function" /></th>
							<xsl:for-each select="Column">
								<td><xsl:value-of select="text()" /><!--<xsl:if test="@n = ../../../Pivot/@n">*</xsl:if>--></td>
							</xsl:for-each>
							<!-- Pivot row and ratio -->
							<xsl:choose>
							<xsl:when test="position() = count(../Row)">
								<td colspan="2"></td>
							</xsl:when>
							<xsl:otherwise>
								<td><xsl:if test="@m = ../../Pivot/@m">*</xsl:if></td>
								<td><xsl:if test="Ratio != 'undefined'"><xsl:value-of select="Ratio" /><!--<xsl:if test="@m = ../../Pivot/@m">*</xsl:if>--></xsl:if></td>
							</xsl:otherwise>
							</xsl:choose>
						</tr>
					</xsl:for-each>
					<!-- Pivot column marker -->
					<tr>
						<td colspan="2"></td>
						<xsl:for-each select="Columns/Column[position() &lt; count(../Column) - 1]">
							<td><xsl:if test="@n = ../../Pivot/@n">*</xsl:if></td>
						</xsl:for-each>
						<td colspan="4"></td>
					</tr>
					<!-- Row for solution -->
					<tr>
						<td>&#32;</td>
						<td><b>u</b>(</td>
						<xsl:for-each select="Solution/Column">
							<xsl:choose>
							<xsl:when test="text() = 'undefined'">
								<td>) =</td>
							</xsl:when>
							<xsl:otherwise>
								<td><xsl:value-of select="text()" /></td>
							</xsl:otherwise>
							</xsl:choose>
						</xsl:for-each>
					</tr>
					<xsl:if test="position() &lt; count(../Tableau)">
						<tr><td colspan="{$columnCount + 4}">.</td></tr>
					</xsl:if>
				</xsl:for-each>
				</table>
			</div>
		</body>
		</html>
	</xsl:template>
</xsl:stylesheet>