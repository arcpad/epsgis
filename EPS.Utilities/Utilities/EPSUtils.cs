using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using ssArcGISRender;
using ESRI.ArcGIS.Geometry;

namespace EPS.Utilities
{
    /// <summary>
    /// 符号化组件工具包
    /// </summary>
    public static class EPSUtils
    {
        /// <summary>
        /// 检入数据
        /// </summary>
        public static void CheckIn()
        {
            // EPS interface routine...
        }

        /// <summary>
        /// 检出数据
        /// </summary>
        /// <param name="envelope">检出范围</param>
        /// <param name="clause">检出条件</param>
        public static void CheckOut(IEnvelope envelope, String clause)
        {
            // EPS interface routine...
        }

        /// <summary>
        /// 是否自定义符号化指定图层（骨架线显示）
        /// </summary>
        /// <param name="fetureLayer"></param>
        /// <param name="isUseFeatureDraw">是否骨架线显示， true 骨架线 else 符号化</param>
        /// <returns></returns>
        public static void RendererFeatureLayer(IFeatureLayer fetureLayer, bool isUseFeatureDraw) 
        {
            IGeoFeatureLayer geoLayer = fetureLayer as IGeoFeatureLayer;
            if (geoLayer == null)
            {
                return;
            }

            IFeatureRenderer renderer = geoLayer.Renderer;
            IssArcGISRenderer ssRenderer = renderer as IssArcGISRenderer;
            if (ssRenderer != null)
            {
                ssRenderer.UseFeatureDraw = isUseFeatureDraw;
            }
        }

        /// <summary>
        /// 设置图层渲染方式
        /// </summary>
        /// <param name="fetureLayer"></param>
        /// <param name="annotationField"></param>
        public static void SetLayerRenderer(IFeatureLayer fetureLayer, String annotationField = "")
        {
            IGeoFeatureLayer geoLayer = fetureLayer as IGeoFeatureLayer;
            if (geoLayer == null)
            {
                return;
            }

            IFeatureRenderer renderer = geoLayer.Renderer;
            IssArcGISRenderer ssRenderer = new ssArcGISRendererClass();
            IFeatureClass ipFeatureClass = geoLayer.FeatureClass;
            // 设置图层类型 0 点 1 线 2 面 3 注记
            int nLayerFeatureType = 0;
            esriGeometryType stype = ipFeatureClass.ShapeType;

            if (stype == esriGeometryType.esriGeometryPoint || stype == esriGeometryType.esriGeometryMultipoint)
            {
                nLayerFeatureType = 0;
            }
            else if (stype == esriGeometryType.esriGeometryLine || stype == esriGeometryType.esriGeometryCircularArc ||
              stype == esriGeometryType.esriGeometryEllipticArc || stype == esriGeometryType.esriGeometryBezier3Curve ||
              stype == esriGeometryType.esriGeometryPath || stype == esriGeometryType.esriGeometryPolyline)
            {
                nLayerFeatureType = 1;
            }
            else
            {
                nLayerFeatureType = 2;
            }

            IFields ipFields = ipFeatureClass.Fields;
            // 判断是否为注记图层
            long textFieldIndex = -1;
            if (String.IsNullOrEmpty(annotationField))
            {
                annotationField = "注记内容";
            }

            textFieldIndex = ipFields.FindField(annotationField);
            if (nLayerFeatureType == 1 && textFieldIndex > -1)
            {
                nLayerFeatureType = 3;
            }

            ssRenderer.LayerFeatureType = nLayerFeatureType;

            // 设置符号显示视野区间(米)
            ssRenderer.SetSymbolLevel(0, 500);
            // 设置符号化模板名称
            ssRenderer.TemplateFileName = "广州基础地理模板_500.mdt";
            // 设置要素代码字段名
            ssRenderer.CodeFieldName = "要素代码";
            // 设置图形特征字段名
            ssRenderer.GraphicInfoFieldName = "图形特征";
            // 设置注记内容字段名
            ssRenderer.TextFieldName = "注记内容";
            // 设置控制点点名字段名
            ssRenderer.PointNameFieldName = "点名";
            // 设置高程字段名
            ssRenderer.ZFieldName = "高程";

            // 设置其它符号化所需字段名(^号分隔)
            ssRenderer.OhterUseFieldNames = "";
            // 设置符号模板字段名称与ArcGIS字段名称对照,格式: 模板字段1:ArcGIS字段1;模板字段2:ArcGIS字段2;...
            ssRenderer.FieldNameRelation = "";
            // 设置符号化显示状态 VARIANT_FALSE 骨架线 VARIANT_TRUE 符号化
            ssRenderer.UseFeatureDraw = true;
            // 设置是否使用符号化缓存 0 不使用 1 使用
            ssRenderer.SetUseCacheStatus(1);
            // 设置颜色使用状态 0 随符号描述 1 随实体 2 自定义
            ssRenderer.SetUseColorStatus(0);
            // 设置自定义颜色,只有颜色使用状态为2时有效
            ssRenderer.SetDrawObjCustomColor(0);

            ssRenderer.FeatureLayer = geoLayer;

            ssRenderer.FeatureClass = geoLayer.FeatureClass;
            ssRenderer.LayerName = geoLayer.Name;
            ISimpleRenderer simpleRenderer = (ISimpleRenderer)renderer;
            ssRenderer.Symbol = simpleRenderer.Symbol;
            // 替换渲染器
            geoLayer.Renderer = (ESRI.ArcGIS.Carto.IFeatureRenderer)ssRenderer;
        }

        /// <summary>
        /// 是否自定义EPS符号化显示
        /// </summary>
        /// <param name="fetureLayer"></param>
        /// <returns></returns>
        public static bool IsLayerRenderer(IFeatureLayer fetureLayer)
        {
            IGeoFeatureLayer geoLayer = fetureLayer as IGeoFeatureLayer;
            if (geoLayer == null)
            {
                return false;
            }

            IFeatureRenderer renderer = geoLayer.Renderer;
            IssArcGISRenderer ssRenderer = renderer as IssArcGISRenderer;
            if (ssRenderer != null)
            {
                return ssRenderer.UseFeatureDraw;
            }
            return false;
        }
    }
}
