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
    /// 对操作空间数据库的常用方法静态描述
    /// </summary>
    public static class GeoDbUtils
    {
        public const string _ANGLE_FIELD = "ANGLE";
        public const string _DTYPE_FIELD = "DTYPE";
        private static IMap _pMap = null;
        private static IWorkspace _pWorkspace = null;

        /// <summary>
        /// 唯一的当前地图
        /// </summary>
        public static IMap Map
        {
            get { return _pMap; }
            set
            {
                _pMap = value;
                InitWorkspace();
            }
        }

        /// <summary>
        /// 获取要素中第index个字段的值，该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素（FeatureClass实现了IRow接口）</param>
        /// <param name="index">字段索引</param>
        /// <returns>值</returns>
        public static object GetFieldValue(IFeature pFeature, int index)
        {
            if (pFeature == null)
                throw new Exception("[" + pFeature.ToString() + "]非法引用");

            if (!(pFeature.Fields.FieldCount > index && index >= 0))
                throw new Exception("字段索引超出边界");

            return pFeature.get_Value(index);
        }

        /// <summary>
        /// 获取要素中第index个字段的值，该函数抛出异常
        /// </summary>
        /// <param name="pRow">行</param>
        /// <param name="index">字段索引</param>
        /// <returns>值</returns>
        public static object GetFieldValue(IRow pRow, int index)
        {
            if (pRow == null)
                throw new Exception("[" + pRow.ToString() + "]非法引用");
            if (!(pRow.Fields.FieldCount > index && index >= 0))
                throw new Exception("字段索引超出边界");

            return pRow.get_Value(index);
        }

        /// <summary>
        /// 获取要素中指定字段的值，该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sField">字段</param>
        /// <returns>值</returns>
        public static object GetFieldValue(IFeature pFeature, string sField)
        {
            int index = pFeature.Fields.FindField(sField);
            if (index == -1)
                throw new Exception("[" + sField + "]不存在");
            else
                return pFeature.get_Value(index);
        }

        /// <summary>
        /// 获取要素行中指定字段的值,该函数抛出异常
        /// </summary>
        /// <param name="pRow">行</param>
        /// <param name="sField">字段</param>
        /// <returns>值</returns>
        public static object GetFieldValue(IRow pRow, string sField)
        {
            int index = pRow.Fields.FindField(sField);
            if (index == -1)
                throw new Exception("[" + sField + "]不存在");
            else
                return pRow.get_Value(index);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, int index, ref int value)
        {
            object obj = GetFieldValue(pFeature, index);
            Debug.Assert(obj != null);
            value = Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, int index, ref double value)
        {
            object obj = GetFieldValue(pFeature, index);
            Debug.Assert(obj != null);
            value = Convert.ToDouble(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, int index, ref string value)
        {
            object obj = GetFieldValue(pFeature, index);
            Debug.Assert(obj != null);
            value = Convert.ToString(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, int index, ref int value)
        {
            object obj = GetFieldValue(pRow, index);
            Debug.Assert(obj != null);
            value = Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, int index, ref double value)
        {
            object obj = GetFieldValue(pRow, index);
            Debug.Assert(obj != null);
            value = Convert.ToDouble(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, int index, ref string value)
        {
            object obj = GetFieldValue(pRow, index);
            Debug.Assert(obj != null);
            value = Convert.ToString(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, string sField, ref int value)
        {
            object obj = GetFieldValue(pFeature, sField);
            Debug.Assert(obj != null);
            value = Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, string sField, ref double value)
        {
            object obj = GetFieldValue(pFeature, sField);
            try
            {
                value = Convert.ToDouble(obj);
                if (value.Equals(double.NaN))
                {
                    value = 0.0;
                }
            }
            catch
            {
                value = 0.0;
            }
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IFeature pFeature, string sField, ref string value)
        {
            object obj = GetFieldValue(pFeature, sField);
            Debug.Assert(obj != null);
            value = Convert.ToString(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pRow">要素行</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, string sField, ref int value)
        {
            object obj = GetFieldValue(pRow, sField);
            Debug.Assert(obj != null);
            value = Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pRow">要素行</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, string sField, ref double value)
        {
            object obj = GetFieldValue(pRow, sField);
            Debug.Assert(obj != null);
            value = Convert.ToDouble(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pRow">要素行</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, string sField, ref string value)
        {
            object obj = GetFieldValue(pRow, sField);
            Debug.Assert(obj != null);
            value = Convert.ToString(obj);
        }

        /// <summary>
        /// 获取要素行中指定字段索引的值,该函数抛出异常
        /// </summary>
        /// <param name="pRow">要素行</param>
        /// <param name="sField">字段</param>
        /// <param name="value">返回值</param>
        public static void GetFieldValue(IRow pRow, string sField, ref IMemoryBlobStream value)
        {
            object obj = GetFieldValue(pRow, sField);
            Debug.Assert(obj != null);
            value = obj as IMemoryBlobStream;
        }

        /// <summary>
        /// 获取要素图层中指定oid的要素
        /// </summary>
        /// <param name="pLayer">要素图层</param>
        /// <param name="id">objectid</param>
        /// <returns>要素</returns>
        public static IFeature GetFeature(IFeatureLayer pLayer, int id)
        {
            Debug.Assert(pLayer != null && id >= 0);
            return pLayer.FeatureClass.GetFeature(id);
        }

        /// <summary>
        ///  获取要素图层中指定条件的要素,并进返回一个要素
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <param name="condition">条件（符合标准SQL标准的条件查询语句）</param>
        /// <returns>要素</returns>
        public static IFeature GetFeature(IFeatureLayer pLayer, string condition)
        {
            Debug.Assert(pLayer != null && condition != "");
            IFeatureClass pfClass = pLayer.FeatureClass;
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.WhereClause = condition;
            IFeatureCursor pfCursor = pfClass.Search(pFilter, false);
            if (pfCursor == null)
                return null;

            return pfCursor.NextFeature();
        }

        /// <summary>
        /// 根据要素获取要素对应的图层
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <returns>图层</returns>
        public static IFeatureLayer GetFeatureLayer(IFeature pFeature)
        {
            IFeatureClass pfClass = pFeature.Class as IFeatureClass;
            return GetFeatureLayer(pfClass.AliasName, true);
        }

        /// <summary>
        /// 根据要素获取要素对应的图层
        /// </summary>
        /// <param name="pFeature">要素</param>
        /// <returns>图层</returns>
        public static ILayer GetLayer(IFeature pFeature)
        {
            IFeatureClass pfClass = (IFeatureClass)pFeature.Class;
            IEnumLayer pEnum = GetDataLayer(true);
            ILayer pLayer = null;
            while ((pLayer = pEnum.Next()) != null)
            {
                if (pLayer.Name == pfClass.AliasName)
                {
                    return pLayer;
                }
            }
            return null;
            //return GetLayer(pfClass.AliasName, true);
        }

        /// <summary>
        /// 根据图层的表名/别名（由bAlisa是否为别名指定），获取图层名
        /// </summary>
        /// <param name="name">表名/别名</param>
        /// <param name="bAlias">是否为别名</param>
        /// <returns></returns>
        public static ILayer GetLayer(string name, bool bAlias)
        {
            IEnumLayer pEnum = _pMap.get_Layers(null as UID, true);
            ILayer pLayer = null;
            if (bAlias)
            {
                while ((pLayer = pEnum.Next()) != null)
                {
                    if (pLayer.Name == name)
                    {
                        return pLayer;
                    }
                }
            }
            else
            {
                while ((pLayer = pEnum.Next()) != null)
                {
                    if (pLayer is IFeatureLayer)
                    {
                        IDataset pDataset = (IDataset)pLayer;
                        if (pDataset.BrowseName == name)
                        {
                            return pLayer;
                        }
                    }
                    else if (pLayer is IGroupLayer)
                    {
                        if (pLayer.Name == name)
                        {
                            return pLayer;
                        }
                    }
                    else if (pLayer is IAnnotationLayer)
                    {
                        if (pLayer.Name == name)
                        {
                            return pLayer;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取图层组
        /// </summary>
        /// <param name="recursive">是否回收资源</param>
        /// <returns>图层组枚举接口</returns>
        public static IEnumLayer GetGroupLayers(bool recursive)
        {
            UID uid = new UIDClass();
            uid.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
            IEnumLayer pEnum = _pMap.get_Layers(uid, recursive);
            return pEnum;
        }

        /// <summary>
        /// 获取图层组
        /// </summary>
        /// <param name="aliasname">图层组别名</param>
        /// <returns>图层组接口</returns>
        public static IGroupLayer GetGroupLayer(string aliasname)
        {
            UID uid = new UIDClass();
            uid.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
            IEnumLayer pEnum = _pMap.get_Layers(uid, true);
            ILayer pLayer = pEnum.Next();
            while (pLayer != null)
            {
                if (pLayer.Name == aliasname)
                    return pLayer as IGroupLayer;
                pLayer = pEnum.Next();
            }
            return null;
        }

        /// <summary>
        /// 获取数据图层（具体含义见arcobject帮组[搜索关键字6CA416B1-E160-11D2-9F4E-00C04F6BC78E]）
        /// </summary>
        /// <param name="recursive">是否会受资源</param>
        /// <returns>图层枚举接口</returns>
        public static IEnumLayer GetDataLayer(bool recursive)
        {
            UID uid = new UIDClass();
            uid.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";
            IEnumLayer pEnum = _pMap.get_Layers(uid, recursive);
            return pEnum;
        }

        /// <summary>
        /// 获取要素图层
        /// </summary>
        /// <param name="recursive">是否会受资源</param>
        /// <returns>图层枚举接口</returns>
        public static IEnumLayer GetFeatureLayers(bool recursive)
        {
            UID uid = new UIDClass();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            IEnumLayer pEnum = _pMap.get_Layers(uid, recursive);
            return pEnum;
        }

        /// <summary>
        /// 获取要素层
        /// </summary>
        /// <param name="name">别名/表名</param>
        /// <param name="bAlias">是否为别名</param>
        /// <returns>要素层</returns>
        public static IFeatureLayer GetFeatureLayer(string name, bool bAlias)
        {
            UID uid = new UIDClass();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            IEnumLayer pEnum = _pMap.get_Layers(uid, true);
            ILayer pLayer = pEnum.Next();
            if (bAlias)
            {
                while (pLayer != null)
                {
                    if (pLayer.Name == name)
                    {
                        return pLayer as IFeatureLayer;
                    }
                    pLayer = pEnum.Next();
                }
            }
            else
            {
                while (pLayer != null)
                {
                    IDataset pDataset = (IDataset)pLayer;
                    if (pDataset.BrowseName == name)
                    {
                        return pLayer as IFeatureLayer;
                    }
                    pLayer = pEnum.Next();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取要素层
        /// </summary>
        /// <param name="aliasname">别名</param>
        /// <param name="groupname">图层组名</param>
        /// <returns>要素层</returns>
        public static IFeatureLayer GetFeatureLayer(string aliasname, string groupname)
        {
            IGroupLayer pgLayer = GetGroupLayer(groupname);
            Debug.Assert(pgLayer != null);
            ICompositeLayer pcomLayer = pgLayer as ICompositeLayer;

            ILayer pLayer = null;
            int nLayer = pcomLayer.Count;
            for (int i = 0; i < nLayer; i++)
            {
                pLayer = pcomLayer.get_Layer(i);
                if (pLayer.Name == aliasname)
                {
                    return pLayer as IFeatureLayer;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取要素层
        /// </summary>
        /// <param name="pgLayer">图层组接口</param>
        /// <param name="aliasname">别名</param>
        /// <returns>要素层</returns>
        public static IFeatureLayer GetFeatureLayer(IGroupLayer pgLayer, string aliasname)
        {
            ICompositeLayer pcomLayer = pgLayer as ICompositeLayer;
            Debug.Assert(pcomLayer != null);

            ILayer pLayer = null;
            int nLayer = pcomLayer.Count;
            for (int i = 0; i < nLayer; i++)
            {
                pLayer = pcomLayer.get_Layer(i);
                if (pLayer.Name == aliasname)
                {
                    return pLayer as IFeatureLayer;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据指定的表、字段、条件获取查询游标，使用完毕后，必须使用AppHelper.ReleaseComObject()释放
        /// </summary>
        /// <param name="sTable">表</param>
        /// <param name="sFields">字段</param>
        /// <param name="sClause">查询条件</param>
        /// <returns>查询游标</returns>
        public static ICursor GetRows(string sTable, string sFields, string sClause)
        {
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.AddField(sFields);
            pQueryFilter.WhereClause = sClause;
            IWorkspace pWorkspace = InitWorkspace();
            IFeatureWorkspace pfWorkspace = pWorkspace as IFeatureWorkspace;

            ITable pTable = pfWorkspace.OpenTable(sTable);
            if (pTable == null)
                throw new Exception("表[" + sTable + "]访问错误");

            return pTable.Search(pQueryFilter, false);

        }

        /// <summary>
        /// 根据指定的表、字段、条件获取查询游标中的第一条数据
        /// </summary>
        /// <param name="sTable">表</param>
        /// <param name="sFields">字段</param>
        /// <param name="sClause">查询条件</param>
        /// <returns>行</returns>
        public static IRow GetRow(string sTable, string sFields, string sClause)
        {
            ICursor pCursor = GetRows(sTable, sFields, sClause);
            if (pCursor == null)
                throw new Exception(sTable + " | " + sFields + " | " + sClause + "条件错误");

            IRow pRow = pCursor.NextRow();
            AppUtils.ReleaseComObject(pCursor);
            return pRow;
        }

        /// <summary>
        /// 初始化GeoDatabase的工作空间
        /// </summary>
        /// <returns>工作空间接口</returns>
        public static IWorkspace InitWorkspace()
        {
            IMap pMap = _pMap;
            // Debug.Assert(pMap != null);
            if (_pWorkspace != null)
                return _pWorkspace;

            IEnumLayer pEnumLayer = pMap.get_Layers(null as UID, true);
            if (pEnumLayer == null)
                return null;

            IFeatureLayer pfLayer = null;
            IFeatureClass pfClass = null;
            IDataset pDataset = null;
            IWorkspace pWorkspace = null;

            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (pLayer is IFeatureLayer)
                {
                    pfLayer = pLayer as IFeatureLayer;
                    pfClass = pfLayer.FeatureClass;
                    pDataset = pfClass as IDataset;
                    if (pDataset != null)
                    {
                        pWorkspace = pDataset.Workspace;
                        IFeatureWorkspace pfw = pWorkspace as IFeatureWorkspace;
                        if (pfw != null)
                        {
                            _pWorkspace = pWorkspace;
                            return pWorkspace;
                        }
                    }
                }
                pLayer = pEnumLayer.Next();
            }
            _pWorkspace = null;
            return null;
        }

        /// <summary>
        /// 对非图形要素的操作使用该函数开启会话
        /// </summary>
        public static void StartEditOperation()
        {
            IWorkspaceEdit pEditor = _pWorkspace as IWorkspaceEdit;
            if (!pEditor.IsBeingEdited())
                throw new Exception("请开启编辑会话");

            pEditor.StartEditOperation();
        }

        /// <summary>
        /// 对非图形要素的操作使用该函数,在使用StartEditOperation后,必须使用该函数结束会话
        /// 二者必须成对儿出现
        /// </summary>
        public static void StopEditOperation()
        {
            IWorkspaceEdit pEditor = _pWorkspace as IWorkspaceEdit;
            pEditor.StopEditOperation();
        }

        /// <summary>
        /// 根据表名获取表的接口
        /// </summary>
        /// <param name="sTable">表名</param>
        /// <returns></returns>
        public static ITable OpenTable(string sTable)
        {
            IFeatureWorkspace pfWorkspace = _pWorkspace as IFeatureWorkspace;
            ITable pTable = pfWorkspace.OpenTable(sTable);
            return pTable;
        }

        /// <summary>
        /// 获取指定条件的行，用于更新数据使用
        /// </summary>
        /// <param name="pTable">表接口</param>
        /// <param name="sClause">查询条件</param>
        /// <returns>用于更新的行</returns>
        public static IRow GetUpdateRow(ITable pTable, string sClause)
        {
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = sClause;
            ICursor pCursor = pTable.Search(pQueryFilter, false);
            IRow pRow = pCursor.NextRow();
            pRow = pTable.GetRow(pRow.OID);
            return pRow;
        }

        /// <summary>
        /// 获取指定条件的行，用于更新数据使用
        /// </summary>
        /// <param name="sTable">表名</param>
        /// <param name="sClause">查询条件</param>
        /// <returns>用于更新的行</returns>
        public static IRow GetUpdateRow(string sTable, string sClause)
        {
            ITable pTable = OpenTable(sTable);
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = sClause;
            ICursor pCursor = pTable.Search(pQueryFilter, false);
            IRow pRow = pCursor.NextRow();
            if (pRow == null)
                return null;
            pRow = pTable.GetRow(pRow.OID);
            return pRow;
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, int index, IMemoryBlobStream value)
        {
            Debug.Assert(pUpdateRow != null && index > 0 && value != null);
            pUpdateRow.set_Value(index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="sField">字段</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, string sField, IMemoryBlobStream value)
        {
            IFields pFields = pUpdateRow.Fields;
            int index = pFields.FindField(sField);
            SetFieldValue(pUpdateRow, index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="sField">字段</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, string sField, string value)
        {
            IFields pFields = pUpdateRow.Fields;
            int index = pFields.FindField(sField);
            SetFieldValue(pUpdateRow, index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, int index, string value)
        {
            Debug.Assert(pUpdateRow != null && index > 0 && value != null);
            pUpdateRow.set_Value(index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, int index, int value)
        {
            Debug.Assert(pUpdateRow != null && index > 0);
            pUpdateRow.set_Value(index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="sField">字段</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, string sField, int value)
        {
            IFields pFields = pUpdateRow.Fields;
            int index = pFields.FindField(sField);
            SetFieldValue(pUpdateRow, index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="index">字段索引</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, int index, double value)
        {
            Debug.Assert(pUpdateRow != null && index > 0);
            pUpdateRow.set_Value(index, value);
        }

        /// <summary>
        /// 修改更新游标中行的数据
        /// </summary>
        /// <param name="pUpdateRow">需要特别说明的是[pUpdateRow(必须使用GetUpdateRow函数获得行接口)]</param>
        /// <param name="sField">字段</param>
        /// <param name="value">值</param>
        public static void SetFieldValue(IRow pUpdateRow, string sField, double value)
        {
            IFields pFields = pUpdateRow.Fields;
            int index = pFields.FindField(sField);
            SetFieldValue(pUpdateRow, index, value);
        }

        /// <summary>
        /// 根据指定的表、字段、条件获取用于数据更改的游标
        /// </summary>
        /// <param name="sTable">表</param>
        /// <param name="sFields">字段</param>
        /// <param name="sClause">标件</param>
        /// <returns>用于数据更改的游标</returns>
        public static ICursor GetUpdateRows(string sTable, string sFields, string sClause)
        {
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.SubFields = sFields;
            pQueryFilter.WhereClause = sClause;

            IFeatureWorkspace pfWorkspace = _pWorkspace as IFeatureWorkspace;
            ITable pTable = pfWorkspace.OpenTable(sTable);
            Debug.Assert(pTable != null);
            return pTable.Update(pQueryFilter, false);
        }

        /// <summary>
        /// 执行update / insert sql语句，不抛出异常
        /// </summary>
        /// <param name="sql">数据库语句</param>
        /// <returns>是否成功执行</returns>
        public static bool ExecuteSql(string sql)
        {
            try
            {
                _pWorkspace.ExecuteSQL(sql);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 执行update / insert sql语句，抛出异常
        /// </summary>
        /// <param name="sql">数据库语句</param>
        public static void ExecuteSQL(string sql)
        {
            _pWorkspace.ExecuteSQL(sql);
        }

        /// <summary>
        /// 当前程序是否处于编辑状态
        /// </summary>
        public static bool IsBeingEdited
        {
            get
            {
                IWorkspace pWorkspace = InitWorkspace();
                IWorkspaceEdit pWorkspaceEdit = pWorkspace as IWorkspaceEdit;
                if (pWorkspaceEdit != null)
                {
                    return pWorkspaceEdit.IsBeingEdited();
                }
                return false;
            }
        }
    }
}
