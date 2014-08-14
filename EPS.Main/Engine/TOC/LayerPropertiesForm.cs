using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

using EPS.Utilities;
using EPS.Engine.Utils;

namespace EPS.Engine.TOC
{
    /// <summary>
    /// 图层属性设置
    /// </summary>
    public partial class LayerPropertiesForm : Form
    {
        private ILayer m_layer;

        public ILayer Layer
        {
            get { return m_layer; }
            set 
            {
                m_layer = value;
                // Initialize();
            }
        }

        /// <summary>
        /// 初始化form element
        /// </summary>
        private void Initialize()
        {
            IFeatureLayer featureLayer = (IFeatureLayer)m_layer;
            IGeoFeatureLayer geoLayer = (IGeoFeatureLayer)m_layer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            ILayerGeneralProperties layerProperties = (ILayerGeneralProperties)featureLayer;

            // 图层属性页
            this.textBoxLayerName.Text = m_layer.Name;
            this.textBoxLayerDescription.Text = layerProperties.LayerDescription;
            this.checkBoxVisibility.Checked = m_layer.Visible;

            String scaleMin = geoLayer.MinimumScale.ToString();
            String sacleMax = geoLayer.MaximumScale.ToString();
            if ("0" == scaleMin && "0" == sacleMax)
            {
                this.radioButtonScaleAll.Checked = true;
                this.radioButtonScaleZoom.Checked = false;
            }
            else
            {
                this.radioButtonScaleAll.Checked = false;
                this.radioButtonScaleZoom.Checked = true;
                this.comboBoxScaleMax.Text = sacleMax;
                this.comboBoxScaleMin.Text = scaleMin;
            }

            // 字段定义页
            IFields fields = featureClass.Fields;
            int fieldCount = fields.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                IField field = fields.get_Field(i);
                ListViewItem item = new ListViewItem(field.Name);
                item.SubItems.Add(field.AliasName);
                item.SubItems.Add(field.Editable ? "允许编辑" : "不允许编辑");
                item.SubItems.Add(field.IsNullable ? "允许为空" : "不允许为空");
                item.SubItems.Add(field.Length.ToString());
                item.SubItems.Add(field.Precision.ToString());
                item.SubItems.Add(field.Scale.ToString());
                item.SubItems.Add(GetFieldType(field.Type));
                listViewFields.Items.Add(item);
            }

            // 数据源及四至范围
            IEnvelope envelope = m_layer.AreaOfInterest;
            textBoxLeft.Text = envelope.XMin.ToString();
            textBoxRight.Text = envelope.XMax.ToString();
            textBoxTop.Text = envelope.YMin.ToString();
            textBoxBottom.Text = envelope.YMax.ToString();

            IDataset dataset = (IDataset)featureLayer;
            textBoxDataSource.AppendText("数据源类型：           ");
            textBoxDataSource.AppendText(featureLayer.DataSourceType);
            textBoxDataSource.AppendText("\r\n数据集：               ");
            textBoxDataSource.AppendText(dataset.BrowseName);
            textBoxDataSource.AppendText("\r\n数据源名称：           ");
            textBoxDataSource.AppendText(dataset.Workspace.PathName);

            textBoxDataSource.AppendText("\r\n要素类：               ");
            textBoxDataSource.AppendText(featureClass.AliasName);
            textBoxDataSource.AppendText("\r\n要素类类型：           ");
            textBoxDataSource.AppendText(GetFeatureType(featureClass.FeatureType));
            textBoxDataSource.AppendText("\r\n几何类型：             ");
            textBoxDataSource.AppendText(GetGeometryType(featureClass.ShapeType));

