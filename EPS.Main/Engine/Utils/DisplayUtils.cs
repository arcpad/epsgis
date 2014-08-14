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
using EPS.Engine.Utils;

namespace EPS.EPS.Engine.Utils
{
    public static class DisplayUtils
    {
        /// <summary>
        /// 清空地图要素选集
        /// </summary>
        /// <param name="pEditor">当前编辑接口</param>
        /// <returns>无效区域（需要重绘）</returns>
        //public static IInvalidArea3 ClearSelection(IEditor pEditor)
        //{
        //    IInvalidArea3 pInvalidArea = new InvalidAreaClass();
        //    pInvalidArea.Display = pEditor.Display;
        //    IEnumFeature pEnum = (IEnumFeature)pEditor.Map.FeatureSelection;
        //    IFeature pFeature = null;
        //    while ((pFeature = pEnum.Next()) != null) { pInvalidArea.Add(pFeature); }
        //    pEditor.Map.ClearSelection();
        //    return pInvalidArea;
        //}

        /// <summary>
        /// 设置旋转要素所在图层的旋转字段,建议使用mxd设置
        /// </summary>
        /// <param name="sRotationField">字段</param>
        public static void SetRotateRenderer(string sRotationField)
        {
            IEnumLayer pEnumLayer = GeoDbUtils.GetFeatureLayers(true);
            IFeatureLayer pfLayer = null;
            while ((pfLayer = (IFeatureLayer)pEnumLayer.Next()) != null)
            {
                IFeatureClass pfClass = pfLayer.FeatureClass;
                if (pfClass != null && pfClass.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    if (pfClass.FindField(sRotationField) != -1)
                    {
                        IGeoFeatureLayer pGeofLayer = (IGeoFeatureLayer)pfLayer;
                        IRotationRenderer pRotRenderer = (IRotationRenderer)pGeofLayer.Renderer;
                        pRotRenderer.RotationField = sRotationField;
                        pRotRenderer.RotationType = esriSymbolRotationType.esriRotateSymbolArithmetic;
                    }
                }
            }
        }

        public enum epcLineSymbol { epcNull, epcSimple, epcCartoGraphic, epcPicture, epcMultiLayer }

        /// <summary>
        /// 获取指定要素的符号
        /// </summary>
        /// <param name="pFeature"></param>
        /// <returns></returns>
        public static IMarkerSymbol GetMarkSymbol(IFeature pFeature)
        {
            IFeatureClass pfClass = pFeature.Class as IFeatureClass;
            IGeoFeatureLayer pGeofLayer = GeoDbUtils.GetFeatureLayer(pfClass.AliasName, true) as IGeoFeatureLayer;
            //IGeoFeatureLayer pGeofLayer = pLayer as IGeoFeatureLayer;
            IFeatureRenderer pFRenderer = pGeofLayer.Renderer;
            ISymbol pSymbol = pFRenderer.get_SymbolByFeature(pFeature);
            IMarkerSymbol pMarkerSymbol = pSymbol as IMarkerSymbol;
            return pMarkerSymbol;
        }

        /// <summary>
        /// 根据线要素,及图层获取符号,及线类型
        /// </summary>
        /// <param name="pFeature">线要素</param>
        /// <param name="pLayer">图层</param>
        /// <param name="linetype">线类新</param>
        /// <returns>符号</returns>
        public static ILineSymbol GetLineSymbol(IFeature pFeature, IFeatureLayer pLayer, out epcLineSymbol linetype)
        {
            IGeoFeatureLayer pGeofLayer = pLayer as IGeoFeatureLayer;
            IFeatureRenderer pFRenderer = pGeofLayer.Renderer;
            ISymbol pSymbol = pFRenderer.get_SymbolByFeature(pFeature);

            linetype = epcLineSymbol.epcNull;
            if (pSymbol is ISimpleLineSymbol)
                linetype = epcLineSymbol.epcSimple;
            else if (pSymbol is ICartographicLineSymbol)
                linetype = epcLineSymbol.epcCartoGraphic;
            else if (pSymbol is IPictureLineSymbol)
                linetype = epcLineSymbol.epcPicture;
            else if (pSymbol is IMultiLayerLineSymbol)
                linetype = epcLineSymbol.epcMultiLayer;

            if (linetype == epcLineSymbol.epcNull)
                return null;

            return pSymbol as ILineSymbol;
        }

