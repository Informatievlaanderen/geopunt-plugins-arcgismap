﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Catalog;
using ZedGraph;

namespace geopunt4Arcgis
{
    public partial class elevationForm : Form
    {
        IActiveView view;
        IMap map;
        ISpatialReference lam72;
        IFeatureClass pointsFC;
        IFeatureClass lineFC;
        IPolyline polyLineLam72 = null;
        IElement lineGrapic;
        IElement pntGrapic;

        dataHandler.dhm dhm;    

        ESRI.ArcGIS.Framework.ICommandItem oldCmd = null;
        ESRI.ArcGIS.Framework.ICommandItem mouseCmd = null;

        geopunt4arcgisExtension gpExtension;
        List<List<double>> profileData;
        LineItem grpCurve;
        LineItem userClickCurve;
        PointPairList profileLine;
        PointPairList userClickrList;
        TextObj hlabel;
        double maxH;
        double minH;
        double maxD;

        public elevationForm()
        {
            //set global objects
            view = ArcMap.Document.ActiveView;
            map = view.FocusMap;

            lam72 = geopuntHelper.lam72;

            gpExtension = geopunt4arcgisExtension.getGeopuntExtension();
            lineFC = gpExtension.profileLineLayer;
            pointsFC = gpExtension.profilePointsLayer;
            dhm = new dataHandler.dhm(gpExtension.timeout);

            ESRI.ArcGIS.Framework.ICommandBars commandBars = ArcMap.Application.Document.CommandBars;
            UID toolID = new UIDClass();
            toolID.Value = ThisAddIn.IDs.geopuntElevationTool;
            mouseCmd = commandBars.Find(toolID, false, false);

            InitializeComponent();
            initGui();
        }

        public void initGui()
        {
            //the GraphPane
            profileLine = new PointPairList();
            userClickrList = new PointPairList();

            grpCurve = profileGrp.GraphPane.AddCurve("", profileLine, Color.FromArgb(0,0,0,0), SymbolType.XCross);
            grpCurve.Symbol.Border.Color = Color.Black;
            grpCurve.Line.Fill = new Fill(Color.FromArgb(100, 227, 185, 113));

            profileGrp.GraphPane.Y2Axis.MajorGrid.IsZeroLine = false;
            userClickCurve = profileGrp.GraphPane.AddCurve("", userClickrList, Color.Blue, SymbolType.None);
 
            profileGrp.GraphPane.Legend.IsVisible = false;
            profileGrp.GraphPane.Title.Text = "Profiel Titel";
            profileGrp.GraphPane.XAxis.Title.Text = "Afstand (m)";
            profileGrp.GraphPane.YAxis.Title.Text = "Hoogte (m)";

            hlabel = new TextObj() { ZOrder = ZOrder.A_InFront };
            profileGrp.GraphPane.GraphObjList.Add(hlabel);

            symbolBtn.SelectedIndex = 0;
        }

