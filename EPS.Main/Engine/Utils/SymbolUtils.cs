using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System.Drawing;

namespace EPS.Engine.Utils
{
    public static class SymbolUtils
    {
        /// <summary>
        /// 获取指定图层，指定类型的符号
        /// </summary>
        /// <param name="pfLayer">图层</param>
        /// <param name="subtype">类型</param>
        /// <returns></returns>
        public static ISymbol GetLayerSymbol(IFeatureLayer pfLayer, string subtype)
        {
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;
            if (subtype == "")
            {
                ISimpleRenderer pSimpleRenderer = pfRenderer as ISimpleRenderer;
                if (pSimpleRenderer != null)
                    return pSimpleRenderer.Symbol;
            }

            IUniqueValueRenderer pUniqueRenderer = pfRenderer as IUniqueValueRenderer;
            if (pUniqueRenderer != null)
                return pUniqueRenderer.get_Symbol(subtype);

            return null;
        }

        /// <summary>
        /// 将符号转换为位图
        /// </summary>
        /// <param name="sym">符号</param>
        /// <param name="width">符号的宽</param>
        /// <param name="height">符号的高</param>
        /// <returns>位图</returns>
        public static Image Convert(ISymbol sym, int width, int height)
        {
            Image img = new Bitmap(width, height);
            Graphics gc = Graphics.FromImage(img);
            IntPtr hdc = gc.GetHdc();
            IEnvelope env = new EnvelopeClass();
            env.XMin = 0;
            env.XMax = width;
            env.YMin = 0;
            env.YMax = height;
            IGeometry geo = CreateGeometryFromSymbol(sym, env);
            if (geo != null)
            {
                ITransformation trans = CreateTransformationFromHDC(hdc, width, height);
                sym.SetupDC((int)hdc, trans);
                sym.Draw(geo);
                sym.ResetDC();
            }
            gc.ReleaseHdc(hdc);
            gc.Dispose();
            return img;
        }

        /// <summary>
        /// 将符号转换为16*16的位图
        /// </summary>
        /// <param name="sym">符号</param>
        /// <returns>位图</returns>
        public static Image Convert(ISymbol sym)
        {
            return Convert(sym, 16, 16);
        }

        /// <summary>
        /// 获取给定符号，给定范围符号的集合图形
        /// </summary>
        /// <param name="sym">符号</param>
        /// <param name="env">符号显示的范围</param>
        /// <returns>几何图形</returns>
        private static IGeometry CreateGeometryFromSymbol(ISymbol sym, IEnvelope env)
        {
            if (sym is IMarkerSymbol)
            {
                IArea area = (IArea)env;
                return (IGeometry)area.Centroid;
            }
            else if (sym is ILineSymbol || sym is ITextSymbol)
            {
                IPolyline line = new PolylineClass();
                IPoint pt = new PointClass();
                pt.PutCoords(env.LowerLeft.X, (env.LowerLeft.Y + env.UpperRight.Y) / 2);
                line.FromPoint = pt;
                pt = new PointClass();
                pt.PutCoords(env.UpperRight.X, (env.LowerLeft.Y + env.UpperRight.Y) / 2);
                line.ToPoint = pt;
                return (IGeometry)line;
            }
            else if (sym is IFillSymbol)
            {
                IPolygon polygon = new PolygonClass();
                IPointCollection ptCol = (IPointCollection)polygon;
                IPoint pt = new PointClass();
                pt.PutCoords(env.LowerLeft.X, env.LowerLeft.Y);
                ptCol.AddPoints(1, ref pt);
                pt.PutCoords(env.UpperLeft.X, env.UpperLeft.Y);
                ptCol.AddPoints(1, ref pt);
                pt.PutCoords(env.UpperRight.X, env.UpperRight.Y);
                ptCol.AddPoints(1, ref pt);
                pt.PutCoords(env.LowerRight.X, env.LowerRight.Y);
                ptCol.AddPoints(1, ref pt);
                pt.PutCoords(env.LowerLeft.X, env.LowerLeft.Y);
                ptCol.AddPoints(1, ref pt);
                return (IGeometry)polygon;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("找到一种特殊的符号!");
                return null;
            }
        }