        /// <summary>
        /// 获取线要素的符号
        /// </summary>
        /// <param name="pFeature">线要素</param>
        /// <returns>符号</returns>
        public static ILineSymbol GetLineSymbol(IFeature pFeature)
        {
            IFeatureLayer pfLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            IGeoFeatureLayer pGeofLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pFRenderer = pGeofLayer.Renderer;
            ISymbol pSymbol = pFRenderer.get_SymbolByFeature(pFeature);
            return pSymbol as ILineSymbol;
        }

        /// <summary>
        /// 获取面的符号
        /// </summary>
        /// <param name="pFeature">面要素</param>
        /// <returns>符号</returns>
        public static IFillSymbol GetFillSymbol(IFeature pFeature)
        {
            IFeatureLayer pfLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            IGeoFeatureLayer pGeofLayer = pfLayer as IGeoFeatureLayer;
            IFeatureRenderer pFRenderer = pGeofLayer.Renderer;
            ISymbol pSymbol = pFRenderer.get_SymbolByFeature(pFeature);
            return pSymbol as IFillSymbol;
        }

        /// <summary>
        /// 根据要素获取符号
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <returns>符号</returns>
        public static ISymbol GetSymbol(IFeature pFeature)
        {
            IFeatureLayer pfLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            if (pfLayer != null && pfLayer is IGeoFeatureLayer)
            {
                IGeoFeatureLayer pGeoFLayer = (IGeoFeatureLayer)pfLayer;
                IFeatureRenderer pfRenderer = pGeoFLayer.Renderer;
                return pfRenderer.get_SymbolByFeature(pFeature);
            }
            return null;
        }

        /// <summary>
        /// 刷新要素
        /// </summary>
        /// <param name="pActiveView">活动视口</param>
        /// <param name="pFeature">要素</param>
        public static void FeatureRefresh(IActiveView pActiveView, IFeature pFeature)
        {
            IGeometry pGeom = pFeature.Shape;
            IFeatureLayer pFeatureLayer = GeoDbUtils.GetFeatureLayer(pFeature);
            if (pGeom.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                double length;
                length = GeometryUtils.ConvertPixelsToMapUnits(pActiveView, 30);
                ITopologicalOperator pTopo = (ITopologicalOperator)pGeom;
                IGeometry pBuffer = pTopo.Buffer(length);
                pActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), pFeatureLayer, pBuffer.Envelope);
            }
            else
            {
                pActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), pFeatureLayer, pGeom.Envelope);
            }
        }

        /// <summary>
        /// 刷新要素
        /// </summary>
        /// <param name="pActiveView">活动视口</param>
        /// <param name="pFeature">要素</param>
        /// <param name="pfLayer">要素图层</param>
        public static void FeatureRefresh(IActiveView pActiveView, IFeature pFeature, IFeatureLayer pfLayer)
        {
            IGeometry pGeom = pFeature.Shape;
            if (pGeom.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                double length;
                length = GeometryUtils.ConvertPixelsToMapUnits(pActiveView, 30);
                ITopologicalOperator pTopo = (ITopologicalOperator)pGeom;
                IGeometry pBuffer = pTopo.Buffer(length);
                pActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), pfLayer, pBuffer.Envelope);
            }
            else
            {
                pActiveView.PartialRefresh((esriViewDrawPhase)(esriDrawPhase.esriDPGeography | esriDrawPhase.esriDPSelection), pfLayer, pGeom.Envelope);
            }
        }
    }
}
