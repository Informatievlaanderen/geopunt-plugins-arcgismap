﻿using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GISClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace geopunt4Arcgis
{
    static class geopuntHelper
    {
        #region "geometry operations"
        public static ISpatialReference lam72
        {
            get 
            {
                Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
                System.Object obj = Activator.CreateInstance(factoryType);
                ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;
                return spatialReferenceFactory.CreateProjectedCoordinateSystem(31370);
            }
        }

        public static ISpatialReference wgs84
        {
            get
            {
                Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
                System.Object obj = Activator.CreateInstance(factoryType);
                ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;
                return spatialReferenceFactory.CreateGeographicCoordinateSystem(4326);
            }
        }

        /// <summary>
        /// Reproject geometry to wanted crs, optimized for Lam72
        /// </summary>
        /// <param name="geom">input geometry, with a set crs</param>
        /// <param name="toSrs">srs to project too</param>
        /// <returns></returns>
        public static IGeometry Transform(IGeometry geom, ISpatialReference toSrs)
        {
            if (toSrs == null || geom.SpatialReference == null || toSrs.FactoryCode == geom.SpatialReference.FactoryCode)
            {
                return geom;
            }
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;

            if (toSrs.FactoryCode == 31370 && 
                    (geom.SpatialReference.FactoryCode == 3812 ||  //=lam2008
                     geom.SpatialReference.FactoryCode == 4937 ||  //=ETRS89
                     geom.SpatialReference.FactoryCode == 4258 ||  //=ETRS89
                     geom.SpatialReference.FactoryCode == 25831))
            {
                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation(
                   (int)esriSRGeoTransformation2Type.esriSRGeoTransformation_Belge_1972_To_ETRS_1989_1) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(toSrs, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
            }

            else if (toSrs.FactoryCode == 31370 && 
                   (geom.SpatialReference.FactoryCode == 4326   || //= wgs84
                    geom.SpatialReference.FactoryCode == 4258   || //=ETRS89
                    geom.SpatialReference.FactoryCode == 32631  || //= WGS utm 31n
                    geom.SpatialReference.FactoryCode == 3043   || //= ETRS89 utm 31n
                    geom.SpatialReference.FactoryCode == 3857   || //= web mercator
                    geom.SpatialReference.FactoryCode == 102100 || //= 3857
                    geom.SpatialReference.FactoryCode == 900913) ) //= 3857
            {
                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)
                    esriSRGeoTransformation3Type.esriSRGeoTransformation_Belge_1972_To_WGS_1984_2) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(toSrs, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
            }
            else if (geom.SpatialReference.FactoryCode == 31370 && 
                    (toSrs.FactoryCode == 3812 ||  //=lam2008
                     toSrs.FactoryCode == 4937 ||  //=ETRS89
                     toSrs.FactoryCode == 4258 ||  //=ETRS89
                     toSrs.FactoryCode == 25831))  // = utm31n erts89
            {
                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation(
                    (int)esriSRGeoTransformation2Type.esriSRGeoTransformation_Belge_1972_To_ETRS_1989_1) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(toSrs, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
            }

            else if (geom.SpatialReference.FactoryCode == 31370 &&
                   (toSrs.FactoryCode == 4326   || //= wgs84
                    toSrs.FactoryCode == 4258   || //=ETRS89
                    toSrs.FactoryCode == 32631  || //= WGS utm 31n
                    toSrs.FactoryCode == 3043   || //= ETRS89 utm 31n
                    toSrs.FactoryCode == 3857   || //= web mercator
                    toSrs.FactoryCode == 102100 || //= 3857
                    toSrs.FactoryCode == 900913))  //= 3857
            {
                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)
                    esriSRGeoTransformation3Type.esriSRGeoTransformation_Belge_1972_To_WGS_1984_2) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(toSrs, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
            }
            else
            {
                IGeometry geometry = geom; //clone geom
                geometry.Project(toSrs);
                return geometry;
            }
        }

        /// <summary>
        /// Reproject geometry to wanted Lam72
        /// </summary>
        /// <param name="geom">the geometry to project</param>
        /// <returns>the projected geometry</returns>
        public static IGeometry Transform2Lam72(IGeometry geom)
        {
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;
            ISpatialReference lam72 = spatialReferenceFactory.CreateSpatialReference(31370);

            if (geom.SpatialReference.FactoryCode == 31370)
            {
                return geom;
            }

            if (    geom.SpatialReference.FactoryCode == 4326 ||   //= wgs84
                    geom.SpatialReference.FactoryCode == 4258 ||   //=ETRS89
                    geom.SpatialReference.FactoryCode == 32631 ||  //= WGS utm 31n
                    geom.SpatialReference.FactoryCode == 3043 ||   //= ETRS89 utm 31n
                    geom.SpatialReference.FactoryCode == 3857 ||   //= web mercator
                    geom.SpatialReference.FactoryCode == 102100 || //= 3857
                    geom.SpatialReference.FactoryCode == 900913)  //= 3857
            {

                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)
                    esriSRGeoTransformation3Type.esriSRGeoTransformation_Belge_1972_To_WGS_1984_2) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(lam72, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
            }
            else if (geom.SpatialReference.FactoryCode == 3812 ||  //=lam2008
                     geom.SpatialReference.FactoryCode == 4937 ||  //=ETRS89
                     geom.SpatialReference.FactoryCode == 4258 ||  //=ETRS89
                     geom.SpatialReference.FactoryCode == 25831 )  //=UTM31n-ERTS89
	        {
                IGeoTransformation geoTransformation = spatialReferenceFactory.CreateGeoTransformation((int)
                    esriSRGeoTransformation2Type.esriSRGeoTransformation_Belge_1972_To_ETRS_1989_1) as IGeoTransformation;

                IGeometry2 geometry = geom as IGeometry2; //clone geom as IGeometry2
                geometry.ProjectEx(lam72, esriTransformDirection.esriTransformReverse, geoTransformation, false, 0, 0);

                return geometry as IGeometry;
	        }
            else
            {
                IGeometry2 geometry = (IGeometry2) geom; //clone geom
                geometry.Project(lam72);
                return geometry;
            }
        }


        /// <summary>
        /// make arcgis IEnvelope from Xmin|Ymin|Xmax|Ymax
        /// </summary>
        /// <param name="xmin">lowest x-coordinate in given srs</param>
        /// <param name="ymin">lowest y-coordinate in given srs<</param>
        /// <param name="xmax">highest x-coordinate in given srs<</param>
        /// <param name="ymax">highest x-coordinate in given srs<</param>
        /// <param name="srs">the srs of the bbox</param>
        /// <returns></returns>
        public static IEnvelope makeExtend(Double xmin, Double ymin, Double xmax, Double ymax, ISpatialReference srs)
        {
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords(xmin, ymin, xmax, ymax);
            envelope.SpatialReference = srs;
            return envelope;
        }
        #endregion

        #region "Zoom by Ratio and Recenter"

        ///<summary>Zoom in ActiveView using a ratio of the current extent and re-center based upon supplied x,y map coordinates.</summary>
        ///
        ///<param name="activeView">An IActiveView interface.</param>
        ///<param name="zoomRatio">A System.Double that is the ratio to zoom in. Less that 1 zooms in</param>
        ///<param name="xMap">A System.Double that is the x portion of a point in map units to re-center on.</param>
        ///<param name="yMap">A System.Double that is the y portion of a point in map units to re-center on.</param>
        /// 
        ///<remarks>Both the width and height ratio of the zoomed area is preserved.</remarks>
        public static void ZoomByRatioAndRecenter(IActiveView activeView, System.Double zoomRatio, System.Double xMap, System.Double yMap)
        {
            if (activeView == null || zoomRatio < 0)
            {
                return;
            }
            IEnvelope envelope = activeView.Extent;
            IPoint point = new PointClass();
            point.X = xMap;
            point.Y = yMap;
            envelope.CenterAt(point);
            envelope.Expand(zoomRatio, zoomRatio, true);
            activeView.Extent = envelope;
        }
        #endregion

        #region "Add Graphic to Map"
        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IPoint interface. It can be of the geometry type esriGeometryPoint</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For marker anfd polygons the outline it will be this color, for lines this is ignored</param>
        ///<param name="size">size in pixel as integer linewidth of outLine or of intire marker if point</param>
        ///<param name="userLock">locked from editing by user?</param>
        public static IElement AddGraphicToMap(IMap map, IGeometry geometry, IRgbColor rgbColor, 
            IRgbColor outlineRgbColor, int size, bool userLock )
        {
            IGraphicsContainer graphicsContainer = (IGraphicsContainer)map; // Explicit Cast
            IElement element = null;
            if ((geometry.GeometryType) == esriGeometryType.esriGeometryPoint)
            {
                // Marker symbols
                ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
                simpleMarkerSymbol.Color = rgbColor;
                simpleMarkerSymbol.Outline = true;
                simpleMarkerSymbol.OutlineColor = outlineRgbColor;
                simpleMarkerSymbol.Size = size;
                simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;

                IMarkerElement markerElement = new MarkerElementClass();
                markerElement.Symbol = simpleMarkerSymbol;
                element = (IElement)markerElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
            {
                //  Line elements
                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = rgbColor;
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = size;

                ILineElement lineElement = new LineElementClass();
                lineElement.Symbol = simpleLineSymbol;
                element = (IElement)lineElement; // Explicit Cast
            }
            else if ((geometry.GeometryType) == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
            {
                // Polygon elements
                ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
                simpleLineSymbol.Color = outlineRgbColor;
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = size;

                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                simpleFillSymbol.Color = rgbColor;
                simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSForwardDiagonal;
                simpleFillSymbol.Outline = simpleLineSymbol;

                IFillShapeElement fillShapeElement = new PolygonElementClass();
                fillShapeElement.Symbol = simpleFillSymbol;

                element = (IElement)fillShapeElement; // Explicit Cast
            }
            if (!(element == null))
            {
                element.Geometry = geometry;
                element.Locked = userLock;
                graphicsContainer.AddElement(element, 0);
            }
            return element;
        }
        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IPoint interface. It can be of the geometry type esriGeometryPoint</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For marker anfd polygons the outline it will be this color, for lines this is ignored</param>
        ///<param name="size">size in pixel as integer linewidth of outLine or of intire marker if point</param>
        public static IElement AddGraphicToMap(IMap map, IGeometry geometry, IRgbColor rgbColor, 
            IRgbColor outlineRgbColor,  int size){
            return  AddGraphicToMap(map, geometry, rgbColor, outlineRgbColor, size, false);
        }
        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IPoint interface. It can be of the geometry type esriGeometryPoint</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For marker anfd polygons the outline it will be this color, for lines this is ignored</param>
        public static IElement AddGraphicToMap(IMap map, IGeometry geometry, IRgbColor rgbColor, IRgbColor outlineRgbColor){
            return  AddGraphicToMap(map, geometry, rgbColor, outlineRgbColor, 5, false);
        }
        #endregion

        #region "Add text to map"
        public static IElement AddTextElement(IMap map, IPoint point, string text)
        {
            IGraphicsContainer graphicsContainer = map as IGraphicsContainer;
            IElement element = new TextElementClass();
            ITextElement textElement = element as ITextElement;

            element.Geometry = point;
            textElement.Text = text;
            graphicsContainer.AddElement(element, 0);
            return element;
        }
        #endregion    

        #region "Create a field"
        /// <summary>
        /// create a single fieldClass 
        /// </summary>
        /// <param name="name">the name of field</param>
        /// <param name="fieldType">the type of the record: string, int, double or date</param>
        /// <param name="len">the of a string field, ignored for other types</param>
        /// <returns>the fieldClass</returns>
        public static IField createField( string name, esriFieldType fieldType , int len) 
        {
            IField2 field = new FieldClass();
            IFieldEdit2 fieldEdit = (IFieldEdit2)field;
            fieldEdit.Name_2 = name;
            fieldEdit.Type_2 =  fieldType;
            if (fieldType == esriFieldType.esriFieldTypeString ) fieldEdit.Length_2 = len;
            fieldEdit.IsNullable_2 = true;
            return (IField) field;
        }
        public static IField createField( string name, esriFieldType fieldType){
           return createField(name, fieldType, 64);
        }
        public static IField createField( string name){
           return createField(name, esriFieldType.esriFieldTypeString, 64);
        }
        #endregion

        #region "Create ShapeFile / featureclass"
        /// <summary>Create a ESRI shapefile </summary>
        /// <param name="shapeFilePath">the path to the shapefile</param>
        /// <param name="field2add">List of Ifields</param>
        /// <param name="srs">the srs of the shapefile</param>
        /// <param name="geomType">the type geometry: point, polyline, polygon, ...</param>
        /// <param name="deleteIfExists">Overwrite existing FeatureClass</param>
        /// <returns>the shapefile loaded in a IFeatureClass</returns>
        public static IFeatureClass createShapeFile(string shapeFilePath, List<IField> field2add , 
                                  ISpatialReference srs, esriGeometryType geomType, bool deleteIfExists )
        {
            FileInfo shapeInfo = new FileInfo(shapeFilePath);

            // Instantiate a feature class description to get the required fields.
            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = fcDescription as IObjectClassDescription;

            IFields fields = ocDescription.RequiredFields;
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;

            // Find the shape field in the required fields and modify its GeometryDef to
            // use wanted geometry and to set the spatial reference.
            int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
            IField geofield = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = geofield.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geomType;
            geometryDefEdit.SpatialReference_2 = srs;

            foreach (IField field in field2add)
            {
                fieldsEdit.AddField(field);
            }

            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactoryClass();
            IWorkspace workspace = workspaceFactory.OpenFromFile( shapeInfo.DirectoryName , 0);
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace) workspace; // Explict Cast

            if ( shapeInfo.Exists )
            {
                if (deleteIfExists)
                {
                    if (!deleteFeatureClass(shapeInfo.FullName)) 
                    {
                        throw new Exception(shapeInfo.Name + " exists and cannot be deleted");
                    }
                }
                else
                {
                    throw new Exception(shapeInfo.Name + " exists");
                }
            }

            IFields validatedFields = validateFields((IWorkspace)workspace, fields);

            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(shapeInfo.Name, validatedFields, 
                                               ocDescription.InstanceCLSID,  ocDescription.ClassExtensionCLSID, 
                                               esriFeatureType.esriFTSimple, fcDescription.ShapeFieldName, "");

            return featureClass;
        }
        /// <summary>Create a ESRI shapefile </summary>
        /// <param name="shapeFilePath">the path to the shapefile</param>
        /// <param name="field2add">List of Ifields</param>
        /// <param name="srs">the srs of the shapefile</param>
        /// <param name="geomType">the type geometry: point, polyline, polygon, ...</param>
        /// <returns>the shapefile loaded in a IFeatureClass</returns>
        public static IFeatureClass createShapeFile(string shapeFilePath, List<IField> field2add , 
                                  ISpatialReference srs, esriGeometryType geomType)
        {
            return createShapeFile(shapeFilePath, field2add, srs, geomType, false);
        }
        /// <summary>Create a feature class in a existing FGDB </summary>
        /// <param name="FGDBPath">Path to the existing FGDB</param>
        /// <param name="FCname">Name of the new FeatureClass</param>
        /// <param name="field2add">List of Ifields</param>
        /// <param name="srs">the srs of the shapefile</param>
        /// <param name="geomType">the type geometry: point, polyline, polygon, ...</param>
        /// <param name="deleteIfExists">Overwrite existing FeatureClass</param>
        /// <returns>the Feuture Class loaded in a IFeatureClass</returns>
        public static IFeatureClass createFeatureClass(string FGDBPath, string FCname, List<IField> field2add,
                                              ISpatialReference srs, esriGeometryType geomType, bool deleteIfExists)
        {
            DirectoryInfo fgbInfo = new DirectoryInfo(FGDBPath);

            if (FGDBPath == "" || FCname == "") return null; // name was not passed in 

            IFeatureClass featureClass;

            // Instantiate a feature class description to get the required fields.
            IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
            IObjectClassDescription ocDescription = fcDescription as IObjectClassDescription;

            IFields fields = ocDescription.RequiredFields;
            IFieldsEdit fieldsEdit = (IFieldsEdit) fields ;

            // Find the shape field in the required fields and modify its GeometryDef to
            // use wanted geometry and to set the spatial reference.
            IField shapeField = null;
            for (int j = 0; j < fields.FieldCount; j++)
            {
                if (fields.get_Field(j).Type == ESRI.ArcGIS.Geodatabase.esriFieldType.esriFieldTypeGeometry)
                {
                    shapeField = fields.get_Field(j);
                    break;
                }
            }
            if (shapeField == null) return null;

            int shapeFieldIndex = fields.FindField(shapeField.Name);
            IField geofield = fields.get_Field(shapeFieldIndex);
            IGeometryDef geometryDef = geofield.GeometryDef;
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geomType;
            geometryDefEdit.SpatialReference_2 = srs;

            foreach (IField field in field2add)
            {
                fieldsEdit.AddField(field);
            }
            
            IWorkspaceFactory2 workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
            IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(FGDBPath, 0);
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

            string validatedName = ValidateTableName((IWorkspace)workspace, FCname);

            if (workspace.get_NameExists(esriDatasetType.esriDTFeatureClass, validatedName))
            {
                if (deleteIfExists)
                {
                    if (!deleteFeatureClass(fgbInfo.FullName + "\\" + validatedName))
                    {
                        throw new Exception(validatedName + " exists and cannot be deleted");
                    }
                }
                else
                {
                    throw new Exception(validatedName + " exists");
                }
            }

            IFields validatedFields = validateFields((IWorkspace) workspace, fields);

            //create the feature class
            featureClass = featureWorkspace.CreateFeatureClass(validatedName, validatedFields,
                                       ocDescription.InstanceCLSID, ocDescription.ClassExtensionCLSID,
                                       esriFeatureType.esriFTSimple, shapeField.Name, "");
            return featureClass;
        }
        /// <summary>Create a feature class in a existing FGDB </summary>
        /// <param name="FGDBPath">Path to the existing FGDB</param>
        /// <param name="FCname">Name of the new FeatureClass</param>
        /// <param name="field2add">List of Ifields</param>
        /// <param name="srs">the srs of the shapefile</param>
        /// <param name="geomType">the type geometry: point, polyline, polygon, ...</param>
        /// <returns>the Feuture Class loaded in a IFeatureClass</returns>
        public static IFeatureClass createFeatureClass(string FGDBPath, string FCname, List<IField> field2add , 
                                  ISpatialReference srs, esriGeometryType geomType)
        {
            return createFeatureClass(FGDBPath, FCname, field2add, srs, geomType, false);
        }
        
        /// <summary>return a validated set of fields</summary>
        /// <param name="workspace">the input workspace</param>
        /// <param name="fields">the input fields</param>
        /// <returns></returns>
        public static IFields validateFields( IWorkspace workspace, IFields fields)
        {
            // Use IFieldChecker to create a validated fields collection.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null;
            fieldChecker.ValidateWorkspace = workspace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
            return validatedFields;
        }

        /// <summary>returns a validated table name</summary>
        /// <param name="workspace">the input workspace of the tablenaam</param>
        /// <param name="tableName">the input tablename</param>
        /// <returns>the output laundered tablenaam</returns>
        public static String ValidateTableName(IWorkspace workspace, String tableName)
        {
            // Create a field checker to validate the table name.
            IFieldChecker fieldChecker = new FieldCheckerClass();
            String validatedName = null;
            fieldChecker.ValidateWorkspace = workspace;
            fieldChecker.ValidateTableName(tableName, out validatedName);
            validatedName = validatedName.Replace("\"", "_").Replace("\'", "_");
            return validatedName;
        }   
        #endregion

        #region "Add featureclass to map"
        /// <summary> Add feature class to active View and then zoom to its extend </summary>
        /// <param name="view">the current active view</param>
        /// <param name="inFeatureClass">the feature class to add</param>
        /// <param name="zoomTo">zoom to loaded feature class</param>
        /// <returns>the created layer</returns>
        public static IFeatureLayer addFeatureClassToMap(IActiveView view, IFeatureClass inFeatureClass, bool zoomTo)
        {
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = inFeatureClass;
            featureLayer.Name = inFeatureClass.AliasName;
            featureLayer.Visible = true;
            view.FocusMap.AddLayer(featureLayer);

            if(zoomTo) view.Extent = featureLayer.AreaOfInterest;
                
            return featureLayer;
        }
        public static IFeatureLayer addFeatureClassToMap(IActiveView view, IFeatureClass inFeatureClass)
        {    
            return addFeatureClassToMap(view, inFeatureClass, false);
        }
        #endregion

        #region "Internet available?"
        /// <summary>check if internet available </summary>
        public static bool IsConnectedToInternet
        {
            get
            {
                try 
                {
                    HttpWebRequest hwebRequest = (HttpWebRequest)WebRequest.Create("https://www.google.be/"); 
                    hwebRequest.Timeout = 5000; //5 seconds timeout to process the request.
                    HttpWebResponse hWebResponse = (HttpWebResponse)hwebRequest.GetResponse(); //Process the request.
                    if (hWebResponse.StatusCode == HttpStatusCode.OK) //Get the response.
                    {
                        return true; //If true, the user is connected to the internet.
                    }
                    else return false; //Else it is not.
                }
                catch (Exception ex) {
                    MessageBox.Show("Geen internet connectie", ex.Message);
                    return false;
                }
            }
        }
        /// <summary>Check if a url refers to a existing site </summary>
        /// <param name="url">the url</param>
        /// <param name="isWMS">indicate if the url is wms</param>
        /// <returns>true if exists</returns>
        public static bool websiteExists( string url, bool isWMS ){
            HttpWebResponse response = null;

            if (isWMS)
            {
                url = url.Split('?')[0] + "?request=GetCapabilities&service=WMS";
            }

            var hwebRequest = (HttpWebRequest)WebRequest.Create(url);

            hwebRequest.Timeout = 8000;
            //hwebRequest.Method = "HEAD";

            try
            {
                response = (HttpWebResponse)hwebRequest.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
        /// <summary>Check if a url refers to a existing site </summary>
        /// <param name="url">the url</param>
        /// <returns>true if exists</returns>
        public static bool websiteExists( string url ){
            return websiteExists(url, false);
        }
        #endregion

        #region "Delete shapefile or FGDB Feature Class"
        /// <summary>Delete a existing shapefile</summary>
        /// <param name="path">the full path to the shapefile (*.shp)</param>
        /// <returns>return if succesfull otherwise false</returns> 
        public static bool deleteFeatureClass( string path )
        {
            FileInfo featureClassPath = new FileInfo(path);

            if (  path.ToLowerInvariant().EndsWith( ".shp"))
            {
                if (!featureClassPath.Exists) return true;

                //create factory
                IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(
                                            featureClassPath.DirectoryName, 0);
                //create featureclass 
                IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(
                                            featureClassPath.Name.Replace(featureClassPath.Extension, ""));
                //cast tot dataset and delete
                IDataset ds = featureClass as IDataset;
                if (ds.CanDelete())
                {
                    ds.Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if ( featureClassPath.DirectoryName.ToLowerInvariant().EndsWith(".gdb") )
            {
                    IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                    IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile( 
                                                                                    featureClassPath.DirectoryName, 0);
                    IWorkspace2 ws = (IWorkspace2)featureWorkspace;
                    if (!ws.get_NameExists(esriDatasetType.esriDTFeatureClass, featureClassPath.Name))
                    {
                        return true;
                    }
                    IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(featureClassPath.Name);
                    IDataset ds = (IDataset) featureClass;
                    if (ds.CanDelete())
                    {
                        ds.Delete();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            else
            {
                throw new Exception("File is not a shapefile or Filegeodatabase Feature Class");
            }
        }
        #endregion

        #region "open / save dialogs"
        /// <summary> Shows dialog for saving data.</summary>
        /// <param name="filters"> for example:
        ///       IGxObjectFilter ipFilter1 = new GxFilterFGDBFeatureClassesClass() ;
        ///       List&lt;IGxObjectFilter&gt; gxFilterList = new List&lt;IGxObjectFilter&gt;(new IGxObjectFilter[] { ipFilter1 });</param>
        /// <param name="dialogTitle">The title of the dialog</param>
        /// <returns> the path to the file to save </returns>
        public static string ShowSaveDataDialog(List<IGxObjectFilter> filters, string dialogTitle)
        {
            try
            {
                IGxDialog pGxDialog = new GxDialogClass();
                pGxDialog.Title = dialogTitle;
                pGxDialog.AllowMultiSelect = false;

                // Create a filter collection for the dialog.
                IGxObjectFilterCollection pFilterCol = (IGxObjectFilterCollection)pGxDialog;

                foreach (IGxObjectFilter filt in filters)
                {
                    pFilterCol.AddFilter(filt, false);
                }

                // Open the dialog
                if (!pGxDialog.DoModalSave(0))
                {
                    return null;
                }
                string dirPath = pGxDialog.FinalLocation.FullName;
                string path = dirPath + "\\" + pGxDialog.Name;

                if (dirPath.ToLowerInvariant().EndsWith(".gdb"))
                {
                    IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                    IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(dirPath, 0);
                    IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                    if (workspace.get_NameExists(esriDatasetType.esriDTFeatureClass, pGxDialog.Name))
                    {
                        IDataset fc = (IDataset)featureWorkspace.OpenFeatureClass(pGxDialog.Name);
                        DialogResult dlg = MessageBox.Show(pGxDialog.Name + " bestaat al, wilt u dit overschrijven?",
                                    "Feature Class bestaat al", MessageBoxButtons.YesNo);
                        if (dlg == DialogResult.No)
                        {
                            return null;
                        }
                        else if (!fc.CanDelete())
                        {
                            MessageBox.Show(pGxDialog.Name + " is al in gebruik, u kunt dit niet overschrijven.",
                                        "Feature Class in gebruik");
                            return null;
                        }
                    }
                }
                else if (Directory.Exists(dirPath))
                {
                    IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
                    IWorkspace2 workspace = (IWorkspace2)workspaceFactory.OpenFromFile(dirPath, 0);
                    IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspace;

                    if (workspace.get_NameExists(esriDatasetType.esriDTFeatureClass, pGxDialog.Name))
                    {
                        IDataset fc = (IDataset)featureWorkspace.OpenFeatureClass(pGxDialog.Name);

                        if (!fc.CanDelete())
                        {
                            MessageBox.Show(pGxDialog.Name + " is al in gebruik, u kunt dit niet overschrijven.",
                                        "Feature Class in gebruik");
                            return null;
                        }
                    }
                }
                return path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace);
                return null;
            }
        }

        /// <summary> Shows dialog for saving data.</summary>
        /// <param name="dialogTitle">The title of the dialog</param>
        /// <returns> the path to the file to save </returns>
        public static string ShowSaveDataDialog( string dialogTitle)
        {
            IGxObjectFilter shpFilter = new GxFilterShapefilesClass();
            IGxObjectFilter gdbFilter = new GxFilterFGDBFeatureClassesClass();
            List<IGxObjectFilter> gxFilterList = new List<IGxObjectFilter>(new IGxObjectFilter[] { gdbFilter, shpFilter});

            return ShowSaveDataDialog(gxFilterList, dialogTitle);
        }
        #endregion

        #region "geosjon converter"
        /// <summary>Convert a GoeJSON point geometry to Arcgis Geometry </summary>
        /// <param name="JSpoint">The deserialised GeoJson Object</param>
        /// <param name="epsg">The EPSG-code of the spatial reference, -1 is unknown</param>
        /// <returns>A Arcgis Point goemetry</returns>
        public static IPoint geojson2esriPoint(datacontract.geojsonPoint JSpoint, int epsg)
        {
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;

            IPoint esriPnt = new PointClass() { X = JSpoint.coordinates[0], Y = JSpoint.coordinates[1] };

             if (epsg != -1)
             {
                 ISpatialReference srs = spatialReferenceFactory.CreateSpatialReference(epsg);
                 esriPnt.SpatialReference = srs;
             }
             return esriPnt;
        }
        /// <summary>Convert a GoeJSON point geometry to Arcgis Geometry </summary>
        /// <param name="JSpoint">The deserialised GeoJson Object</param>
        /// <returns>A Arcgis Point goemetry</returns>
        public static IPoint geojson2esriPoint(datacontract.geojsonPoint JSpoint){
            return geojson2esriPoint(JSpoint, -1);
        }
        /// <summary>Convert a GeoJSON Line geometry to Arcgis Geometry </summary>
        /// <param name="JSline">The deserialised GeoJson Object</param>
        /// <param name="epsg">The EPSG-code of the spatial reference, -1 is unknown</param>
        /// <returns>A Arcgis PolyLine goemetry</returns>
        public static IPolyline geojson2esriLine(datacontract.geojsonLine JSline, int epsg)
        {
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;

            IGeometryBridge2 pGeoBrg = new GeometryEnvironment() as IGeometryBridge2;

            ESRI.ArcGIS.esriSystem.WKSPoint[] aWKSPointBuffer = new ESRI.ArcGIS.esriSystem.WKSPoint[ JSline.coordinates.Count ];
            for (int n = 0; n < JSline.coordinates.Count; n++)
            {
                double[] xy = JSline.coordinates[n].ToArray();
                aWKSPointBuffer[n].X = xy[0];
                aWKSPointBuffer[n].Y = xy[1];
            }

            IPolyline esriLine = new PolylineClass();
            pGeoBrg.SetWKSPoints(esriLine as IPointCollection4, ref aWKSPointBuffer);

            if (epsg != -1)
            {
                ISpatialReference srs = spatialReferenceFactory.CreateSpatialReference(epsg);
                esriLine.SpatialReference = srs;
            }

            return esriLine;
        }
        /// <summary>Convert a GeoJSON Line geometry to Arcgis Geometry </summary>
        /// <param name="JSline">The deserialised GeoJson Object</param>
        /// <returns>A Arcgis PolyLine goemetry</returns>
        public static IPolyline geojson2esriLine(datacontract.geojsonLine JSline){
            return geojson2esriLine( JSline, -1 );
        }
        /// <summary>Convert a GeoJSON Polygon geometry to Arcgis Geometry </summary>
        /// <param name="JSPolygon">The deserialised GeoJson Object</param>
        /// <param name="epsg">The EPSG-code of the spatial reference, -1 is unknown</param>
        /// <returns>A Arcgis Polygon goemetry</returns>
        public static IPolygon geojson2esriPolygon(datacontract.geojsonPolygon JSPolygon, int epsg)
        {
            Type factoryType = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(factoryType);
            ISpatialReferenceFactory3 spatialReferenceFactory = obj as ISpatialReferenceFactory3;

            IGeometryBridge2 pGeoBrg = new GeometryEnvironment() as IGeometryBridge2;

            IGeometryCollection esriGeometryCol = new PolygonClass();

            for (int n = 0; n < JSPolygon.coordinates.Count; n++)
            {
                List<List<double>> JSring = JSPolygon.coordinates[n];
                IPointCollection4 ring = new RingClass();
                ESRI.ArcGIS.esriSystem.WKSPoint[] aWKSPointBuffer = new ESRI.ArcGIS.esriSystem.WKSPoint[JSring.Count];
                for (int i = 0; i < JSring.Count; i++)
                {
                    double[] xy = JSring[i].ToArray();
                    aWKSPointBuffer[i].X = xy[0];
                    aWKSPointBuffer[i].Y = xy[1];
                }
                pGeoBrg.SetWKSPoints(ring , ref aWKSPointBuffer);
                object Missing = Type.Missing;
                esriGeometryCol.AddGeometry(ring as IGeometry, ref Missing, ref Missing);
            }

            IPolygon esriPolygon = esriGeometryCol as IPolygon;

            if (epsg != -1)
            {
                ISpatialReference srs = spatialReferenceFactory.CreateSpatialReference(epsg);
                esriPolygon.SpatialReference = srs;
            }
            return esriPolygon;
        }
        /// <summary>Convert a GeoJSON Polygon geometry to Arcgis Geometry </summary>
        /// <param name="JSPolygon">The deserialised GeoJson Object</param>
        /// <returns>A Arcgis Polygon goemetry</returns>
        public static IPolygon geojson2esriPolygon(datacontract.geojsonPolygon JSPolygon){
            return geojson2esriPolygon(JSPolygon, -1);
        }
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI point object</param>
        /// <returns>A geojson Object</returns>
        public static datacontract.geojsonPoint esri2geojsonPoint(IPoint esriPoint) 
        {
            int epsg;
            string epsgUri;

            if (esriPoint.SpatialReference == null)
            {
                epsgUri = "";
            }
            else if (esriPoint.SpatialReference.FactoryCode == 900913 || esriPoint.SpatialReference.FactoryCode == 102100)
            {
                epsg = 3857;//google mercator
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }
            else
            {
                epsg = esriPoint.SpatialReference.FactoryCode;
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }

            datacontract.geojsonCRS JScrs = new datacontract.geojsonCRS(){ 
                type= "link",
                properties = new Dictionary<string, string>() { { "href", epsgUri } }
            };

            datacontract.geojsonPoint JSpoint = new datacontract.geojsonPoint()
            {
                type = "Point",
                coordinates = new List<double>() { esriPoint.X, esriPoint.Y },
                crs = JScrs
            };

           return JSpoint;
        }
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI polygon object</param>
        /// <returns>A geojson string</returns>
        public static string esri2geojsonPointString(IPoint esriPoint)
        {
            datacontract.geojsonPoint gjs = esri2geojsonPoint(esriPoint);
            return JsonConvert.SerializeObject(gjs);
        }
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI polyline object</param>
        /// <returns>A geojson Object</returns>
        public static datacontract.geojsonLine esri2geojsonLine(IPolyline esriLine)
        {
            int epsg;
            string epsgUri;

            if (esriLine.SpatialReference == null)
            {
                epsgUri = "";
            }
            else if (esriLine.SpatialReference.FactoryCode == 900913 || esriLine.SpatialReference.FactoryCode == 102100)
            {
                epsg = 3857;//google mercator
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }
            else
            {
                epsg = esriLine.SpatialReference.FactoryCode;
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }

            datacontract.geojsonCRS JScrs = new datacontract.geojsonCRS()
            {
                type = "link",
                properties = new Dictionary<string, string>() { { "href", epsgUri } }
            };

            datacontract.geojsonLine JSline = new datacontract.geojsonLine() {type = "Polyline", crs = JScrs };
            List<List<double>> coords = new List<List<double>>();

            IPointCollection4 nodes = esriLine as IPointCollection4;

            for (int n = 0; n < nodes.PointCount; n++)
            {
                IPoint node = nodes.get_Point(n);
                List<double> pt = new List<double>() {node.X, node.Y };
                coords.Add(pt);
            }
            JSline.coordinates = coords;

            return JSline;
        }
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI polyline object</param>
        /// <returns>A geojson String</returns>
        public static string esri2geojsonLineString(IPolyline esriLine)
        {
            datacontract.geojsonLine gjsline = esri2geojsonLine(esriLine);
            return JsonConvert.SerializeObject(gjsline);
        }
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI polygon object</param>
        /// <returns>A geojson Object</returns>
        public static datacontract.geojsonPolygon esri2geojsonPolygon(IPolygon esriPolygon)
        {
            int epsg;
            string epsgUri;
            
            if (esriPolygon.SpatialReference == null) {
                epsgUri = "";
            }
            else if (esriPolygon.SpatialReference.FactoryCode == 900913 || esriPolygon.SpatialReference.FactoryCode == 102100)
            {
                epsg = 3857;//google mercator
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }
            else { 
                epsg = esriPolygon.SpatialReference.FactoryCode;
                epsgUri = string.Format("http://www.opengis.net/def/crs/EPSG/0/{0}", epsg);
            }

            datacontract.geojsonCRS JScrs = new datacontract.geojsonCRS()
            {
                type = "link",
                properties = new Dictionary<string, string>() { { "href", epsgUri } }
            };

            datacontract.geojsonPolygon JSpolygon = new datacontract.geojsonPolygon() { type = "Polygon", crs = JScrs };
            List<List<List<double>>> coords = new List<List<List<double>>>();

            IGeometryCollection rings = esriPolygon as IGeometryCollection;
            for (int n = 0; n < rings.GeometryCount; n++) 
            {
                IPointCollection4 ring = rings.get_Geometry(n) as IPointCollection4;
                List<List<double>> JSring = new List<List<double>>();

                for (int i = 0; i < ring.PointCount; i++)
                {
                   IPoint pt = ring.get_Point(i);
                   List<double> JSpt = new List<double>() {pt.X, pt.Y };
                   JSring.Add(JSpt);
                }
                coords.Add(JSring);

            }
            JSpolygon.coordinates = coords;

            return JSpolygon;
        }     
        /// <summary>Convert a ESRI geometry to geojson </summary>
        /// <param name="esriPoint">A ESRI polygon object</param>
        /// <returns>A geojson string</returns>
        public static string esri2geojsonPolygonString(IPolygon esriPoly)
        {
            datacontract.geojsonPolygon gjsline = esri2geojsonPolygon(esriPoly);
            return JsonConvert.SerializeObject(gjsline);
        }

        #endregion

        #region "WMS handlers"
        /// <summary>Add a WMS to the map</summary>
        /// <param name="map">The map to add the wms to</param>
        /// <param name="WMSurl">the url point to the WMS</param>
        public static void addWMS2map(IMap map, string WMSurl, short transparency, string layername)
        {
           IActiveView view;
           ILayer lyr;
           
           if (layername != null) {
              lyr = getWMSLayerByName(WMSurl, layername);
           }
           else {
              lyr = getWMSasLayer(WMSurl);
           }

           ILayerEffects layerEffects = (ILayerEffects)lyr;
           layerEffects.Transparency = transparency < 100 ? transparency : (short)100;

           map.AddLayer(lyr);
           view = (IActiveView)map;
           makeCompositeLayersVisibile(lyr);
           view.ContentsChanged();
        }
        public static void addWMS2map(IMap map, string WMSurl, short transparency) {
            addWMS2map(map, WMSurl, transparency, null);
        }
        public static void addWMS2map(IMap map, string WMSurl, string layerName){
            addWMS2map(map, WMSurl, 0, layerName);
        }
        public static void addWMS2map(IMap map, string WMSurl) {
            addWMS2map(map, WMSurl, 0, null);
        }

        public static ILayer getWMSLayerByName(string WMSurl, string layerName)
        {
            IPropertySet propSet = new PropertySetClass();
            IWMSConnectionName connName = new WMSConnectionNameClass();
            IWMSGroupLayer wmsMapLayer = new WMSMapLayerClass();
            IDataLayer dataLayer = (IDataLayer)wmsMapLayer;
            IWMSServiceDescription serviceDesc;
            ILayer qrylyr; ILayer wmsLyr;
            //connect to url
            propSet.SetProperty("URL", WMSurl);
            connName.ConnectionProperties = propSet;
            dataLayer.Connect((IName)connName);
            //get service description
            serviceDesc = wmsMapLayer.WMSServiceDescription;
            //loop through layers

            List<IWMSLayerDescription> lyrs = listWMSlayers(serviceDesc);
            //query layers
            List<IWMSLayerDescription> qry = (
                from IWMSLayerDescription q in lyrs 
                where q.Name == layerName select q).ToList<IWMSLayerDescription>() ;

            //nothing found
            if ( qry.Count == 0 ) return null;

            //else
            IWMSLayerDescription layerDesc = qry[0];
            //clear allready loaded contents
            wmsMapLayer.Clear();

            if (layerDesc.LayerDescriptionCount == 0)
            {
                qrylyr = (ILayer)wmsMapLayer.CreateWMSLayer(layerDesc);
                wmsMapLayer.InsertLayer(qrylyr, 0);
            }
            else
            {
                qrylyr = (ILayer)wmsMapLayer.CreateWMSGroupLayers(layerDesc);
                wmsMapLayer.InsertLayer(qrylyr, 0);
            }
            makeCompositeLayersVisibile(qrylyr);

            wmsMapLayer.Expanded = true;
            ((IWMSMapLayer)wmsMapLayer).BackgroundColor = new RgbColor() { NullColor = true };
            wmsLyr = (ILayer)wmsMapLayer;
            wmsLyr.Name = serviceDesc.WMSTitle;
            wmsLyr.Visible = true;

            return wmsLyr;
        }

        public static ILayer getWMSasLayer(string WMSurl)
        {
            IPropertySet propSet = new PropertySetClass();
            IWMSConnectionName connName = new WMSConnectionNameClass();
            IWMSGroupLayer wmsMapLayer = new WMSMapLayerClass();
            IDataLayer dataLayer = (IDataLayer) wmsMapLayer;
            IWMSServiceDescription serviceDesc;
            
            //connect to url
            propSet.SetProperty("URL", WMSurl);
            connName.ConnectionProperties = propSet;
            dataLayer.Connect((IName)connName);
            //get service description
            serviceDesc = wmsMapLayer.WMSServiceDescription;

            ILayer wmsLayer = (ILayer)wmsMapLayer;
            wmsLayer.Visible = true;
            wmsMapLayer.Expanded = true;
            wmsLayer.Name = serviceDesc.WMSTitle;
            makeCompositeLayersVisibile( wmsLayer);
            return wmsLayer;
        }

        private static List<IWMSLayerDescription> ListWMSlayersByDescription(IWMSLayerDescription root) 
        {
            List<IWMSLayerDescription> lyrNames = new List<IWMSLayerDescription>();
            lyrNames.Add(root);

            for (int i = 0; i < root.LayerDescriptionCount; i++)
            {
                IWMSLayerDescription layerDesc = root.get_LayerDescription(i);

                lyrNames.Add(layerDesc);

                if (layerDesc.LayerDescriptionCount != 0)
                {
                    List<IWMSLayerDescription> namesL2 = ListWMSlayersByDescription(layerDesc);
                    lyrNames.AddRange(namesL2);
                }
            }
            return lyrNames;
        }

        public static List<IWMSLayerDescription> listWMSlayers(IWMSServiceDescription serviceDesc)
        {
            List<IWMSLayerDescription> lyrNames = new List<IWMSLayerDescription>();

            for (int i = 0; i < serviceDesc.LayerDescriptionCount; i++)
            {
                lyrNames.AddRange(
                    ListWMSlayersByDescription(serviceDesc.get_LayerDescription(i)));
            }
            return lyrNames;
        }
        public static List<IWMSLayerDescription> listWMSlayers(string WMSurl) 
        {
            List<IWMSLayerDescription> lyrNames = new List<IWMSLayerDescription>();
            
            IPropertySet propSet = new PropertySetClass();
            IWMSConnectionName connName = new WMSConnectionNameClass();
            IWMSGroupLayer wmsMapLayer = new WMSMapLayerClass();
            IDataLayer dataLayer = (IDataLayer)wmsMapLayer;
            IWMSServiceDescription serviceDesc;

            //connect to url
            propSet.SetProperty("URL", WMSurl);
            connName.ConnectionProperties = propSet;
            dataLayer.Connect((IName)connName);
            //get service description
            serviceDesc = wmsMapLayer.WMSServiceDescription;

            lyrNames = listWMSlayers(serviceDesc);
            return lyrNames;
        }

        #endregion

        #region "Make Composite Layer Visible"
        /// <summary>Toggle the visibility on and off for a composite layer, including all child layers</summary>
        /// <param name="activeView">An IActiveView interface.</param>
        /// <param name="layer">The composite layer as Ilayer</param>
        /// <param name="onlyGroupLayer">Only turn on all composite layers or all layers</param>
        public static void makeCompositeLayersVisibile(ILayer layer)
        {
            //return if not composite
            if (layer is ICompositeLayer2)
            {
                ICompositeLayer2 compositeLayer2 = layer as ICompositeLayer2;

                //Turn the layer visibility on
                layer.Visible = true;

                //Turn each sub-layer (ie. composite layer) visibility on if onlyGroupLayer == true, loop through sub grouplayers
                for (int compositeLayerIndex = 0; compositeLayerIndex < compositeLayer2.Count; compositeLayerIndex++)
                {
                    ILayer lyr = compositeLayer2.get_Layer(compositeLayerIndex);
                    lyr.Visible = true;
                    makeCompositeLayersVisibile(lyr);
                }
            }
            else
            {
                layer.Visible = true;
            }
        }
        #endregion

        #region "CSV and table related"
        /// <summary>Read a csv file into a datatable </summary>
        /// <param name="csvPath">the path to the csv-file</param>
        /// <param name="separator">the character separating the values, can be "COMMA", "PUNTCOMMA", "SPATIE" or "TAB", 
        /// for any sepator string the the input is used</param>
        /// <returns>a datable containing the values form the file</returns>
        public static DataTable loadCSV2datatable(string csvPath, string separator, int maxRows, System.Text.Encoding codex)
        {
            FileInfo csv = new FileInfo(csvPath);
            string sep;
            DataTable tbl = new DataTable();

            System.Text.Encoding textEncoding = System.Text.Encoding.Default;
            if (codex != null) textEncoding = codex;

            if (!csv.Exists)
                throw new Exception("Deze csv-file bestaat niet: " + csv.Name);
            if( separator == "" || separator == null ) 
                throw new Exception("Deze separator is niet toegelaten");

            switch (separator)
            {
                case "Comma":
                    sep = ",";
                    break;
                case "Puntcomma":
                    sep = ";";
                    break;
                case "Spatie":
                    sep = " ";
                    break;
                case "Tab":
                    sep = "/t";
                    break;
                default:
                    sep = separator;
                    break;
            }
            using (Microsoft.VisualBasic.FileIO.TextFieldParser csvParser =
                            new Microsoft.VisualBasic.FileIO.TextFieldParser(csv.FullName, textEncoding, true))
            {
                csvParser.Delimiters = new string[] { sep };
                csvParser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                csvParser.TrimWhiteSpace = !(separator == "TAB" || separator == "SPATIE") ;

                string[] colNames = csvParser.ReadFields();
                string[] row;
                int counter = 0;

                foreach (string colName in colNames)
                {
                    tbl.Columns.Add(colName);
                }
                while (!csvParser.EndOfData)
                {
                    try
                    {
                        if (counter >= maxRows)
                        {
                            return tbl;
                        }
                        counter++;

                        row = csvParser.ReadFields();

                        if (tbl.Columns.Count != row.Count())
                        {
                            throw new Exception("Niet alle rijen hebben hetzelfde aantal kolommen, op eerste lijn: " +
                             tbl.Rows.Count.ToString() + " gevonden: " + row.Count() + " op lijn: " + string.Join(sep, row));
                        }
                        tbl.Rows.Add(row);
                    }
                    catch (Microsoft.VisualBasic.FileIO.MalformedLineException ex)
                    {
                        throw new Exception("CSV is kan niet worden gelezen, het heeft niet de correcte vorm: " + csvParser.ErrorLine, ex);
                    }
                }
            }
            return tbl;
        }
        /// <summary>Read a csv file into a datatable </summary>
        /// <param name="csvPath">the path to the csv-file</param>
        /// <param name="separator">the character separating the values, can be "COMMA", "PUNTCOMMA", "SPATIE" or "TAB", 
        /// for any sepator string the the input is used</param>
        /// <returns>a datable containing the values form the file</returns>
        public static DataTable loadCSV2datatable(string csvPath, string separator, int maxRows)
        {
            return loadCSV2datatable(csvPath, separator, maxRows, null);
        }
        /// <summary>Read a csv file into a datatable </summary>
        /// <param name="csvPath">the path to the csv-file</param>
        /// <param name="separator">the character separating the values, can be "COMMA", "PUNTCOMMA", "SPATIE" or "TAB", 
        /// for any sepator string the the input is used</param>
        /// <returns>a datable containing the values form the file</returns>
        public static DataTable loadCSV2datatable(string csvPath, string separator)
        {
            return loadCSV2datatable(csvPath, separator, 500, null);
        }
        #endregion

        #region "Get Polyline From Mouse Clicks"
        ///<summary>Create a polyline geometry object using the RubberBand.TrackNew method when a user click the mouse on the map control.</summary>
        ///<param name="activeView">An ESRI.ArcGIS.Carto.IActiveView interface that will user will interace with to draw a polyline.</param>
        ///<returns>An ESRI.ArcGIS.Geometry.IPolyline interface that is the polyline the user drew</returns>
        ///<remarks>Double click the left mouse button to end tracking the polyline.</remarks>
        public static IPolyline GetPolylineFromMouseClicks(IActiveView activeView)
        {
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

            IRubberBand rubberBand = new RubberLineClass();
            IGeometry geometry = rubberBand.TrackNew(screenDisplay, null);

            IPolyline polyline = (IPolyline)geometry;

            return polyline;

        }
        #endregion

        #region "Translate strings"
        /// <summary>Translate the adresType string into u more userfriendly string</summary>
        /// <param name="adresTypeString">the string to translate</param>
        /// <returns>the translated string or the input string if none is found</returns>
        public static string adresTypeStringTranslate(string adresTypeString)
        {
            switch (adresTypeString)
            {
                case "crab_straat":
                     return  "locatie van centrum straat";         
                case "crab_huisnummer_afgeleidVanGebouw":
                     return  "locatie o.b.v. gebouw";
                case "crab_huisnummer_afgeleidVanGemeente":
                     return  "locatie o.b.v. centrum gemeente";
                case "crab_huisnummer_afgeleidVanPerceelGrb":
                     return  "locatie o.b.v. GRB-perceel";
                case "crab_huisnummer_afgeleidVanPerceelKadaster":
                     return  "locatie o.b.v. kadasterperceel";
                case "crab_huisnummer_afgeleidVanStraat":
                     return  "locatie o.b.v. centrum straat";
                case "crab_huisnummer_geinterpoleerdObvNevenliggendeHuisnummersGebouw":
                     return  "locatie o.b.v. interpollatie tussen gebouwen";
                case "crab_huisnummer_geinterpoleerdObvNevenliggendeHuisnummersPerceelGrb":
                     return  "locatie o.b.v. interpollatie tussen GRB-percelen";
                case "crab_huisnummer_geinterpoleerdObvNevenliggendeHuisnummersPerceelKadaster":
                     return  "locatie o.b.v. interpollatie tussen kadasterpercelen";
                case "crab_huisnummer_geinterpoleerdObvWegverbinding":
                     return  "locatie o.b.v. wegverbinding";
                case "crab_huisnummer_manueleAanduidingVanBrievenbus":
                     return  "locatie o.b.v. manuele aanduiding van brievenbus";
                case "crab_huisnummer_manueleAanduidingVanGebouw":
                     return  "locatie o.b.v. manuele aanduiding van gebouw";
                case "crab_huisnummer_manueleAanduidingVanIngangVanGebouw":
                     return  "locatie o.b.v. manuele aanduiding van ingang gebouw";
                case "crab_huisnummer_manueleAanduidingVanLigplaats":
                    return  "locatie o.b.v. manuele aanduiding van ligplaats";
                case "crab_huisnummer_manueleAanduidingVanLot":
                     return  "locatie o.b.v. manuele aanduiding van lot";
                case "crab_huisnummer_manueleAanduidingVanNutsaansluiting":
                    return  "locatie o.b.v. manuele aanduiding van nutsaansluiting";
                case "crab_huisnummer_manueleAanduidingVanPerceel":
                     return  "locatie o.b.v. manuele aanduiding van perceel";
                case "crab_huisnummer_manueleAanduidingVanStandplaats":
                    return  "locatie o.b.v. manuele aanduiding van standplaats";
                case "crab_huisnummer_manueleAanduidingVanToegangTotDeWeg":
                    return  "locatie o.b.v. manuele aanduiding van toegang tot de weg";
                default:
                    return adresTypeString;
            }
        }
        #endregion

    }
}
