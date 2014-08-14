using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using log4net;

namespace EPS.Utils
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    /// <author>hao.w</author>
    public class Logger
    {
        // public static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog logInfo = LogManager.GetLogger("INFO");
        public static ILog logError = LogManager.GetLogger("ERROR");
        public static ILog logDebug = LogManager.GetLogger("DEBUG");

        /// <summary>
        /// 本地错误日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(String msg)
        {
            logError.Error(msg);
        }

        /// <summary>
        /// 本地消息日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(String msg)
        {
            logInfo.Info(msg);
        }

        /// <summary>
        /// 本地调试日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(String msg)
        {
            logDebug.Debug(msg);
        }

        /// <summary>
        /// 数据库错误日志
        /// </summary>
        /// <param name="msg"></param>
        public static void DbError(String msg)
        {
            // logging error messages to database
        }

        /// <summary>
        /// 数据库消息日志
        /// </summary>
        /// <param name="msg"></param>
        public static void DbInfo(String msg)
        {
            // logging info messages to database
        }
    }
}