        public void getData(IPolyline profileLineGeom)
        {
            try
            {
                polyLineLam72 = (IPolyline)geopuntHelper.Transform((IGeometry)profileLineGeom, lam72);

                int samplesCount = (int)samplesNum.Value;
                datacontract.geojsonLine gjs = geopuntHelper.esri2geojsonLine(polyLineLam72);
                profileData = dhm.getDataAlongLine(gjs, samplesCount, dataHandler.CRS.Lambert72);

                ArcMap.Application.CurrentTool = oldCmd;

                this.WindowState = FormWindowState.Normal;
                this.Focus();

                maxH = profileData.Select(c => c[3]).Max();
                minH = profileData.Where(c => c[3] > -999).Select(c => c[3]).Min();
                maxD = profileData.Select(c => c[0]).Max();
                profileGrp.GraphPane.YAxis.Scale.Max = maxH;
                profileGrp.GraphPane.YAxis.Scale.Min = minH;
                profileGrp.GraphPane.XAxis.Scale.Max = maxD;

                addLineGrapic();
                createGraph();
            }
            catch (WebException wex)
            {
                if (wex.Status == WebExceptionStatus.Timeout)
                    MessageBox.Show("De connectie werd afgebroken." +
                        " Het duurde te lang voor de server een resultaat terug gaf.\n" +
                        "U kunt via de instellingen de 'timout'-tijd optrekken.", wex.Message);
                else if (wex.Response != null)
                {
                    string resp = new StreamReader(wex.Response.GetResponseStream()).ReadToEnd();
                    MessageBox.Show(resp, wex.Message);
                }
                else
                    MessageBox.Show(wex.Message, "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +" "+ ex.StackTrace, "Error");
            }

        }

        #region "overrides"
        protected override void OnClosed(EventArgs e)
        {
            if (ArcMap.Application.CurrentTool == mouseCmd)
            {
                ArcMap.Application.CurrentTool = oldCmd;
            }
            clearGraphics();
            view.Refresh();
            base.OnClosed(e);
        }
        #endregion

        #region "event handlers"
        private void drawbtn_Click(object sender, EventArgs e)
        {
            if (mouseCmd == null)  return;

            this.WindowState = FormWindowState.Minimized;

            oldCmd = ArcMap.Application.CurrentTool;
            ArcMap.Application.CurrentTool = mouseCmd;
        }

        private void profileGrp_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                int index = 0;
                CurveItem nearestobject = null;
                PointF clickedPoint = new PointF(e.X, e.Y);
                profileGrp.GraphPane.FindNearestPoint(clickedPoint, grpCurve, out nearestobject, out index);

                if (nearestobject == null)
                {
                    userClickCurve.IsVisible = false;
                    hlabel.IsVisible = false;
                    profileGrp.Refresh();
                    if (pntGrapic != null)
                    {
                        IGraphicsContainer graphicsContainer = (IGraphicsContainer)map;
                        graphicsContainer.DeleteElement(pntGrapic);
                        pntGrapic = null;
                        view.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    }
                    msgLbl.Text = "";
                    return;
                }
                double x = nearestobject.Points[index].X;
                double h = nearestobject.Points[index].Y;

                userClickCurve.IsVisible = true;
                hlabel.IsVisible = true;

                //set the label
                hlabel.Text = h.ToString("f2");
                hlabel.Location.X = x;
                hlabel.Location.Y = h;
                hlabel.FontSpec.Fill = new Fill(Color.Azure);
                //draw the line
                userClickrList.Clear();
                userClickrList.Add(x, h);
                userClickrList.Add(x, profileGrp.GraphPane.YAxis.Scale.Min);

                interpolateLineAndAddPoint(x);

                profileGrp.Refresh();

                msgLbl.Text = string.Format("Afstand: {0:0.00} m, Hoogte: {1:0.00} m", x, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : " + ex.StackTrace);
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            if (polyLineLam72 == null) return;
            this.getData(polyLineLam72);
            profileGrp.Refresh();
        }

        private void helpLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.geopunt.be/voor-experts/geopunt-plug-ins/arcgis%20plugin/functionaliteiten/hoogteprofiel");
        }

        private void savePointsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (profileData == null) return;

                if (pointsFC == null)
                {
                    this.Visible = false;
                    string fcPath = geopuntHelper.ShowSaveDataDialog("Save as ...");
                    this.Visible = true;

                    if (fcPath == null) return;

                    createFeatureClass(fcPath, map.SpatialReference, esriGeometryType.esriGeometryPoint);
                }
                string prfName = profileGrp.GraphPane.Title.Text;

                addValuesToPointsFC(prfName);
                view.Refresh();
                msgLbl.Text = string.Format("{0} is opgeslagen naar {1}", prfName, pointsFC.AliasName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }

        private void saveLineBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (polyLineLam72 == null) return;

                if (lineFC == null)
                {
                    this.Visible = false;
                    string fcPath = geopuntHelper.ShowSaveDataDialog("Save as ...");
                    this.Visible = true;

                    if (fcPath == null) return;

                    createFeatureClass(fcPath, map.SpatialReference, esriGeometryType.esriGeometryPolyline);
                }
                string prfName = profileGrp.GraphPane.Title.Text;

