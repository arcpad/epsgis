// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Windows.Forms;
using EPS.Engine.Utils;
using ESRI.ArcGIS.Controls;
using EPS.Main.Tools.Identify;
using System.IO;

namespace EPS.Main.Tools
{
    /// <summary>
    /// Summary description for DrawGraphicLine.
    /// </summary>
    //[Guid("001c57ca-c292-459d-95a7-9984d78d0d93")]
    //[ClassInterface(ClassInterfaceType.None)]
    //[ProgId("CustomTool.IdentifyTool")]
    public sealed class IdentifyTool : BaseTool
    {
        //#region COM Registration Function(s)
        //[ComRegisterFunction()]
        //[ComVisible(false)]
        //static void RegisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryRegistration(registerType);

        //    //
        //    // TODO: Add any COM registration code here
        //    //
        //}

        //[ComUnregisterFunction()]
        //[ComVisible(false)]
        //static void UnregisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryUnregistration(registerType);

        //    //
        //    // TODO: Add any COM unregistration code here
        //    //
        //}

        //#region ArcGIS Component Category Registrar generated code
        ///// <summary>
        ///// Required method for ArcGIS Component Category registration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryRegistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    MxCommands.Register(regKey);

        //}
        ///// <summary>
        ///// Required method for ArcGIS Component Category unregistration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryUnregistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    MxCommands.Unregister(regKey);

        //}

        //#endregion
        //#endregion

        public IdentifyTool()
        {
            base.m_category = "EPS.Tools"; //localizable text 
            base.m_caption = "自定义识别";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "属性识别.";  //localizable textt
            base.m_name = "CustomTool_IdentifyTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = Properties.Resources.IdentifyWindowShow16;
                
                //new Bitmap(GetType(), bitmapResourceName);
                MemoryStream ms = new MemoryStream(Properties.Resources.SelectFeatures);
                ms.Position = 0;
                base.m_cursor = new Cursor(ms);
                // ms.Close();
                    //new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        private IHookHelper m_hookHelper = new HookHelperClass();

        #region Overriden Class Methods


        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_hookHelper.Hook = hook;
        }

        private IdentifyDialog identifyDialog;

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // 新建属性查询对象
            identifyDialog = IdentifyDialog.CreateInstance(MainForm.Instance.AxMapCtrl);
            identifyDialog.Owner = MainForm.Instance;
            identifyDialog.Show();
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            IPoint point = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            if (identifyDialog.IsDisposed)
            {
                OnClick();
            }

            identifyDialog.OnMouseDown(Button, point.X, point.Y);


            ////Get the active view from the application object (ie. hook)
            //IActiveView activeView = MapUtils.GetActiveView();

            ////Get the polyline object from the users mouse clicks
            //IPolyline polyline = GetPolylineFromMouseClicks(activeView);

            ////Make a color to draw the polyline 
            //IRgbColor rgbColor = new RgbColorClass();
            //rgbColor.Red = 255; 

            ////Add the users drawn graphics as persistent on the map
            //AddGraphicToMap(activeView.FocusMap, polyline, rgbColor, rgbColor);

            ////Only redraw the portion of the active view that contains the graphics 
            //activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            IPoint point = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            identifyDialog.OnMouseMove(point.X, point.Y);
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            IPoint point = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            identifyDialog.OnMouseUp(point.X, point.Y);
        }
        #endregion
        
        #region "Get Polyline From Mouse Clicks"

        ///<summary>
        ///Create a polyline geometry object using the RubberBand.TrackNew method when a user click the mouse on the map control. 
        ///</summary>
        ///<param name="activeView">An ESRI.ArcGIS.Carto.IActiveView interface that will user will interace with to draw a polyline.</param>
        ///<returns>An ESRI.ArcGIS.Geometry.IPolyline interface that is the polyline the user drew</returns>
        ///<remarks>Double click the left mouse button to end tracking the polyline.</remarks>
        public IPolyline GetPolylineFromMouseClicks(IActiveView activeView)
        {

          IScreenDisplay screenDisplay = activeView.ScreenDisplay;

          IRubberBand rubberBand = new RubberLineClass();
          IGeometry geometry = rubberBand.TrackNew(screenDisplay, null);

          IPolyline polyline = (IPolyline)geometry;

          return polyline;

        }
        #endregion

        #region "Add Graphic to Map"

        ///<summary>Draw a specified graphic on the map using the supplied colors.</summary>
        ///      
        ///<param name="map">An IMap interface.</param>
        ///<param name="geometry">An IGeometry interface. It can be of the geometry type: esriGeometryPoint, esriGeometryPolyline, or esriGeometryPolygon.</param>
        ///<param name="rgbColor">An IRgbColor interface. The color to draw the geometry.</param>
        ///<param name="outlineRgbColor">An IRgbColor interface. For those geometry's with an outline it will be this color.</param>
        ///      
        ///<remarks>Calling this function will not automatically make the graphics appear in the map area. Refresh the map area after after calling this function with Methods like IActiveView.Refresh or IActiveView.PartialRefresh.</remarks>
        public void AddGraphicToMap(IMap map, IGeometry geometry, IRgbColor rgbColor, IRgbColor outlineRgbColor)
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
            simpleMarkerSymbol.Size = 15;
            simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;

            IMarkerElement markerElement = new MarkerElementClass();
            markerElement.Symbol = simpleMarkerSymbol;
            element = (IElement)markerElement; // Explicit Cast
          }
          else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolyline)
          {
            //  Line elements
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Color = rgbColor;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 5;

            ILineElement lineElement = new LineElementClass();
            lineElement.Symbol = simpleLineSymbol;
            element = (IElement)lineElement; // Explicit Cast
          }
          else if ((geometry.GeometryType) == esriGeometryType.esriGeometryPolygon)
          {
            // Polygon elements
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
            simpleFillSymbol.Color = rgbColor;
            simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSForwardDiagonal;
            IFillShapeElement fillShapeElement = new PolygonElementClass();
            fillShapeElement.Symbol = simpleFillSymbol;
            element = (IElement)fillShapeElement; // Explicit Cast
          }
          if (!(element == null))
          {
            element.Geometry = geometry;
            graphicsContainer.AddElement(element, 0);
          }
        }
        #endregion
    }
}
