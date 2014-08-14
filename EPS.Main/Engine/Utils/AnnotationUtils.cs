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

namespace EPS.Engine.Utils
{
    public static class AnnotationUtils
    {
         /// <summary>
        /// 获取与要素设置的releationshipclss对应的标注要素

        /// </summary>
        /// <param name="pFeature">地图要素</param>
        /// <returns>标注要素</returns>
        public static IAnnotationFeature2 GetLinkedFeature(IFeature pFeature)
        {
            // 方法一
            IFeatureClass pfClass = (IFeatureClass)pFeature.Class;
            IEnumRelationshipClass pEnumRelation =
                            pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = pEnumRelation.Next();
            if (pRelationship == null)
                return null;

            ISet pSet = pRelationship.GetObjectsRelatedToObject(pFeature);
            if (pSet == null)
                return null;
            
            object o = pSet.Next();
            if (o == null || !(o is IAnnotationFeature2))
                return null;

            IAnnotationFeature2 pAnnoFeature = (IAnnotationFeature2)o;
            return pAnnoFeature;
            /*
            方法二

            // 注记的FeatureClass
            IFeatureClass pDestionClass = (IFeatureClass)pRelationship.DestinationClass;
            if (pDestionClass == null)
                return null;

            string foreignKey = pRelationship.OriginForeignKey;
            if (string.IsNullOrEmpty(foreignKey))
                return null;

            string where = foreignKey + "='" + pFeature.OID + "'";
            IAnnotationFeature2 pAnnoFeature =
                (IAnnotationFeature2)GeometryHelper.SearchFeature(pDestionClass, where);

            return pAnnoFeature;
            */
        }

        /// <summary>
        /// 获取与标注对应的关联设备
        /// </summary>
        /// <param name="pAnnoFeature"></param>
        /// <returns></returns>
        public static IFeature GetLinkedFeature(IAnnotationFeature2 pAnnoFeature)
        {
            // 方法一
            IFeature pFeature = (IFeature)pAnnoFeature;
            IFeatureClass pfClass = (IFeatureClass)pFeature.Class;
            IEnumRelationshipClass pEnumRelation =
                            pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = pEnumRelation.Next();
            if (pRelationship == null)
                return null;

            ISet pSet = pRelationship.GetObjectsRelatedToObject(pFeature);
            if (pSet == null)
                return null;

            object o = pSet.Next();
            if (o == null || !(o is IFeature))
                return null;

            pFeature = (IFeature)o;
            return pFeature;

            // 方法二

            /*
            IFeature pFeature = (IFeature)pAnnoFeature;
            IFeatureClass pfClass = (IFeatureClass)pFeature.Class;
            IEnumRelationshipClass enumRelCls =
                        pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = enumRelCls.Next();
            if (pRelationship == null)
                return null;

            // 注记的FeatureClass
            IFeatureClass pDestionClass = (IFeatureClass)pRelationship.OriginClass;
            if (pDestionClass == null)
                return null;

            string foreignKey = pRelationship.DestinationPrimaryKey;
            if (string.IsNullOrEmpty(foreignKey))
                return null;

            pFeature = pDestionClass.GetFeature(pAnnoFeature.LinkedFeatureID);
            return pFeature;
            */
        }

        /// <summary>
        /// 获取与pFeautre对应的标注的FeatureClass
        /// </summary>
        /// <param name="pFeature">地图要素，如杆塔</param>
        /// <returns></returns>对应的标注的FeatureClass
        public static IFeatureClass GetLinkedFeatureClass(IFeature pFeature)
        {
            IFeatureClass pfClass = (IFeatureClass)pFeature.Class;
            IEnumRelationshipClass enumRelCls =
                            pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = enumRelCls.Next();
            if (pRelationship == null)
                return null;

            // 注记的FeatureClass
            IFeatureClass pDestionClass = (IFeatureClass)pRelationship.DestinationClass;
            return pDestionClass;
        }

        /// <summary>
        /// 获取与pfClass对应的标注的FeatureClass
        /// </summary>
        /// <param name="pfClass">地图要素类，如杆塔</param>
        /// <returns>对应的标注的FeatureClass</returns>
        public static IFeatureClass GetLinkedFeatureClass(IFeatureClass pfClass)
        {
            IEnumRelationshipClass enumRelCls =
                            pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = enumRelCls.Next();
            if (pRelationship == null)
                return null;

            // 注记的FeatureClass
            IFeatureClass pDestionClass = (IFeatureClass)pRelationship.DestinationClass;
            return pDestionClass;
        }

        /// <summary>
        /// 创建图形要素和标注要素之间的关联关系[使用前确保要素层与标注层，确实存在关联关系]
        /// </summary>
        /// <param name="OriginObject">图形要素</param>
        /// <param name="DestinationObject">标注要素</param>
        /// <returns></returns>
        public static IRelationship CreateRelationship(IFeature OriginObject, 
            IAnnotationFeature2 DestinationObject)
        {
            IFeatureClass pfClass = (IFeatureClass)OriginObject.Class;
            IEnumRelationshipClass pEnumRelation =
                            pfClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass pRelationship = pEnumRelation.Next();
            if (pRelationship == null)
                return null;

            IRelationship pRelation = null;
            pRelation = pRelationship.CreateRelationship((IObject)OriginObject, 
                (IObject)DestinationObject);
            return pRelation;
            //IObject OriginObject, IObject DestinationObject
        }
    }
}
