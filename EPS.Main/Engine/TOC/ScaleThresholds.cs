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
using ESRI.ArcGIS.SystemUI;

namespace EPS.Engine.TOC
{
	public class ScaleThresholds : BaseCommand, ICommandSubType
	{
		private IMapControl3 m_mapControl;
		private long m_subType;

		public ScaleThresholds()
		{

		}
	
		public override void OnClick()
		{
			ILayer layer = (ILayer) m_mapControl.CustomProperty;
			if (m_subType == 1) layer.MaximumScale = m_mapControl.MapScale;
			if (m_subType == 2) layer.MinimumScale = m_mapControl.MapScale;
			if (m_subType == 3)
			{
				layer.MaximumScale = 0;
				layer.MinimumScale = 0;
			}
			m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography,null,null);
		}
	
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;
		}
	
		public int GetCount()
		{
			return 3;
		}
	
		public void SetSubType(int SubType)
		{
			m_subType = SubType;
		}
	
		public override string Caption
		{
			get
			{
				if (m_subType == 1) return "设置最大比例尺";
				else if (m_subType == 2) return "设置最小比例尺";
				else return "取消比例尺阀值限制";
			}
		}
	
		public override bool Enabled
		{
			get
			{
				bool enabled = true;
				ILayer layer = (ILayer) m_mapControl.CustomProperty;
				
				if (m_subType == 3)
				{
					if ((layer.MaximumScale == 0) & (layer.MinimumScale == 0)) enabled = false;
				}
				return enabled;
			}
		}
	}
}

