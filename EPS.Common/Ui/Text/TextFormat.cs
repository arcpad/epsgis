using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace EPS.Ui.Text
{
    /// <summary>
    /// ���ݲ�����
    /// </summary>
    /// <author>hao.w</author>
    public enum TextFormat
    {
        /// <summary>
        /// ��������
        /// </summary>
        Digital = 0,

        /// <summary>
        /// ��������������
        /// </summary>
        Integer = 1,

        /// <summary>
        /// ���֣�����С����(.)������(-)
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// �������֡���ĸ,������ո�
        /// </summary>
        Word = 3,

        /// <summary>
        /// �������֡���ĸ����ո�
        /// </summary>
        MutilWord = 4,

        /// <summary>
        /// ��������������
        /// </summary>
        Description = 5
    }
}