        /// <summary>
        /// 根据指定设备上下文，指定的宽高创建投影变换
        /// </summary>
        /// <param name="HDC">设备上下文</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns>投影变换</returns>
        private static ITransformation CreateTransformationFromHDC(IntPtr HDC, int width, int height)
        {
            IEnvelope env = new EnvelopeClass();
            env.PutCoords(0, 0, width, height);
            tagRECT frame = new tagRECT();
            frame.left = 0;
            frame.top = 0;
            frame.right = width;
            frame.bottom = height;
            double dpi = Graphics.FromHdc(HDC).DpiY;
            long lDpi = (long)dpi;
            if (lDpi == 0)
            {
                System.Windows.Forms.MessageBox.Show("获取设备比例尺失败!");
                return null;
            }
            IDisplayTransformation dispTrans = new DisplayTransformationClass();
            dispTrans.Bounds = env;
            dispTrans.VisibleBounds = env;
            dispTrans.set_DeviceFrame(ref frame);
            dispTrans.Resolution = dpi;
            return dispTrans;
        }

        /// <summary>
        /// 判断给定标志值(e.g. 1,0)是否属于要素对应图层的标志值
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sValue">标志值</param>
        /// <returns>是否属于</returns>
        public static bool UniueValueEqual(IFeature pFeature, string sValue)
        {
            IFeatureLayer pfLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;
            string[] arrsField = null;
            if (GetUniqueFields(pfLayer, out arrsField) && arrsField != null)
            {
                string[] arrsValue = sValue.Split(',');
                int nField = arrsField.Length;
                string sFieldValue = null;
                for (int i = 0; i < nField; i++)
                {
                    GeoDbUtils.GetFieldValue(pFeature, arrsField[i], ref sFieldValue);
                    if (sFieldValue != arrsValue[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据指定的图层，指定的标志值获取对应的标志名称
        /// </summary>
        /// <param name="pfLayer">图层</param>
        /// <param name="sUniqueValue">标志值</param>
        /// <returns>标志名称</returns>
        public static string GetUniqueLabel(IFeatureLayer pfLayer, string sUniqueValue)
        {
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;
            if (pfRenderer is IUniqueValueRenderer)
            {
                IUniqueValueRenderer pUVRenderer = pfRenderer as IUniqueValueRenderer;
                int nValueCount = pUVRenderer.ValueCount;
                string sValue = null;
                for (int i = 0; i < nValueCount; i++)
                {
                    sValue = pUVRenderer.get_Value(i); // 如 "1,1" "0|1"
                    if (sValue == sUniqueValue)
                    {
                        return pUVRenderer.get_Label(sUniqueValue);
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 获取指定图层的所有标志值
        /// </summary>
        /// <param name="pfLayer">图层</param>
        /// <param name="sArrValues">所有标志值</param>
        /// <returns>是否存在分层字段</returns>
        public static bool GetUniqueValues(IFeatureLayer pfLayer, out string[] sArrValues)
        {
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;

            if (pfRenderer is IUniqueValueRenderer)
            {
                IUniqueValueRenderer pUVRenderer = pfRenderer as IUniqueValueRenderer;
                int nValueCount = pUVRenderer.ValueCount;
                sArrValues = new string[nValueCount];
                for (int i = 0; i < nValueCount; i++)
                {
                    sArrValues[i] = pUVRenderer.get_Value(i); // 如 "1,1" "0|1"
                }
                return true;
            }
            sArrValues = null;
            return false;
        }

        /// <summary>
        /// 根据指定的标志名称，获取图层中对应的标志值
        /// </summary>
        /// <param name="pfLayer">图层</param>
        /// <param name="sLabel">标志名称</param>
        /// <returns>标志值</returns>
        public static string GetUniqueValue(IFeatureLayer pfLayer, string sLabel)
        {
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;
            if (pfRenderer is IUniqueValueRenderer)
            {
                IUniqueValueRenderer pUVRenderer = pfRenderer as IUniqueValueRenderer;
                int nValueCount = pUVRenderer.ValueCount;
                string sValue = null;
                for (int i = 0; i < nValueCount; i++)
                {
                    sValue = pUVRenderer.get_Value(i); // 如 "1,1" "0|1"
                    if (sLabel == pUVRenderer.get_Label(sValue))
                    {
                        return sValue;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 获取图层对应的所有标志字段
        /// </summary>
        /// <param name="pfLayer">图层</param>
        /// <param name="sArrFields">所有标志字段</param>
        /// <returns>是否存在分层字段</returns>
        public static bool GetUniqueFields(IFeatureLayer pfLayer, out string[] sArrFields)
        {
            IGeoFeatureLayer pGeoFLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;

            if (pfRenderer is IUniqueValueRenderer)
            {
                IUniqueValueRenderer pUVRenderer = pfRenderer as IUniqueValueRenderer;
                int nFieldCount = pUVRenderer.FieldCount;
                sArrFields = new string[nFieldCount];
                // 一般nFieldCount为1，在特殊情况下为多个域，此时value的值由分割符',' 或'|'等分割
                for (int i = 0; i < nFieldCount; i++)
                {
                    sArrFields[i] = pUVRenderer.get_Field(i);
                }
                return true;
            }
            sArrFields = null;
            return false;
        }

        /// <summary>
        /// 根据指定要素获取要素对应的图层的全部标志字段和标志值
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sArrField">全部标志字段</param>
        /// <param name="nArrValue">全部标志值</param>
        /// <returns>是否存在分层字段</returns>
        public static bool GetUniqueFieldValue(IFeature pFeature, out string[] sArrField, out string[] nArrValue)
        {
            IFeatureLayer pfLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            if (pfLayer != null && GetUniqueFields(pfLayer, out sArrField) == true)
            {
                if (sArrField != null && sArrField.Length > 0)
                {
                    nArrValue = new string[sArrField.Length];
                    for (int i = 0; i < sArrField.Length; i++)
                    {
                        object o = GeoDbUtils.GetFieldValue(pFeature, sArrField[i]);
                        if (o == null)
                        {
                            nArrValue[i] = "0";
                        }
                        else
                        {
                            nArrValue[i] = o.ToString();
                        }
                    }
                    return true;
                }
            }
            sArrField = null;
            nArrValue = null;
            return false;
        }

        /// <summary>
        /// 设置指定要素的标志值
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="arrsField">标志字段</param>
        /// <param name="sValue">标志值（分隔符为','）</param>
        public static void SetUniqueValue(IFeature pFeature, string[] arrsField, string sValue)
        {
            int nField = arrsField.Length;
            if (nField > 0)
            {
                string[] arrsValue = sValue.Split(',');
                for (int i = 0; i < nField; i++)
                {
                    GeoDbUtils.SetFieldValue(pFeature, arrsField[i], arrsValue[i]);
                }
                pFeature.Store();
            }
        }

        /// <summary>
        /// 根据rgb值创建颜色
        /// </summary>
        /// <param name="nRed">红分量</param>
        /// <param name="nGreen">绿分量</param>
        /// <param name="nBlue">蓝分量</param>
        /// <returns>颜色接口</returns>
        public static IColor CreateColor(int nRed, int nGreen, int nBlue)
        {
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = nRed;
            pRgbColor.Green = nGreen;
            pRgbColor.Blue = nBlue;
            return (IColor)pRgbColor;
        }

        /// <summary>
        /// 如果地图的参考比例尺被设定,该函数起作用,将指定的像素转换为地图的范围
        /// </summary>
        /// <param name="pMap">地图接口</param>
        /// <param name="pixels">像素</param>
        /// <returns>地图对应的范围</returns>
        public static double ConvertPixels2ReferenceScaleMapUnits(IMap pMap, int pixels)
        {
            IActiveView pView = (IActiveView)pMap;
            if (pMap.ReferenceScale > 0)
            {
                IDisplayTransformation pDT = pView.ScreenDisplay.DisplayTransformation;
                tagRECT deviceRECT = pDT.get_DeviceFrame();
                int pixelExtent = deviceRECT.right - deviceRECT.left;
                IEnvelope pEnv = pDT.VisibleBounds;
                double realWorldDisplayExtent = pEnv.Width;
                double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
                return pixels * sizeOfOnePixel;
            }
            return pixels;
        }
    }
}
