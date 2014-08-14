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

namespace EPS.Engine.TOC
{
	public sealed class LayerProperties : BaseCommand  
	{
		private IMapControl3 m_mapControl;

        public LayerProperties()
		{
			base.m_caption = "Õº≤„ Ù–‘";
		}
	
		public override void OnClick()
		{
			ILayer layer = (ILayer) m_mapControl.CustomProperty;
            if (layer is IGeoFeatureLayer)
            {
                LayerPropertiesForm form = new LayerPropertiesForm();
                form.Layer = layer;
                form.ShowDialog();
            }

			// m_mapControl.Extent = layer.AreaOfInterest;
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

