using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using EPS.Main;
using ESRI.ArcGIS.Geometry;

namespace EPS.Engine.Utils
{
    /// <summary>
    /// Map utilities for current application
    /// </summary>
    public static class MapUtils
    {
        public static IActiveView GetActiveView() {
            return MainForm.Instance.MapControl.ActiveView;
        }

        public static IMap GetMap() {
            return MainForm.Instance.MapControl.ActiveView.FocusMap;
        }
    }
}