            IGeoDataset geoDataset = (IGeoDataset)featureClass;
            // 通过IGeoDataset接口获取FeatureClass坐标系统
            ISpatialReference spatialReference = geoDataset.SpatialReference;
            IProjectedCoordinateSystem projectCoordSystem = null;
            if (spatialReference is IProjectedCoordinateSystem)
            {
                projectCoordSystem = (IProjectedCoordinateSystem)spatialReference;

                if (projectCoordSystem == null)
                {
                    return;
                }

                textBoxDataSource.AppendText("\r\n");

                IProjection project = projectCoordSystem.Projection;
                textBoxDataSource.AppendText("\r\n投影坐标系：           ");
                textBoxDataSource.AppendText(projectCoordSystem.Name);
                textBoxDataSource.AppendText("\r\n投影：                 ");
                textBoxDataSource.AppendText(project.Name);
                textBoxDataSource.AppendText("\r\nFalseEasting：         ");
                textBoxDataSource.AppendText(projectCoordSystem.FalseEasting.ToString());
                textBoxDataSource.AppendText("\r\nFalseNorthing：        ");
                textBoxDataSource.AppendText(projectCoordSystem.FalseNorthing.ToString());
                textBoxDataSource.AppendText("\r\n中央经线：             ");
                textBoxDataSource.AppendText(projectCoordSystem.get_CentralMeridian(true).ToString());
                textBoxDataSource.AppendText("\r\n缩放比例：             ");
                textBoxDataSource.AppendText(projectCoordSystem.ScaleFactor.ToString());
                textBoxDataSource.AppendText("\r\n高度原点：             ");
                try
                {
                    textBoxDataSource.AppendText(projectCoordSystem.LongitudeOfOrigin.ToString());
                }
                catch (Exception e)
                {
                    textBoxDataSource.AppendText("0");
                }

                textBoxDataSource.AppendText("\r\n单位：                 ");
                textBoxDataSource.AppendText(projectCoordSystem.CoordinateUnit.Name);

                textBoxDataSource.AppendText("\r\n");

                IGeographicCoordinateSystem geographCoordinateSystem = projectCoordSystem.GeographicCoordinateSystem;
                if (geographCoordinateSystem != null)
                {
                    textBoxDataSource.AppendText("\r\n地理坐标系：           ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.Name);
                    textBoxDataSource.AppendText("\r\n基准面：               ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.Datum.Name);
                    textBoxDataSource.AppendText("\r\n本初子午线：           ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.PrimeMeridian.Name);
                    textBoxDataSource.AppendText("\r\n单位：                 ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.CoordinateUnit.Name);
                }
            }
            else if (spatialReference is IGeographicCoordinateSystem)
            {
                IGeographicCoordinateSystem geographCoordinateSystem = spatialReference as IGeographicCoordinateSystem;
                if (geographCoordinateSystem != null)
                {
                    textBoxDataSource.AppendText("\r\n地理坐标系：           ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.Name);
                    textBoxDataSource.AppendText("\r\n基准面：               ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.Datum.Name);
                    textBoxDataSource.AppendText("\r\n本初子午线：           ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.PrimeMeridian.Name);
                    textBoxDataSource.AppendText("\r\n单位：                 ");
                    textBoxDataSource.AppendText(geographCoordinateSystem.CoordinateUnit.Name);
                }
            }

            

            // filter

            IDisplayString displayString = (IDisplayString)featureLayer;
            textBoxFilter.AppendText(displayString.ExpressionProperties.Expression);

            // 标注
            IAnnotateLayerPropertiesCollection annoLayerPropsColl = geoLayer.AnnotationProperties;
            IAnnotateLayerProperties annoLayerProps = null;
            IElementCollection placedElements = null;
            IElementCollection unplacedElements = null;
            annoLayerPropsColl.QueryItem(0, out annoLayerProps, out placedElements, out unplacedElements);
            ILabelEngineLayerProperties aLELayerProps = annoLayerProps as ILabelEngineLayerProperties;
            // annoLayerProps.DisplayAnnotation;
            checkBoxLabelVisibility.Checked = annoLayerProps.DisplayAnnotation;

            //初始化字体大小下拉框
            for (int k = 5; k <= 11; k++)
            {
                cmbFontSize.Items.Add(k);
            }
            for (int k = 12; k <= 72; k = k + 2)
            {
                cmbFontSize.Items.Add(k);
            }
            // cmbFontSize.Text = "8";

            //初始化字体下拉框
            foreach (FontFamily onefontfamily in FontFamily.Families)
            {
                //去掉名称头个字为@的字体
                if (onefontfamily.Name.Substring(0, 1) != "@")
                {
                    cmbFontName.Items.Add(onefontfamily.Name);
                }
            }
            // cmbFontName.Text = "宋体";
            ITextSymbol pTextSymbol = aLELayerProps.Symbol;
            stdole.IFontDisp pFontDisp = pTextSymbol.Font;
            cmbFontSize.Text = pFontDisp.Size.ToString();
            toolBarStyle.Buttons[0].Pushed = pFontDisp.Bold;
            toolBarStyle.Buttons[1].Pushed = pFontDisp.Italic;
            toolBarStyle.Buttons[2].Pushed = pFontDisp.Underline;
            cmbFontName.Text = pFontDisp.Name;
            IRgbColor rgbColor = (IRgbColor)pTextSymbol.Color;

            colorButtonFont.Color = Color.FromArgb(rgbColor.Transparency,
                rgbColor.Red,
                rgbColor.Green,
                rgbColor.Blue);
            colorButtonFont.Refresh();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField field = fields.get_Field(i);
                cmbFieldName.Items.Add(field.Name);
            }

            cmbFieldName.Enabled = aLELayerProps.IsExpressionSimple;
            String expr = aLELayerProps.Expression.Trim('[').Trim(']');
            cmbFieldName.SelectedIndex = cmbFieldName.FindString(expr);

            cmbMaxScale.Text = annoLayerProps.AnnotationMaximumScale.ToString();
            cmbMinScale.Text = annoLayerProps.AnnotationMinimumScale.ToString();

            if (cmbMaxScale.Text == "0" && cmbMinScale.Text == "0")
            {
                rbScaleWithLayer.Checked = true;
                this.rbScaleDefined.Checked = false;
            }
            else
            {
                rbScaleWithLayer.Checked = false;
                this.rbScaleDefined.Checked = true;
            }

            //ITextSymbol pTextSymbol = aLELayerProps.Symbol;
            //stdole.IFontDisp pFontDisp = pTextSymbol.Font;
            //pFontDisp.Size = decimal.Parse(fontSize);
            //pFontDisp.Bold = boldBool;
            //pFontDisp.Italic = italicBool;
            //pFontDisp.Name = fontStyle;
            //IRgbColor pRgbColor = new RgbColorClass();
            //pRgbColor.Red = int.Parse(fontColor.R.ToString());
            //pRgbColor.Blue = int.Parse(fontColor.B.ToString());
            //pRgbColor.Green = int.Parse(fontColor.G.ToString());
            //pTextSymbol.Font = pFontDisp;
            //pTextSymbol.Color = pRgbColor;
            //aLELayerProps.Symbol = pTextSymbol;
            //annoLayerProps = aLELayerProps as IAnnotateLayerProperties;
            //annoLayerProps.FeatureLayer = geoLayer;
            //annoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
            //annoLayerPropsColl.Add(annoLayerProps);

            // 符号
            checkBoxCostomSymbol.Checked = EPSUtils.IsLayerRenderer(featureLayer);
        }

        public String GetGeometryType(esriGeometryType geometryType) {
            switch(geometryType)
            {
                case esriGeometryType.esriGeometryNull:
                    return "NULL";
                case esriGeometryType.esriGeometryPoint:
                    return "点";
                case esriGeometryType.esriGeometryMultipoint:
                    return "多点";
                case esriGeometryType.esriGeometryPolyline:
                    return "多义线";
                case esriGeometryType.esriGeometryPolygon:
                    return "面";
                case esriGeometryType.esriGeometryEnvelope:
                    return "矩形";
                case esriGeometryType.esriGeometryPath:
                    return "路径";
                case esriGeometryType.esriGeometryAny:
                    return "Any";
                case esriGeometryType.esriGeometryMultiPatch:
                    return "MultiPatch";
                case esriGeometryType.esriGeometryRing:
                    return "环";
                case esriGeometryType.esriGeometryLine:
                    return "线";
                case esriGeometryType.esriGeometryCircularArc:
                    return "弧";
                case esriGeometryType.esriGeometryBezier3Curve:
                    return "贝塞尔曲线";
                case esriGeometryType.esriGeometryEllipticArc:
                    return "椭圆弧";
                case esriGeometryType.esriGeometryBag:
                    return "Bag";
                case esriGeometryType.esriGeometryTriangleStrip:
                    return "三角地带";
                case esriGeometryType.esriGeometryTriangleFan:
                    return "TriangleFan";
                case esriGeometryType.esriGeometryRay:
                    return "射线";
                case esriGeometryType.esriGeometrySphere:
                    return "球";
                case esriGeometryType.esriGeometryTriangles:
                    return "三角形";
                default:
                    return "";
            }
        }

        public String GetFeatureType(esriFeatureType featureType) 
        {
            switch (featureType)
            {
                case esriFeatureType.esriFTSimple:
                    return "简单要素";
                case esriFeatureType.esriFTSimpleJunction:
                    return "简单连接点";
                case esriFeatureType.esriFTComplexJunction:
                    return "复制连接点";
                case esriFeatureType.esriFTCoverageAnnotation:
                    return "Coverage注记";
                case esriFeatureType.esriFTDimension:
                    return "范围标注";
                case esriFeatureType.esriFTRasterCatalogItem:
                    return "栅格类";
                case esriFeatureType.esriFTAnnotation:
                    return "注记";
                case esriFeatureType.esriFTSimpleEdge:
                    return "简单接边";
                case esriFeatureType.esriFTComplexEdge:
                    return "复杂接边";
                default:
                    return "";
            }
        }

        public String GetFieldType(esriFieldType fieldType) {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "短整形";
                case esriFieldType.esriFieldTypeInteger:
                    return "长整形";
                case esriFieldType.esriFieldTypeSingle:
                    return "单精度";
                case esriFieldType.esriFieldTypeDouble:
                    return "双精度";
                case esriFieldType.esriFieldTypeString:
                    return "字符串";
                case esriFieldType.esriFieldTypeDate:
                    return "日期类型";
                case esriFieldType.esriFieldTypeOID:
                    return "系统ID";
                case esriFieldType.esriFieldTypeGeometry:
                    return "几何类型";
                case esriFieldType.esriFieldTypeBlob:
                    return "二进制类型";
                case esriFieldType.esriFieldTypeRaster:
                    return "栅格类型";
                case esriFieldType.esriFieldTypeGUID:
                    return "GUID";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "GlobalID";
                case esriFieldType.esriFieldTypeXML:
                    return "XML";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 设置图层
        /// </summary>
        private void DoApply()
        {
            IFeatureLayer featureLayer = (IFeatureLayer)m_layer;
            IGeoFeatureLayer geoLayer = (IGeoFeatureLayer)m_layer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            ILayerGeneralProperties layerProperties = (ILayerGeneralProperties)featureLayer;

            // 图层属性页
            String tmp = this.textBoxLayerName.Text.Trim();
            if (!String.IsNullOrEmpty(tmp))
            {
                m_layer.Name = tmp;
            }
            tmp = layerProperties.LayerDescription.Trim();
            layerProperties.LayerDescription = tmp;
            m_layer.Visible = this.checkBoxVisibility.Checked;

            if (this.radioButtonScaleAll.Checked)
            {
                geoLayer.MaximumScale = 0;
                geoLayer.MinimumScale = 0;
            }
            else
            {
                tmp = this.comboBoxScaleMax.Text.Trim();
                double d = 0;
                if (Double.TryParse(tmp, out d))
                {
                    geoLayer.MaximumScale = d;
                }
                else
                {
                    geoLayer.MaximumScale = 0;
                }
                tmp = this.comboBoxScaleMin.Text.Trim();
                if (Double.TryParse(tmp, out d))
                {
                    geoLayer.MinimumScale = d;
                }
                else 
                {
                    geoLayer.MinimumScale = 0;
                }
            }

            // 标注
            IAnnotateLayerPropertiesCollection annoLayerPropsColl = geoLayer.AnnotationProperties;
            IAnnotateLayerProperties annoLayerProps = null;
            IElementCollection placedElements = null;
            IElementCollection unplacedElements = null;
            annoLayerPropsColl.QueryItem(0, out annoLayerProps, out placedElements, out unplacedElements);
            ILabelEngineLayerProperties aLELayerProps = annoLayerProps as ILabelEngineLayerProperties;

            annoLayerProps.DisplayAnnotation = checkBoxLabelVisibility.Checked;
            if (annoLayerProps.DisplayAnnotation)
            {
                // cmbFontName.Text = "宋体";
                ITextSymbol textSymbol = aLELayerProps.Symbol;
                stdole.IFontDisp fontDisp = textSymbol.Font;
                decimal d = 8;
                if (decimal.TryParse(cmbFontSize.Text, out d))
                {
                    fontDisp.Size = d;
                }

                fontDisp.Bold = toolBarStyle.Buttons[0].Pushed;
                fontDisp.Italic = toolBarStyle.Buttons[1].Pushed;
                fontDisp.Underline = toolBarStyle.Buttons[2].Pushed;
                fontDisp.Name = cmbFontName.Text;
                IRgbColor rgbColor = new RgbColorClass();
                rgbColor.Transparency = this.colorButtonFont.Color.A;
                rgbColor.Red = this.colorButtonFont.Color.R;
                rgbColor.Green = this.colorButtonFont.Color.G;
                rgbColor.Blue = this.colorButtonFont.Color.B;
                textSymbol.Color = rgbColor;
                textSymbol.Font = fontDisp;
                aLELayerProps.Symbol = textSymbol;
                annoLayerProps = aLELayerProps as IAnnotateLayerProperties;
                annoLayerProps.FeatureLayer = geoLayer;
                annoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
                // annoLayerPropsColl.Add(annoLayerProps);

                if (cmbFieldName.SelectedIndex == -1)
                {
                    aLELayerProps.IsExpressionSimple = false;
                }
                else
                {
                    aLELayerProps.IsExpressionSimple = true;
                    String expr = "[" + cmbFieldName.Items[cmbFieldName.SelectedIndex] + "]";
                    aLELayerProps.Expression = expr;
                }

                if (this.rbScaleWithLayer.Checked)
                {
                    annoLayerProps.AnnotationMaximumScale = 0;
                    annoLayerProps.AnnotationMinimumScale = 0;
                }
                else
                {
                    double dTmp = 0;
                    if (double.TryParse(cmbMaxScale.Text, out dTmp))
                    {
                        annoLayerProps.AnnotationMaximumScale = dTmp;
                    }
                    dTmp = 0;
                    if (double.TryParse(cmbMinScale.Text, out dTmp))
                    {
                        annoLayerProps.AnnotationMinimumScale = dTmp;
                    }
                }
 

                //ITextSymbol pTextSymbol = aLELayerProps.Symbol;
                //stdole.IFontDisp pFontDisp = pTextSymbol.Font;
                //pFontDisp.Size = decimal.Parse(fontSize);
                //pFontDisp.Bold = boldBool;
                //pFontDisp.Italic = italicBool;
                //pFontDisp.Name = fontStyle;
                //IRgbColor pRgbColor = new RgbColorClass();
                //pRgbColor.Red = int.Parse(fontColor.R.ToString());
                //pRgbColor.Blue = int.Parse(fontColor.B.ToString());
                //pRgbColor.Green = int.Parse(fontColor.G.ToString());
                //pTextSymbol.Font = pFontDisp;
                //pTextSymbol.Color = pRgbColor;
                //aLELayerProps.Symbol = pTextSymbol;
                //annoLayerProps = aLELayerProps as IAnnotateLayerProperties;
                //annoLayerProps.FeatureLayer = geoLayer;
                //annoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
                //annoLayerPropsColl.Add(annoLayerProps);
            }

            // 符号
            EPSUtils.RendererFeatureLayer(featureLayer, checkBoxCostomSymbol.Checked);
            MapUtils.GetActiveView().Refresh();
        }

        public LayerPropertiesForm()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DoApply();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            DoApply();
        }

        private void LayerPropertiesForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        private void toolBarStyle_Click(object sender, EventArgs e)
        {
            ToolBarButton btn = (e as ToolBarButtonClickEventArgs).Button;
            btn.Pushed = !btn.Pushed;
        }

        private void buttonCurScaleMin_Click(object sender, EventArgs e)
        {
            comboBoxScaleMin.Text = MapUtils.GetActiveView().FocusMap.MapScale.ToString();
        }

        private void buttonCurScaleMax_Click(object sender, EventArgs e)
        {
            comboBoxScaleMax.Text = MapUtils.GetActiveView().FocusMap.MapScale.ToString();
        }
    }
}
