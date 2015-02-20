﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace geopunt4Arcgis
{
    public partial class AboutGeopunt4arcgisForm : Form
    {
        string htmlLocation;
        geopunt4arcgisExtension gpExtension;

        public AboutGeopunt4arcgisForm()
        {
            gpExtension = geopunt4arcgisExtension.getGeopuntExtension();

            InitializeComponent();
            this.Text = String.Format("About {0}", ThisAddIn.Name);
            this.labelProductName.Text = String.Format("Naam: {0}", ThisAddIn.Name); 
            this.labelVersion.Text = String.Format("Versie: {0} van {1}", ThisAddIn.Version, ThisAddIn.Date);
            this.labelAuthor.Text = String.Format("Auteur: {0}", ThisAddIn.Author);
            this.labelCompanyName.Text = String.Format("In opdracht van: {0}", ThisAddIn.Company);
            string rootFolder = Path.GetDirectoryName(this.GetType().Assembly.Location);
            htmlLocation = Path.Combine( rootFolder, "Resources/about.html");

            descriptionWebBox.DocumentText = getHtml();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void logoPictureBox_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.geopunt.be/nl/voor-experts/geopunt-plug-ins");
        }

        protected override void OnClosed(EventArgs e)
        {
            gpExtension.aboutDlg = null;
            base.OnClosed(e);
        }

        public string getHtml() 
        {
            string html = @"<!DOCTYPE html>
<html>
<head>
  <title>Over Geopunt4ArcGIS</title>
  <style>body {font-size: 80%}</style>
</head>
<body>
<h1>Geopunt4Arcgis</h1>

<h2>Doelstelling:</h2>

<p>'Geopunt voor ArcGIS' is een add-in voor ESRI ArcGIS desktop, die de webservices van het Vlaamse geoportaal Geopunt ontsluit naar desktop ArcGIS-gebruikers.</p>

<p>Het Vlaamse Geoportaal Geopunt bied een aantal geografische diensten (web-services) aan die mogen gebruikt worden door derden zoals andere overheden en bedrijven. Deze tool implementeert de achterliggende diensten in een arcgis deskop addin.</p>

<p>De webservices, die zijn gebaseerd op de OGC standaard WMS en kunnen gemakkelijk worden toegevoegd aan ArcGIS desktop. GIS-gebruikers kunnen deze diensten ontdekken via het metadatacenter. De achterliggende zoekservice voor deze diensten is niet direct bruikbaar in ArcGIS en wordt in deze plug-in ingebouwd.
Sommige diensten aangeboden door Geopunt zijn niet gebaseerd op een open standaard omdat het gaat om diensten die geen open standaard hebben. Deze publieke webservices zijn opgesteld volgens een REST-volle API, die eenvoudiger in gebruik is voor programmeurs dan OGC-diensten, maar omdat ze niet gestandaardiseerd zijn kunnen ze niet zomaar binnen getrokken worden in desktop software.</p>

Het gaat onder andere over:
<ul>
    <li>Geocoderen, gebaseerd op de officiële CRAB adressen-databank.</li>
    <li>Locaties zoeken, door koppeling van adressen aan de CRAB-databank, bijvoorbeeld de scholendatabank van de Vlaamse overheid. (documentatie nog niet beschikbaar).</li>
    <li>Innames van openbaar domein, van het Generiek Informatieplatform Openbaar Domein (GIPOD) GIPOD, de officiële databank met manifestaties, wegenwerken en andere obstructies op het openbaar domein.</li>
    <li>Hoogteprofiel, een dienst waarmee de hoogte, in digitaal hoogtemodel Vlaanderen, langsheen een lijn kan worden opgevraagd.</li>
    <li>Metadata zoekdienst, deze diensten worden gebruik in het metadatacenter van Geopunt en bevat ondermeer metadatafiches van AGIV, het samenwerkingsverband MercatorNet en DOV.</li>
</ul>

<p>Om GIS-gebruikers binnen en buiten de Vlaamse Overheid dezelfde functionaliteit ter beschikking te stellen als aangeboden in Geopunt, wenst AGIV deze gebruikers te voorzien van software plug-ins die deze functionaliteit geïntegreerd aanbieden binnen de meest gangbare GIS-desktop omgevingen. </p>

<h2>Systeemvereisten</h2>

<ul>
    <li>Minimaal: ArcGIS Destkop versie 10.0 sp4 of hoger, Gewenst: ArcGIS Destkop versie 10.2 of hoger</li>
    <li>Het .net-framework versie 3.5 voor ArcGIS 10.0, .net-framework versie 4.0 voor alle andere versies. (normaal gezien samen met arcgis geïnstalleerd) </li>
    <li>Een verbinding met het Internet.</li>
</ul>

<h2>Functionaliteit</h2>

<p><strong>Zoeken op  adres (Geocoding)</strong> 
Op basis van een input string wordt gezocht naar een adres in CRAB. Je kiest de gemeente uit een selectielijst en geeft een adres op als input tekst. De input tekst bevat verplicht een straatnaam en optioneel een huisnummer. 
Als output krijg je een lijst van strings in de vorm (straatnaam huisnummer, gemeente) die voldoen aan de selectie criteria. Het maximaal aantal elementen in de lijst is 12. Het maximaal aantal elementen in de lijst is 25. Je kan het gewenste adres selecteren door erop te klikken, de locatie van het adres licht dan even op. Je kan een geselecteerd adres ook  toevoegen  als graphic of Feature Class of zoomen naar het adres. </p>

<p><strong>Een adres Identificeren op een lokatie op de kaart (Reverse geocoding)</strong> 
Met deze tool kan je op een locatie in Vlaanderen op de kaart klikken om het correcte adres in CRAB te weten te komen. Het gevonden adres verschijnt in een popup. Je kan de locatie van dit adres toevoegen als graphic of Feature Class.</p>

<p><strong>Zoeken naar interessepunten</strong>
Met deze tool kan je de Geopunt POI-service doorzoeken en de resultaten opslaan als een Feature Class of graphic.</p>

<p><strong>GIPOD</strong>
Met deze kan zoeken op innames van openbaar domein, van het Generiek Informatieplatform Openbaar Domein (GIPOD) GIPOD, de officiële databank met manifestaties, wegenwerken en andere obstructies op het openbaar domein.
Je kunt deze gegevens opslaan als Feature Class.</p>

<p><strong>CSV-bestanden geocoderen</strong>
Met deze tool kan je een CSV-bestand geocoderen, omzetten naar een kaartlaag. Een CSV-bestand (Comma Seperated Values) is een tekstbestand waarin de waarden door een teken (de separator) meestal een komma of puntkomma gescheiden zijn. Als je CSV-bestand adresgegevens bevat, kan je die met deze tool op de kaart weergeven en opslaan als Feature CLass met hun correct CRAB-adres.</p>

<h2>Gerelateerde links</h2>

<ul>
<li><a href='http://www.geopunt.be/over-geopunt/' target='_blank'>Over Geopunt</a></li>
<li><a href='http://kgis.be/pages/over-mij.html' target='_blank'>Over de auteur</a></li>
<li><a href='http://resources.arcgis.com/en/help/main/10.1/index.html#//014p0000001m000000' target='_blank'>Add-ins voor Arcgis</a></li>
<li><a href='https://github.com/warrieka/geopunt4arcgis/' target='_blank'>Broncode</a></li>
</ul>
</body>
</html>";

            return html;
        }

    }
}
