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
    /// <summary>
    /// 数据同步检测、处理、仲裁、提交
    /// </summary>
    public static class VersionUtils
    {
        /// <summary>
        /// 数据版本是否被锁定
        /// </summary>
        /// <param name="pVerison">指定的版本</param>
        /// <returns>是否被锁定</returns>
        public static bool IsLocked(IVersion pVerison)
        {
            IEnumLockInfo pEnumLockInfo = pVerison.VersionLocks;
            ILockInfo pLockInfo = pEnumLockInfo.Next();
            do
            {
                if (pLockInfo != null)
                    return true;
            }
            while ((pLockInfo = pEnumLockInfo.Next()) != null);

            return false;
        }

        /// <summary>
        /// 提交当前版本(经过仲裁)
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="sVersion">父版本名称</param>
        public static void PostVerison(IWorkspace pWorkspace, string sVersion)
        {
            IVersion pVersion = (IVersion)pWorkspace;
            if (!IsLocked(pVersion))
                throw new Exception("不能提交数据到父版本,其它用户正在编辑");

            IWorkspaceEdit pWorkspaceEdit = (IWorkspaceEdit)pVersion;
            if (!pWorkspaceEdit.IsBeingEdited())
                throw new Exception("请开始编辑会话");

            IVersionEdit pVersionEdit = (IVersionEdit)pVersion;
            IVersionInfo pVersionInfo = pVersion.VersionInfo;
            try
            {
                string sParentName = pVersionInfo.VersionName;
                bool bConflicts = pVersionEdit.Reconcile(sParentName);
                if (bConflicts)
                {
                    pWorkspaceEdit.StopEditing(true);
                }
                else
                {
                    if (pVersionEdit.CanPost())
                    {
                        pVersionEdit.Post(sParentName);
                        pWorkspaceEdit.StopEditing(true);
                    }
                    else
                    {
                        pWorkspaceEdit.StopEditing(true); // false
                        //MessageBox.Show("不能提交数据到父版本[" + sParentName + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pWorkspaceEdit.IsBeingEdited())
                {
                    pWorkspaceEdit.StopEditing(true); // false
                }
                pWorkspaceEdit.StartEditing(true);
            }
        }

        /// <summary>
        /// 提交当前版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="VersionName">父版本名称</param>
        /// <returns>是否提交成功(经过仲裁)</returns>
        public static bool PostVersion(IWorkspace pWorkspace, string VersionName)
        {
            IVersionEdit pVersionEdit = (IVersionEdit)pWorkspace;
            pVersionEdit.Reconcile(VersionName);
            pVersionEdit.Post(VersionName);
            return true;
        }

        /// <summary>
        /// 仲裁版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="VersionName">版本名称</param>
        /// <returns>是否(产生冲突)经过仲裁</returns>
        public static bool ReconcileVersion(IWorkspace pWorkspace, string VersionName)
        {
            IVersionEdit pVersionEdit = (IVersionEdit)pWorkspace;
            return pVersionEdit.Reconcile(VersionName);
        }

        /// <summary>
        /// 改变版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="sVersionName">版本名称</param>
        public static void ChangeVersion(IWorkspace pWorkspace, string sVersionName)
        {
            IVersionedWorkspace pVersionedWorkspace;
            if (!(pWorkspace is IVersionedWorkspace))
                return;

            pVersionedWorkspace = (IVersionedWorkspace)pWorkspace;
            IVersion pVersion = pVersionedWorkspace.FindVersion(sVersionName);
            //ChangeVersion(pWorkspace, pVersion);
        }

        /// <summary>
        ///  删除当前版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="VersionName">版本名称</param>
        /// <returns></returns>
        public static bool DeleteVersion(IWorkspace pWorkspace, string VersionName)
        {
            IVersionedWorkspace pVerWorkspace = (IVersionedWorkspace)pWorkspace;
            IVersion pVersion = pVerWorkspace.FindVersion(VersionName);
            pVersion.Delete();
            return true;
        }

        /// <summary>
        /// 建立版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="VersionName">版本名称</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateVersion(IWorkspace pWorkspace, string VersionName)
        {
            IVersion pVersion = (IVersion)pWorkspace;
            pVersion.CreateVersion(VersionName);
            return true;
        }

        /// <summary>
        /// 注册\反注册版本
        /// </summary>
        /// <param name="pWorkspace">工作区</param>
        /// <param name="bRegist">注册\反注册版本</param>
        public static void RegistVersion(IWorkspace pWorkspace, bool bRegist)
        {
            IVersionedObject verObj = (IVersionedObject)pWorkspace;
            if (verObj != null && (!verObj.IsRegisteredAsVersioned))
            {
                // 数据集可以被注册而且还没有被注册为版本数据集
                // 下面的方法 如果使用参数 false 那么表示注册为没有版本的数据也就是反注册
                verObj.RegisterAsVersioned(bRegist);
                // sde.Default
            }
        }
    }
}
