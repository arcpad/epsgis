using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using log4net;

namespace EPS.Utils
{
    /// <summary>
    /// ��־������
    /// </summary>
    /// <author>hao.w</author>
    public class Logger
    {
        // public static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog logInfo = LogManager.GetLogger("INFO");
        public static ILog logError = LogManager.GetLogger("ERROR");
        public static ILog logDebug = LogManager.GetLogger("DEBUG");

        /// <summary>
        /// ���ش�����־
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(String msg)
        {
            logError.Error(msg);
        }

        /// <summary>
        /// ������Ϣ��־
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(String msg)
        {
            logInfo.Info(msg);
        }

        /// <summary>
        /// ���ص�����־
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(String msg)
        {
            logDebug.Debug(msg);
        }

        /// <summary>
        /// ���ݿ������־
        /// </summary>
        /// <param name="msg"></param>
        public static void DbError(String msg)
        {
            // logging error messages to database
        }

        /// <summary>
        /// ���ݿ���Ϣ��־
        /// </summary>
        /// <param name="msg"></param>
        public static void DbInfo(String msg)
        {
            // logging info messages to database
        }
    }
}