                addValuesToLineFC(prfName);
                view.Refresh();
                msgLbl.Text = string.Format("{0} is opgeslagen naar {1}", prfName, lineFC.AliasName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + ex.StackTrace);
            }
        }
        //toolbar
        private void setTitleBtn_Click(object sender, EventArgs e)
        {
            string title = inputForm.showInputDlg("Geef de titel van dit profiel op:", this);
            if (title == null) return;
            profileGrp.GraphPane.Title.Text = title;
            profileGrp.Refresh();
        }

        private void savePrfAsImageBtn_Click(object sender, EventArgs e)
        {
            string prfPicName = profileGrp.GraphPane.Title.Text + ".png";
            profileGrp.SaveAs(prfPicName);
        }

        private void unZoomBtn_Click(object sender, EventArgs e)
        {
            profileGrp.ZoomOutAll(profileGrp.GraphPane);
        }

        private void zoomRectActivateBtn_Click(object sender, EventArgs e)
        {
            profileGrp.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            profileGrp.ZoomModifierKeys = Keys.None;
            profileGrp.PanButtons = System.Windows.Forms.MouseButtons.Left;
            profileGrp.PanModifierKeys = Keys.Control;
        }

        private void PanActivateBtn_Click(object sender, EventArgs e)
        {
            profileGrp.PanButtons = System.Windows.Forms.MouseButtons.Left;
            profileGrp.PanModifierKeys = Keys.None;
            profileGrp.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            profileGrp.ZoomModifierKeys = Keys.Control;
        }

        private void prevZoomBtn_Click(object sender, EventArgs e)
        {
            profileGrp.ZoomOut(profileGrp.GraphPane);
        }

        private void fillBtn_Click(object sender, EventArgs e)
        {
            Color clr = grpCurve.Line.Fill.Color;
            ColorDialog cDlg = new ColorDialog();
            cDlg.Color = Color.FromArgb(255, clr);
            cDlg.ShowDialog(this);
            grpCurve.Line.Fill.Color = Color.FromArgb(100, cDlg.Color.R, cDlg.Color.G, cDlg.Color.B);
        }

        private void symbolBtn_Click(object sender, EventArgs e)
        {
            switch (symbolBtn.Text)
            {
                case "Diamant":
                    grpCurve.Symbol.Type = SymbolType.Diamond;
                    profileGrp.Refresh();
                    break;
                case "Kruisje":
                    grpCurve.Symbol.Type = SymbolType.XCross;
                    profileGrp.Refresh();
                    break;
                case "Cirkel":
                    grpCurve.Symbol.Type = SymbolType.Circle;
                    profileGrp.Refresh();
                    break;
                case "Driehoek":
                    grpCurve.Symbol.Type = SymbolType.Triangle;
                    profileGrp.Refresh();
                    break;
                case "Geen symbool":
                    grpCurve.Symbol.Type = SymbolType.None;
                    profileGrp.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void addDhmBtn_Click(object sender, EventArgs e)
        {
           geopuntHelper.addWMS2map(view.FocusMap,
               "https://geo.api.vlaanderen.be/dhmv/wms?request=getcapabilities&version=1.3.0&service=wms?", 40, "DHMVII_DTM_1m");
        }
        #endregion

        #region "private functions"

        private void interpolateLineAndAddPoint(double dist)
        {
            if (lineGrapic == null) return;
            try
            {
                IPolyline4 geom = (IPolyline4) polyLineLam72;
                IPoint pnt = new PointClass();
                geom.QueryPoint(esriSegmentExtension.esriNoExtension, dist, false, pnt);
                //check if null
                if (pnt == null) return;

                IGeometry mapPnt = geopuntHelper.Transform((IGeometry)pnt, map.SpatialReference);

                IRgbColor rgb = new RgbColorClass() { Blue= 255 };
                IRgbColor black = new RgbColorClass() { Blue= 0, Green= 0, Red= 0 };
                if (pntGrapic != null)
                {
                    IGraphicsContainer graphicsContainer = (IGraphicsContainer)map;
                    graphicsContainer.DeleteElement(pntGrapic);
                    pntGrapic = null;
                }
                pntGrapic = geopuntHelper.AddGraphicToMap(map, mapPnt, rgb, black, 6, true);
                view.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message + " : " + ex.StackTrace );
            }

        }
        
        private void addLineGrapic()
        {
            try
            {
                clearGraphics();
                IGeometry lineLam72 = (IGeometry)polyLineLam72;
                IGeometry mapLine = geopuntHelper.Transform(lineLam72, map.SpatialReference);

                IRgbColor rgb = new RgbColorClass() { Red = 255 };
                lineGrapic = (IElement)geopuntHelper.AddGraphicToMap(view.FocusMap, mapLine, rgb, rgb, 2, true);
                view.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message + " : " + ex.StackTrace );
            }
        }

        private void createGraph()
        {
            if (profileData == null) return;

            // Set the Title
            gpExtension.profileCounter += 1;
            profileGrp.GraphPane.Title.Text = string.Format("Hoogte Profiel {0}", gpExtension.profileCounter);

            // Make the data array
            profileLine.Clear();
            foreach (List<double> rec in profileData)
            {
                double x = rec[0];
                double h = rec[3];
                profileLine.Add(x, h);
            }
            // Tell the graph to refigure the axes since the data have changed
            profileGrp.AxisChange();
        }

        private void clearGraphics()
        {
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)map;

            if (lineGrapic == null && pntGrapic == null) return;
            try
            {
                if (lineGrapic != null)
                {
                    graphicsContainer.DeleteElement(lineGrapic);
                    lineGrapic = null;
                }
                if (pntGrapic != null)
                {
                    graphicsContainer.DeleteElement(pntGrapic);
                    pntGrapic = null;
                }
            }
            catch (Exception)
            {
                Console.Write("Element was already deleted");
            }
        }

        private void createFeatureClass(string fcPath, ISpatialReference srs, esriGeometryType geomType)
        {
            List<IField> attr = new List<IField>();
            attr.Add(geopuntHelper.createField("naam", esriFieldType.esriFieldTypeString, 255));
            if (geomType == esriGeometryType.esriGeometryPoint)
            {
                attr.Add(geopuntHelper.createField("Afstand", esriFieldType.esriFieldTypeDouble));
                attr.Add(geopuntHelper.createField("Hoogte", esriFieldType.esriFieldTypeDouble));
            }
            FileInfo fcInfo = new FileInfo(fcPath);

            IFeatureClass lyr = null;
            if (fcInfo.Extension == ".shp")
            {
                lyr = geopuntHelper.createShapeFile(fcInfo.FullName, attr, map.SpatialReference, geomType, true);
            }
            else if (fcInfo.DirectoryName.ToLowerInvariant().EndsWith(".gdb"))
            {
                lyr = geopuntHelper.createFeatureClass(fcInfo.DirectoryName, fcInfo.Name, attr, srs, geomType, true);
            }
            else
            {
                throw new Exception("Bestand is geen shapefile of FileGeodatabase Feature Class");
            }

            if (geomType == esriGeometryType.esriGeometryPolyline)
            {
                lineFC = lyr;
                gpExtension.profileLineLayer = lineFC;
                geopuntHelper.addFeatureClassToMap(view, lineFC);
            }
            else if (geomType == esriGeometryType.esriGeometryPoint)
            {
                pointsFC = lyr;
                gpExtension.profilePointsLayer = pointsFC;
                geopuntHelper.addFeatureClassToMap(view, pointsFC);
            } 
        }

        private void addValuesToLineFC(string prfName)
        {
            if (profileData == null || lineFC == null) return;

            IPolyline geom = geopuntHelper.Transform((IGeometry)polyLineLam72, map.SpatialReference ) as IPolyline;

            IFeature feature = lineFC.CreateFeature();
            feature.Shape = (IGeometry) geom;

            if (prfName.Length > 255) prfName = prfName.Substring(0, 255);

            int naamIdx = lineFC.FindField("naam");
            feature.set_Value(naamIdx, prfName);
            feature.Store();
        }

        private void addValuesToPointsFC(string prfName)
        {
            using (ComReleaser comReleaser = new ComReleaser())
            {
                if( pointsFC == null || profileData == null) return;

                List<List<double>> data = profileData.Where(c => c[3] > -999).ToList();

                //Spatialreference 
                ISpatialReference srs = view.FocusMap.SpatialReference;
                // Create a feature buffer.
                IFeatureBuffer featureBuffer = pointsFC.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featureBuffer);

                // Create an insert cursor.
                IFeatureCursor insertCursor = pointsFC.Insert(true);
                comReleaser.ManageLifetime(insertCursor);

                foreach (List<double> row in data)
                {
                    IPoint fromXY; IPoint toXY;

                    IPolyline4 geom = (IPolyline4)polyLineLam72;
                    fromXY = new PointClass();
                    geom.QueryPoint(esriSegmentExtension.esriNoExtension, row[0], false, fromXY);

                    //reproject the point
                    toXY = geopuntHelper.Transform(fromXY as IGeometry, map.SpatialReference) as IPoint;

                    featureBuffer.Shape = toXY as IGeometry;

                    int naamIdx = pointsFC.FindField("naam");
                    featureBuffer.set_Value(naamIdx, prfName);
                    int AfstandIdx = pointsFC.FindField("Afstand");
                    featureBuffer.set_Value(AfstandIdx, row[0]);
                    int HoogteIdx = pointsFC.FindField("Hoogte");
                    featureBuffer.set_Value(HoogteIdx, row[3]);

                    insertCursor.InsertFeature(featureBuffer);
                }
                insertCursor.Flush();
            }
        }

        #endregion
    }
}
