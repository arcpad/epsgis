using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EPS.Main
{
    /// <summary>
    /// <br>LoginForm</br>
    /// <br>[功能描述: 登录界面UC]</br>
    /// <br>yuyang</br>
    public partial class LoginForm : Form
    {
        #region 变量

        //// 获取公用的数据连接
        //protected DataAccessBase _DataAccessBase;
        //public DataAccessBase GetDataAccessConnection
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ConfigHelper.GetDecryptConfig("ConnectionStringUnieap")))
        //        {
        //            MessageBox.Show("数据库连接未配置，请配置数据库连接");
        //            return null;
        //        }

        //        if (_DataAccessBase == null)
        //            _DataAccessBase = DataAccessFactory.CreateDataAccess("ConnectionStringUnieap");
        //        return _DataAccessBase;
        //    }
        //}

        #endregion

        #region 属性

        #endregion

        #region 方法

        /// <summary>
        /// 检查是否合法
        /// </summary>
        /// <returns></returns>
        protected bool IsValid()
        {
            if (this.txtAccount.Text.Trim() == string.Empty)
            {
                MessageBox.Show("帐号不允许为空！");
                return false;
            }

            if (CheckBadChar(this.txtAccount.Text.Trim()) || IsHaveSqlKeyword(this.txtAccount.Text.Trim()))
            {
                MessageBox.Show("帐号中含义非法字符！");
                return false;
            }

            if (CheckBadChar(this.txtPassword.Text.Trim()) || IsHaveSqlKeyword(this.txtPassword.Text.Trim()))
            {
                MessageBox.Show("密码中含义非法字符！");
                return false;
            }

            return true;
        }

        /// <summary> 
        /// 检查是否含有非法字符 
        /// </summary> 
        /// <param name="str">要检查的字符串 </param> 
        /// <returns> </returns> 
        public bool CheckBadChar(string str)
        {
            bool result = false;
            if (string.IsNullOrEmpty(str))
                return result;
            string tempChar;
            string[] arrBadChar ={ "=", "'", ";", ",", "(", ")", "%", "-", "#", "select" };
            tempChar = str;
            for (int i = 0; i < arrBadChar.Length; i++)
            {
                if (tempChar.IndexOf(arrBadChar[i]) >= 0)
                    result = true;
            }
            return result;
        }

        /// <summary>
        /// 检查是否有SQL关键字
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public bool IsHaveSqlKeyword(string Str)
        {
            bool ReturnValue = false;
            try
            {
                if (Str != "")
                {
                    string SqlStr = "select*|and'|or'|insertinto|deletefrom|altertable|update|createtable|createview|dropview|createindex|dropindex|createprocedure|dropprocedure|createtrigger|droptrigger|createschema|dropschema|createdomain|alterdomain|dropdomain|);|select@|declare@|print@|char(|select";
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (Str.IndexOf(ss) >= 0)
                        {
                            ReturnValue = true;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = true;
            }
            return ReturnValue;
        }

        /// <summary>
        /// 是否具有登录权限
        /// </summary>
        /// <returns></returns>
        protected bool IsHaveRight(ref string strPsnName)
        {
            //string strAccount = this.txtAccount.Text.Trim();
            //string strPassword = this.txtPassword.Text.Trim();

            //string strCountBaseSql = "SELECT COUNT(PSN_ID) FROM ORG_PERSON WHERE PSN_ACCOUNT='{0}'";
            //string strCountQuerySql = string.Format(strCountBaseSql, strAccount);

            //if (this.GetDataAccessConnection != null)
            //{
            //    object countResult = this.GetDataAccessConnection.ExecuteScalar(strCountQuerySql);
            //    if (countResult != null)
            //    {
            //        int count = Int32.Parse(countResult.ToString());
            //        if (count <= 0)
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //    string strBaseSql = "SELECT PSN_PWD,PSN_NAME FROM ORG_PERSON WHERE PSN_ACCOUNT='{0}'";
            //    string strQuerySql = string.Format(strBaseSql, strAccount);
            //    string strPasswordResult = string.Empty;
            //    DataSet ds = this.GetDataAccessConnection.ExecuteDataSet(strQuerySql);
            //    DataTable dt = ds.Tables[0];
            //    if (dt.Rows.Count > 0)
            //    {
            //        DataRow dr = dt.Rows[0];
            //        strPasswordResult = dr[0] == null ? string.Empty : dr[0].ToString();
            //        strPsnName = dr[1] == null ? string.Empty : dr[1].ToString();
            //    }

            //    string strPasswordDecoded = NeuSoft.GIS.EDPassword.PasswordTool.DecodePasswd(strPasswordResult);
            //    if (strPassword != strPasswordDecoded)
            //    {
            //        MessageBox.Show("用户名或密码错误！");
            //        return false;
            //    }

            //    if ("超级管理员" == strPsnName)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("权限不足，无法登陆！");
            //        return false;
            //    }
            //}

            return false;
        }

        public LoginForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 事件
        private void btnOK_Click(object sender, EventArgs e)
        {
            //if (IsValid())
            //{
            //    string strPsnName = string.Empty;
            //    if (IsHaveRight(ref strPsnName))
            //    {
            //        frmMain frm = new frmMain();
            //        this.Hide();
            //        this.Visible = false;

            //        try
            //        {
            //            SysLogDao daoLog = new SysLogDao();
            //            daoLog.InsertLoginInfo(txtAccount.Text, strPsnName);
            //        }
            //        catch (Exception ex)
            //        {
            //            Program.Log.Error(ex.Message);
            //            Program.Log.Error(ex.StackTrace);
            //        }


            //        System.Windows.Forms.DialogResult reusult = frm.ShowDialog();

            //        this.Close();
            //    }
            //}

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //frmServerConfig frmConfig = new frmServerConfig();
            //frmConfig.ShowDialog();
        }

        #endregion

    }
}