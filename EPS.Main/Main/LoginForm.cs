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
    /// <br>[��������: ��¼����UC]</br>
    /// <br>yuyang</br>
    public partial class LoginForm : Form
    {
        #region ����

        //// ��ȡ���õ���������
        //protected DataAccessBase _DataAccessBase;
        //public DataAccessBase GetDataAccessConnection
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ConfigHelper.GetDecryptConfig("ConnectionStringUnieap")))
        //        {
        //            MessageBox.Show("���ݿ�����δ���ã����������ݿ�����");
        //            return null;
        //        }

        //        if (_DataAccessBase == null)
        //            _DataAccessBase = DataAccessFactory.CreateDataAccess("ConnectionStringUnieap");
        //        return _DataAccessBase;
        //    }
        //}

        #endregion

        #region ����

        #endregion

        #region ����

        /// <summary>
        /// ����Ƿ�Ϸ�
        /// </summary>
        /// <returns></returns>
        protected bool IsValid()
        {
            if (this.txtAccount.Text.Trim() == string.Empty)
            {
                MessageBox.Show("�ʺŲ�����Ϊ�գ�");
                return false;
            }

            if (CheckBadChar(this.txtAccount.Text.Trim()) || IsHaveSqlKeyword(this.txtAccount.Text.Trim()))
            {
                MessageBox.Show("�ʺ��к���Ƿ��ַ���");
                return false;
            }

            if (CheckBadChar(this.txtPassword.Text.Trim()) || IsHaveSqlKeyword(this.txtPassword.Text.Trim()))
            {
                MessageBox.Show("�����к���Ƿ��ַ���");
                return false;
            }

            return true;
        }

        /// <summary> 
        /// ����Ƿ��зǷ��ַ� 
        /// </summary> 
        /// <param name="str">Ҫ�����ַ��� </param> 
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
        /// ����Ƿ���SQL�ؼ���
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
        /// �Ƿ���е�¼Ȩ��
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
            //        MessageBox.Show("�û������������");
            //        return false;
            //    }

            //    if ("��������Ա" == strPsnName)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Ȩ�޲��㣬�޷���½��");
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

        #region �¼�
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
        /// ����
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