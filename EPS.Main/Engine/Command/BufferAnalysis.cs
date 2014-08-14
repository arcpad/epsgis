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
using EPS.Tools;

namespace EPS.Engine.TOC
{
	public sealed class BufferAnalysis : BaseCommand  
	{
		private IMapControl3 m_mapControl;

        private IHookHelper m_hookHelper;

        public BufferAnalysis()
		{
			base.m_caption = "»º´æÇø·ÖÎö";
		}
	
		public override void OnClick()
		{
            BufferAnalysisForm form = new BufferAnalysisForm(m_hookHelper);
            form.ShowDialog();
		}
	
		public override void OnCreate(object hook)
		{
			m_mapControl = (IMapControl3) hook;

            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
		}
	}
}

