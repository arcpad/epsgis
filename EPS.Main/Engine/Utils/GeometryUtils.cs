using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using EPS.Utils;

namespace EPS.Engine.Utils
{
    /// <summary>
    /// 几何图形的常用方法
    /// </summary>
    public static class GeometryUtils
    {
        /// <summary>
        /// 角度同步,点与线的角度同步
        /// </summary>
        /// <param name="pPolyline">线</param>
        /// <param name="pPoint">线附近的点(许可的范围内)</param>
        /// <param name="dAngle">返回点符号的角度</param>
        /// <param name="pRetPoint">返回的在线上的点的位置</param>
        /// <returns>是否可以同步角度(如果距离过大,无法执行角度同步)</returns>
        public static bool SynchronizeAngle(IPolyline pPolyline, IPoint pPoint, ref double dAngle, ref IPoint pRetPoint)
        {
            double dRadius = 10;
            IPoint pHitPoint = new PointClass();
            IHitTest pHitTest = pPolyline as IHitTest;
            double hitDist = 0.0;
            int hitPartIndex = 0;
            int hitSegmentIndex = 0;
            bool bRightSide = false;

            bool bPointOnLine = pHitTest.HitTest(pPoint, dRadius, esriGeometryHitPartType.esriGeometryPartBoundary, pHitPoint, ref hitDist, ref hitPartIndex, ref hitSegmentIndex, ref bRightSide);
            if (bPointOnLine)
            {
                if (distance(pHitPoint, pPoint) < 0.0001)
                    pRetPoint = null;
                else
                    pRetPoint = pHitPoint;

                ISegmentCollection pSegment = pPolyline as ISegmentCollection;
                ILine pLine = pSegment.get_Segment(hitSegmentIndex) as ILine;
                dAngle = pLine.Angle;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 两个几何图形之间的距离
        /// </summary>
        /// <param name="pGeo1">几何图形</param>
        /// <param name="pGeo2">几何图形</param>
        /// <returns>距离</returns>
        public static double distance(IGeometry pGeo1, IGeometry pGeo2)
        {
            IProximityOperator pProximity = pGeo1 as IProximityOperator;
            return pProximity.ReturnDistance(pGeo2);
        }

        /// <summary>
        /// 点是否在圆内
        /// </summary>
        /// <param name="pCircle">封闭的圆弧</param>
        /// <param name="pPoint">点</param>
        /// <returns>是否在圆内</returns>
        public static bool PointInCircle(ICircularArc pCircle, IPoint pPoint)
        {
            return distance(pCircle.CenterPoint, pPoint) < pCircle.Radius;
        }

        /// <summary>
        /// 搜索指定图层,指定矩形内的要素
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="pEnvelope">矩形</param>
        /// <returns>要素集</returns>
        public static IFeatureCursor SearchFeatures(IFeatureLayer pflayer, IEnvelope pEnvelope)
        {
            Debug.Assert(pEnvelope != null && !pEnvelope.IsEmpty);
            IFeatureClass pfClass = pflayer.FeatureClass;
            ISpatialFilter psFilter = new SpatialFilterClass();
            psFilter.Geometry = pEnvelope;
            psFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            psFilter.GeometryField = pfClass.ShapeFieldName;
            IFeatureCursor result = pfClass.Search(psFilter, false);
            Debug.Assert(result != null);
            return result;
        }

        /// <summary>
        /// 搜索指定图层,在指定点,指定的范围内的要素
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="point">点</param>
        /// <param name="tolerance">范围</param>
        /// <returns>要素集</returns>
        public static IFeatureCursor SearchFeatures(IFeatureLayer pflayer, IPoint point, double tolerance)
        {
            double dx, dy;
            point.QueryCoords(out dx, out dy);
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.XMin = dx - tolerance;
            pEnvelope.XMax = dx + tolerance;
            pEnvelope.YMin = dy - tolerance;
            pEnvelope.YMax = dy + tolerance;

            IFeatureClass pfClass = pflayer.FeatureClass;
            ISpatialFilter psFilter = new SpatialFilterClass();
            psFilter.Geometry = pEnvelope;
            psFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            psFilter.GeometryField = pfClass.ShapeFieldName;
            IFeatureCursor result = pfClass.Search(psFilter, false);
            Debug.Assert(result != null);
            return result;
        }

        /// <summary>
        /// 搜索指定图层,指定矩形内的要素
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="pEnvelope">矩形</param>
        /// <param name="where">条件</param>
        /// <returns>要素集</returns>
        public static IFeatureCursor SearchFeatures(IFeatureLayer pflayer, IEnvelope pEnvelope, string where)
        {
            Debug.Assert(pEnvelope != null && !pEnvelope.IsEmpty);
            IFeatureClass pfClass = pflayer.FeatureClass;
            ISpatialFilter psFilter = new SpatialFilterClass();
            psFilter.Geometry = pEnvelope;
            psFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            psFilter.GeometryField = pfClass.ShapeFieldName;
            psFilter.WhereClause = where;
            IFeatureCursor result = pfClass.Search(psFilter, false);
            Debug.Assert(result != null);
            return result;
        }

        /// <summary>
        /// 搜索指定图层,并与指定要素相交的要素
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="feature">要素</param>
        /// <returns>要素集</returns>
        public static IFeatureCursor SearchFeatures(IFeatureLayer pflayer, IFeature feature)
        {
            Debug.Assert(pflayer != null && feature != null);

            IFeatureClass pfClass = pflayer.FeatureClass;
            ISpatialFilter psFilter = new SpatialFilterClass();
            psFilter.Geometry = feature.Shape;
            psFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            psFilter.GeometryField = pfClass.ShapeFieldName;
            IFeatureCursor result = pfClass.Search(psFilter, false);
            Debug.Assert(result != null);
            return result;

        }

        /// <summary>
        /// 搜索指定图层,指定的范围内的要素(仅取一个要素)
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="pEnvelope">范围</param>
        /// <returns>要素</returns>
        public static IFeature SearchFeature(IFeatureLayer pflayer, IEnvelope pEnvelope)
        {
            IFeatureCursor pfCursor = SearchFeatures(pflayer, pEnvelope);
            IFeature pFeature = pfCursor.NextFeature();
            if (pfCursor != null)
            {
                AppUtils.ReleaseComObject(pfCursor);
            }
            return pFeature;
        }

        /// <summary>
        /// 搜索指定图层,在指定点,指定的范围内的要素(仅取一个要素)
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="point">点</param>
        /// <param name="tolerance"><范围/param>
        /// <returns>要素</returns>
        public static IFeature SearchFeature(IFeatureLayer pflayer, IPoint point, double tolerance)
        {
            IFeatureCursor pfCursor = SearchFeatures(pflayer, point, tolerance);
            IFeature pFeature = pfCursor.NextFeature();
            if (pfCursor != null)
            {
                AppUtils.ReleaseComObject(pfCursor);
            }
            return pFeature;
        }

        /// <summary>
        /// 搜索指定图层,并与指定要素相交的要素(仅取一个要素)
        /// </summary>
        /// <param name="pflayer">图层</param>
        /// <param name="feature">要素</param>
        /// <returns>要素</returns>
        public static IFeature SearchFeature(IFeatureLayer pflayer, IFeature feature)
        {
            IFeatureCursor pfCursor = SearchFeatures(pflayer, feature);
            IFeature pFeature = pfCursor.NextFeature();
            if (pfCursor != null)
            {
                AppUtils.ReleaseComObject(pfCursor);
            }
            return pFeature;
        }

        /// <summary>
        /// 搜索指定图层,指定条件的要素(仅取一个要素)
        /// </summary>
        /// <param name="pfClass">要素类</param>
        /// <param name="where">条件</param>
        /// <returns>要素</returns>
        public static IFeature SearchFeature(IFeatureClass pfClass, string where)
        {
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.WhereClause = where;
            IFeatureCursor pCursor = pfClass.Search(pFilter, false);
            IFeature pFeature = pCursor.NextFeature();
            AppUtils.ReleaseComObject(pCursor);
            return pFeature;
        }

        /// <summary>
        /// 取得两个集合图形的范围的并集
        /// </summary>
        /// <param name="pGeom1">几何图形</param>
        /// <param name="pGeom2">几何图形</param>
        /// <returns>范围</returns>
        public static IEnvelope Union(IGeometry pGeom1, IGeometry pGeom2)
        {
            try
            {
                IEnvelope pEnvelope = pGeom1.Envelope;
                pEnvelope.Union(pGeom2.Envelope);
                return pEnvelope;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 在给定的视口范围内,1个像素单位所代表的地图单位
        /// </summary>
        /// <param name="pActiveView">视口</param>
        /// <param name="pixelUnits">像素单位</param>
        /// <returns>地图单位</returns>
        public static double ConvertPixelsToMapUnits(IActiveView pActiveView, double pixelUnits)
        {
            IPoint p1 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperLeft;
            IPoint p2 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperRight;
            int x1, x2, y1, y2;
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(p1, out x1, out y1);
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(p2, out x2, out y2);
            double pixelExtent = x2 - x1;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }

        /// <summary>
        /// 矩形的中心
        /// </summary>
        /// <param name="pEnvelope">矩形</param>
        /// <returns>中心点</returns>
        public static IPoint CenterPoint(IEnvelope pEnvelope)
        {
            IPoint pPoint = new PointClass();
            pPoint.X = (pEnvelope.XMin + pEnvelope.XMax) / 2;
            pPoint.Y = (pEnvelope.YMax + pEnvelope.YMin) / 2;
            pPoint.Z = (pEnvelope.ZMax + pEnvelope.ZMin) / 2;
            return pPoint;
        }

        /// <summary>
        /// 两个集合图形的交点的集合
        /// </summary>
        /// <param name="pIntersect">几何图形</param>
        /// <param name="pOther">几何图形</param>
        /// <returns>交点的集合</returns>
        public static IPointCollection GetIntersection(IGeometry pIntersect, IGeometry pOther)
        {   // 获取点的交集
            IClone pClone;
            pClone = pIntersect.SpatialReference as IClone;
            if (pClone.IsEqual(pOther.SpatialReference as IClone))
            {
                pOther.Project(pIntersect.SpatialReference);
            }
            ITopologicalOperator pTopoOp;
            pTopoOp = pOther as ITopologicalOperator;
            pTopoOp.Simplify();
            pTopoOp = pIntersect as ITopologicalOperator;
            IGeometry pGeomResult = pTopoOp.Intersect(pOther, esriGeometryDimension.esriGeometry0Dimension);

            if (pGeomResult == null)
                return null;

            if (pGeomResult is IPointCollection)
                return pGeomResult as IPointCollection;

            return null;
        }

        /// <summary>
        /// 将指定的多义线,按某一方向,一定的距离偏移出新的多义线(右手规则)
        /// </summary>
        /// <param name="pBaseLine">指定的多义线</param>
        /// <param name="bOrient">偏移方向(可以使用IHitTest取出方向)</param>
        /// <param name="dOffset">偏移距离</param>
        /// <returns>偏移出的多义线</returns>
        public static IPolyline Offset(IPolyline pBaseLine, bool bOrient, double dOffset)
        {
            // bOrient 以第一点到第二点右边为正方向
            // dOffset 以bOrient方向作出的偏移
            // 1.或取偏移的第一个点
            IPointCollection ptColl = (IPointCollection)pBaseLine;
            IPoint pt0 = ptColl.get_Point(0);
            IPoint pt1 = ptColl.get_Point(1);
            IPoint pt2 = null;
            IPolyline pOffsetLine = new PolylineClass();
            IPointCollection pOffsetColl = (IPointCollection)pOffsetLine;
            IPoint pTemp = GetOffsetPoint(pt0, pt1, dOffset, bOrient);
            object o = Type.Missing;
            pOffsetColl.AddPoint(pTemp, ref o, ref o);

            double seta, seta1, b, b1;
            for (int i = 1; i < ptColl.PointCount - 1; ++i)
            {
                pt0 = ptColl.get_Point(i - 1);
                pt1 = ptColl.get_Point(i);
                pt2 = ptColl.get_Point(i + 1);

                IPoint pTemp1 = GetOffsetPoint(pt1, pt2, dOffset, bOrient);
                if (pTemp1 == null)
                    continue;

                // 内积为 0,三点共线
                if ((pt2.X - pt0.X) * (pt1.Y - pt0.Y) - (pt1.X - pt0.X) * (pt2.Y - pt0.Y) == 0)
                {
                    o = Type.Missing;
                    pOffsetColl.AddPoint(pTemp1, ref o, ref o);
                    continue;
                }
                else
                {
                    double base1 = pt1.X - pt0.X;
                    double base2 = pt2.X - pt1.X;
                    if (base1 != 0 && base2 != 0)
                    {
                        seta = (pt1.Y - pt0.Y) / base1;
                        b = pTemp.Y - seta * pTemp.X;
                        seta1 = (pt2.Y - pt1.Y) / base2;
                        b1 = pTemp1.Y - seta1 * pTemp1.X;
                        pTemp.X = (b - b1) / (seta1 - seta);
                        pTemp.Y = seta * pTemp.X + b;
                        o = Type.Missing;
                        pOffsetColl.AddPoint(pTemp, ref o, ref o);
                    }
                    else if (base1 != 0 && base2 == 0)
                    {
                        seta = (pt1.Y - pt0.Y) / base1;
                        b = pTemp.Y - seta * pTemp.X;
                        pTemp.X = pTemp1.X;
                        pTemp.Y = pTemp.X * seta + b;
                        o = Type.Missing;
                        pOffsetColl.AddPoint(pTemp, ref o, ref o);
                    }
                    else if (base1 == 0 && base2 != 0)
                    {
                        seta1 = (pt2.Y - pt1.Y) / base2;
                        b1 = pTemp1.Y - seta1 * pTemp1.X;
                        pTemp.X = pTemp1.X;
                        pTemp.Y = pTemp.X * seta1 + b1;
                        o = Type.Missing;
                        pOffsetColl.AddPoint(pTemp, ref o, ref o);
                    }
                    else
                    {
                        throw new Exception("error");
                    }
                }
            }
            pt0 = ptColl.get_Point(ptColl.PointCount - 2);
            pt1 = ptColl.get_Point(ptColl.PointCount - 1);
            pt2 = pOffsetColl.get_Point(pOffsetColl.PointCount - 1);
            // 矢量和
            double dx0 = pt2.X - pt0.X;
            double dy0 = pt2.Y - pt0.Y;
            double dx1 = pt1.X - pt0.X;
            double dy1 = pt1.Y - pt0.Y;
            pTemp.X = dx0 + dx1 + pt0.X;
            pTemp.Y = dy0 + dy1 + pt0.Y;
            o = Type.Missing;
            pOffsetColl.AddPoint(pTemp, ref o, ref o);
            return (IPolyline)pOffsetColl;
        }

        /// <summary>
        /// 两条线的交点
        /// </summary>
        /// <param name="pt1">第一条线的端点</param>
        /// <param name="pt2">第一条线的端点</param>
        /// <param name="pt3">第二条线的端点</param>
        /// <param name="pt4">第二条线的端点</param>
        /// <param name="x">交点的坐标</param>
        /// <param name="y">交点的坐标</param>
        /// <returns>
        /// <list type="返回值类型">
        /// <item>-1(说明平行)</item>
        /// <item>1(在直线12上)</item>
        /// <item>2(在直线34上)</item>
        /// <item>3(两条线有交点)</item>
        /// </list>
        /// </returns>
        public static int IntersectPoint(IPoint pt1, IPoint pt2, IPoint pt3, IPoint pt4, out double x, out double y)
        {
            double x1, y1, x2, y2, x3, y3, x4, y4;
            x1 = pt1.X; x2 = pt2.X; y1 = pt1.Y; y2 = pt2.Y;
            x3 = pt3.X; x4 = pt4.X; y3 = pt3.Y; y4 = pt4.Y;
            double px12, py12; //直线12的方向矢量
            double px34, py34; //直线34的方向矢量
            double t12, t34;   //12 和34的长度
            double nx12, ny12; //直线12的单位矢
            double nx34, ny34; //直线34的单位矢
            double px13, py13; //13连线矢量
            double ty;         //13连线矢量在直线12上的投影
            double tysx, tysy; //13连线矢量在直线12上的投影矢量
            double czsx, czsy; //3到12的垂线矢量
            double czL;        //垂线长度
            double gene;       //变换因子

            px12 = x2 - x1;    //12连线矢量
            py12 = y2 - y1;
            px34 = x4 - x3;    //34连线矢量
            py34 = y4 - y3;

            //如果矢积为0，则说明平行，置平行标志，返回
            if (px12 * py34 - px34 * py12 == 0)
            {
                x = 0;
                y = 0;
                return -1;
            }

            //如果3正好落在直线12上,交点为 (x3,x4)
            if ((x3 - x1) * (y3 - y2) + (x3 - x2) * (y3 - y1) == 0)
            {
                x = x3;
                y = y3;
            }
            else
            {
                t12 = Math.Sqrt(px12 * px12 + py12 * py12); //12长度
                t34 = Math.Sqrt(px34 * px34 + py34 * py34); //34长度
                nx12 = px12 / t12;                          //12单位矢
                ny12 = py12 / t12;
                nx34 = px34 / t34;                          //34单位矢
                ny34 = py34 / t34;

                px13 = x3 - x1;                             //13连线矢
                py13 = y3 - y1;

                ty = px13 * nx12 + py13 * ny12;             //投影
                tysx = ty * nx12;                           //投影矢
                tysy = ty * ny12;
                czsx = tysx - px13;                         //垂线矢
                czsy = tysy - py13;
                czL = Math.Sqrt(czsx * czsx + czsy * czsy); //垂线长度

                // 求矢量的投影 矢量v1在v2上的投影 (vx1, vy1)是矢量v1 (vx2, vy2)是矢量v2
                // double prj = (vx1 * vx2 + vy1 * vy2) / sqrt(vx2 * vx2 + vy2 * vy2);
                //gene = czL / calPro(x4 - x3, y4 - y3, czsx, czsy); //变换因子

                double prj = ((x4 - x3) * czsx + (y4 - y3) * czsy) / Math.Sqrt(czsx * czsx + czsy * czsy);
                gene = czL / prj; //变换因子
                x = x3 + px34 * gene;
                y = y3 + py34 * gene;
            }

            int ret = 0;
            if ((x1 - x) * (x2 - x) + (y1 - y) * (y2 - y) < 0)
                ret++;          //判断是否在直线12上
            if ((x3 - x) * (x4 - x) + (y3 - y) * (y4 - y) < 0)
                ret += 2;       //判断是否在直线34上

            return ret;
        }

        /// <summary>
        /// 此函数暂时不使用
        /// </summary>
        /// <param name="ptColl"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static bool PointLieLineRight(IPointCollection ptColl, IPoint pt)
        {
            double dZero = 1e-10;
            double dMinDist = 1E10;
            int ptIndex = -1;
            IPoint pt1, pt2;
            for (int i = 0; i < ptColl.PointCount - 1; ++i)
            {
                pt1 = ptColl.get_Point(i);
                pt2 = ptColl.get_Point(i + 1);
                double dist = Point2Line(pt, pt1, pt2);

                if (dist > dZero && dMinDist > dist)
                {
                    dMinDist = dist;
                    ptIndex = i;
                }
            }
            if (ptIndex != -1)
            {
                pt1 = ptColl.get_Point(ptIndex);
                pt2 = ptColl.get_Point(ptIndex + 1);

                // 直线向量     (pt2.X-pt1.X, pt2.Y-pt1.Y)
                // 点的旋转向量 (pt.X-pt1.X,  pt.Y-pt1.Y)
                double x = pt2.X - pt1.X;
                double y = pt2.Y - pt1.Y;
                double x1 = pt.X - pt1.X;
                double y1 = pt.Y - pt1.Y;

                if (Math.Abs(x) < dZero) // vert
                {
                    return y * x1 > 0;
                }
                else
                {
                    double k = y / x;
                    double yy = k * x1;
                    return x * (yy - y1) > 0;
                }
            }
            return false;
        }

        /// <summary>
        /// 将线要素切割
        /// </summary>
        /// <param name="pFeature">被切割的线要素</param>
        /// <param name="pPoint">切割点</param>
        /// <returns>切割后的线要素</returns>
        public static IFeature SplitPolyLine(IFeature pFeature, IPoint pPoint)
        {
            IGeometry pGeometry = pFeature.Shape;
            if (((IPolyline)pGeometry).FromPoint.X == pPoint.X && ((IPolyline)pGeometry).FromPoint.Y == pPoint.Y)
                return pFeature;
            if (((IPolyline)pGeometry).ToPoint.X == pPoint.X && ((IPolyline)pGeometry).ToPoint.Y == pPoint.Y)
                return pFeature;
            try
            {
                IWorkspaceEdit pEditor = GeoDbUtils.InitWorkspace() as IWorkspaceEdit;
                pEditor.StartEditOperation();
                IFeatureEdit pEditFeature = pFeature as IFeatureEdit;
                ISet pSet = pEditFeature.Split(pPoint);
                pSet.Reset();
                IFeature pRetFeature = pSet.Next() as IFeature;
                pGeometry = pRetFeature.Shape;
                if (((IPointCollection)(IPolyline)pGeometry).PointCount == 2)
                    pRetFeature = pSet.Next() as IFeature;

                pEditor.StopEditOperation();
                return pRetFeature;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 判断两个点是否是同一点
        /// </summary>
        /// <param name="pt1">点</param>
        /// <param name="pt2">点</param>
        /// <returns>是否相同</returns>
        public static bool IdentialPoint(IPoint pt1, IPoint pt2)
        {
            return Math.Abs(pt1.X - pt2.X) < 1e-9 && Math.Abs(pt1.Y - pt2.Y) < 1e-9 && Math.Abs(pt1.Z - pt2.Z) < 1e-9;
        }

        /// <summary>
        /// 合并多条多义线,如果合并的多义线不是简单的多义线则抛出异常
        /// </summary>
        /// <param name="pPolylines">多义线集</param>
        /// <returns>多义线</returns>
        public static IPolyline UnionPolylines(IPolyline[] pPolylines)
        {
            // 第一条线段
            int indexFirst = -1;
            int degree = 0;
            IRelationalOperator pRO = null;
            int nSelected = pPolylines.Length;
            IPolyline pRetPolyline = new PolylineClass();
            ITopologicalOperator pTopoOper = (ITopologicalOperator)pRetPolyline;
            IGeometryCollection pGeometryCollection = new GeometryBagClass();
            for (int i = 0; i < pPolylines.Length; i++)
            {
                object o = Type.Missing;
                pGeometryCollection.AddGeometry(pPolylines[i],
                    ref o, ref o);
            }
            pTopoOper.ConstructUnion((IEnumGeometry)pGeometryCollection);
            IGeometryCollection pGeometryColl = (IGeometryCollection)pTopoOper;
            if (pGeometryColl.GeometryCount > 1)
                throw new Exception("线段的空间连接不正确");

            return (IPolyline)pTopoOper;

            // AddGeometry
            // 数据检查
            //ConstructUnion
            /*
            double tol = GeometryHelper.ConvertPixelsToMapUnits(pActiveView, 4);
            IPoint pHitPoint = null;
            double hitDist = 0;
            int partIndex = 0;
            int vertexIndex = 0;
            int vertexOffset = 0;
            bool vertex = false;
            if (EditHelper.HitTest(tol, _pMenuPosition, pFeature, ref pHitPoint, ref hitDist, ref partIndex, ref vertexIndex, ref vertexOffset, ref vertex))
            {

            }
            */
            for (int i = 0; i < nSelected; i++)
            {
                int nTouch = 0;
                pRO = (IRelationalOperator)pPolylines[i];
                IHitTest hittest = (IHitTest)pRO;
                for (int j = 0; j < nSelected; j++)
                {
                    if (i != j && pRO.Touches(pPolylines[j]))
                    {
                        nTouch++;
                    }
                }
                if (nTouch == 0 || nTouch > 2)
                {
                    throw new Exception("line touch error");
                }
                else if (nTouch == 1)
                {
                    if (indexFirst == -1)
                        indexFirst = i;

                    if (++degree > 2)
                        throw new Exception("multi patchs");
                }
            }
            // 依据第一条线topo有序
            if (indexFirst == -1)
                throw new Exception("line circle");

            IPolyline pTemp = pPolylines[indexFirst];
            pPolylines[indexFirst] = pPolylines[0];
            pPolylines[0] = pTemp;
            IPolyline pPolyline = new PolylineClass();
            ISegmentCollection pSegments = (ISegmentCollection)pPolyline;
            pSegments.AddSegmentCollection((ISegmentCollection)pPolylines[0]);
            for (int i = 0; i < nSelected - 1; i++)
            {
                pRO = (IRelationalOperator)pPolylines[i];
                for (int j = i + 1; j < nSelected; j++)
                {
                    if (pRO.Touches(pPolylines[j]))
                    {
                        pTemp = pPolylines[j];
                        pPolylines[j] = pPolylines[i + 1];
                        pPolylines[i + 1] = pTemp;
                        if (IdentialPoint(pPolylines[i].FromPoint, pPolylines[i + 1].ToPoint))
                        {
                            //pSegments.AddSegmentCollection();
                        }
                        break;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 点(pt)是否在线上
        /// </summary>
        /// <param name="pt">点</param>
        /// <param name="pt1">线的端点</param>
        /// <param name="pt2">线的端点</param>
        /// <returns>否在线上</returns>
        private static bool PointInLine(IPoint pt, IPoint pt1, IPoint pt2)
        {
            return PointInLine(pt.X, pt.Y, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="pt">点</param>
        /// <param name="pt1">线的端点</param>
        /// <param name="pt2">线的端点</param>
        /// <returns>距离</returns>
        private static double Point2Line(IPoint pt, IPoint pt1, IPoint pt2)
        {
            return PointToLine(pt.X, pt.Y, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// 点是否在线上
        /// </summary>
        /// <param name="x">点的坐标</param>
        /// <param name="y">点的坐标</param>
        /// <param name="x1">端点的坐标</param>
        /// <param name="y1">端点的坐标</param>
        /// <param name="x2">端点的坐标</param>
        /// <param name="y2">端点的坐标</param>
        /// <returns>点是否在线上</returns>
        private static bool PointInLine(double x, double y, double x1, double y1, double x2, double y2)
        {
            // 判断点是否在外接矩形范围内 
            if (x < Math.Min(x1, x2) || x > Math.Max(x1, x2) || y < Math.Min(y1, y2) || y > Math.Max(y1, y2))
                return false;
            //计算叉乘积  
            if (1E10 > Math.Abs((x - x1) * (y2 - y1) - (x2 - x1) * (y - y1)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 两点距离
        /// </summary>
        /// <param name="x1">端点的坐标</param>
        /// <param name="y1">端点的坐标</param>
        /// <param name="x2">端点的坐标</param>
        /// <param name="y2">端点的坐标</param>
        /// <returns>距离</returns>
        private static double distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="x">点的坐标</param>
        /// <param name="y">点的坐标</param>
        /// <param name="x1">端点的坐标</param>
        /// <param name="y1">端点的坐标</param>
        /// <param name="x2">端点的坐标</param>
        /// <param name="y2">端点的坐标</param>
        /// <returns>距离</returns>
        private static double PointToLine(double x, double y, double x1, double y1, double x2, double y2)
        {
            if (PointInLine(x, y, x1, y1, x2, y2))
                return 0.0;
            if (Math.Abs(x2 - x1) <= 1E10)
            { //垂直线段
                //垂足在线段范围内
                if (y > Math.Min(y1, y2) && y < Math.Max(y1, y2))
                    return Math.Abs(x - x1);
                else
                {
                    //判断线段的哪个端点离垂足比较近,然后计算两点间距离
                    if (Math.Abs(y - y1) < Math.Abs(y - y2))
                        return distance(x, y, x1, y1);
                    else
                        return distance(x, y, x2, y2);
                }
            }
            else if (Math.Abs(y2 - y1) <= 1e10)
            { //水平线段
                if (x > Math.Min(x1, x2) && x < Math.Max(x1, x2)) //垂足在线段范围内
                    return Math.Abs(y - y1);
                else
                {
                    //判断线段的哪个端点离垂足比较近,然后计算两点间距离
                    if (Math.Abs(x - x1) < Math.Abs(x - x2))
                        return distance(x, y, x1, y1);
                    else
                        return distance(x, y, x2, y2);
                }
            }
            else
            {
                double k = 0.0; // 斜率
                k = (y1 - y2) / (x2 - x1);
                double footx, footy;
                footx = (k * k * x1 + k * (y1 - y) + x) / (k * k + 1);
                footy = k * (x1 - footx) + y1;

                if (PointInLine(footx, footy, x1, y1, x2, y2)) //垂足在线段上
                    return distance(x, y, footx, footy);
                else
                    return Math.Min(distance(x, y, x1, y1), distance(x, y, x2, y2));
            }
        }

        /// <summary>
        /// 获取偏移点
        /// </summary>
        /// <param name="pt1">起点</param>
        /// <param name="pt2">止点</param>
        /// <param name="dOffset">距离</param>
        /// <param name="bOrient">方向（使用IHitTest获取）</param>
        /// <returns></returns>
        private static IPoint GetOffsetPoint(IPoint pt1, IPoint pt2, double dOffset, bool bOrient)
        {
            // 标准化向量
            double x = pt2.X - pt1.X;
            double y = pt2.Y - pt1.Y;

            if (x == 0 && y == 0)
                return null;
            // 顺时针方向
            if (bOrient)
            {
                double ratio = dOffset / Math.Sqrt(x * x + y * y);
                double x1 = y * ratio;
                double y1 = -x * ratio;

                IPoint pt = new PointClass();
                pt.X = x1 + pt1.X;
                pt.Y = y1 + pt1.Y;
                return pt;
            }
            else
            {
                double ratio = dOffset / Math.Sqrt(x * x + y * y);
                double x1 = -y * ratio;
                double y1 = x * ratio;

                IPoint pt = new PointClass();
                pt.X = x1 + pt1.X;
                pt.Y = y1 + pt1.Y;
                return pt;
            }
        }
    }
}
