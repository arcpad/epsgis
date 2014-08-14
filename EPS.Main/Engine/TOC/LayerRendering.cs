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

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESP.Main.Engine.Toc.Renderer;
using EPS.Engine.Utils;
using System.Windows.Forms;
using EPS.Main;

namespace EPS.Engine.TOC
{
	public sealed class LayerRendering : BaseCommand  
	{
		private IMapControl3 m_mapControl;

        public LayerRendering()
		{
			base.m_caption = "Í¼ÀýäÖÈ¾";
		}
	
		public override void OnClick()
		{
			ILayer layer = (ILayer) m_mapControl.CustomProperty;
			// m_mapControl.Extent = layer.AreaOfInterest;

            //IBasicMap map = new MapClass();
            //ILayer layer = new FeatureLayerClass();
            //object other = new Object();
            //object index = new Object();
            //esriTOCControlItem item = new esriTOCControlItem();

            //Determine what kind of item has been clicked on
            // axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);

            //QI to IFeatureLayer and IGeoFeatuerLayer interface
            // if (layer == null) return;
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            if (featureLayer == null) {
                return;
            } 
            IGeoFeatureLayer geoFeatureLayer = (IGeoFeatureLayer)featureLayer;
            ISimpleRenderer simpleRenderer = null;
            if (geoFeatureLayer.Renderer is ISimpleRenderer)
            {
                simpleRenderer = (ISimpleRenderer)geoFeatureLayer.Renderer;
            }
            else
            {
                MessageBox.Show("demo½öÖ§³Ö¼òµ¥Í¼Àý");
                return;
            }

            //Create the form with the SymbologyControl
            RendererForm symbolForm = new RendererForm();

            //Get the IStyleGalleryItem
            IStyleGalleryItem styleGalleryItem = null;
            //Select SymbologyStyleClass based upon feature type
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassMarkerSymbols, simpleRenderer.Symbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassLineSymbols, simpleRenderer.Symbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    styleGalleryItem = symbolForm.GetItem(esriSymbologyStyleClass.esriStyleClassFillSymbols, simpleRenderer.Symbol);
                    break;
            }

            // Release the form
            // symbolForm.Close();
            symbolForm.Dispose();
            MainForm.Instance.Activate();

            if (styleGalleryItem == null) return;

            //Create a new renderer
            simpleRenderer = new SimpleRendererClass();
            //Set its symbol from the styleGalleryItem
            simpleRenderer.Symbol = (ISymbol)styleGalleryItem.Item;
            //Set the renderer into the geoFeatureLayer
            geoFeatureLayer.Renderer = (IFeatureRenderer)simpleRenderer;

            //Fire contents changed event that the TOCControl listens to
            IActiveView activeView = MapUtils.GetActiveView();
            activeView.ContentsChanged();
            activeView.Refresh();
            // axPageLayoutControl1.ActiveView.ContentsChanged();
            // Refresh the display
            // axPageLayoutControl1.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
		}
	
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;
		}

        public override bool Enabled
        {
            get
            {
                ILayer layer = (ILayer)m_mapControl.CustomProperty;
                return layer is IGeoFeatureLayer;
            }
        }
	}
}

