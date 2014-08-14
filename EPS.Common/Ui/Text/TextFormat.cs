using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EPS.Ui.Text
{
    /// <summary>
    /// 数据操作类
    /// </summary>
    /// <author>hao.w</author>
    public enum TextFormat
    {
        /// <summary>
        /// 数字类型
        /// </summary>
        Digital = 0,

        /// <summary>
        /// 整数，包含负数
        /// </summary>
        Integer = 1,

        /// <summary>
        /// 数字，包含小数点(.)、负数(-)
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// 允许数字、字母,不允许空格
        /// </summary>
        Word = 3,

        /// <summary>
        /// 允许数字、字母、许空格
        /// </summary>
        MutilWord = 4,

        /// <summary>
        /// 描述，不限输入
        /// </summary>
        Description = 5
    }
}