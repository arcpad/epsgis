using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EPS.Engine.Utils;
using ESRI.ArcGIS.Geometry;

namespace EPS.Biz
{
    public partial class ProjectManageForm : Form
    {
        public ProjectManageForm()
        {
            InitializeComponent();
        }

        private void buttonAddProject_Click(object sender, EventArgs e)
        {
            ProjectEditForm form = new ProjectEditForm();
            form.Text = "创建项目";
            form.ShowDialog(this);
        }

        private void buttonEditProject_Click(object sender, EventArgs e)
        {
            ProjectEditForm form = new ProjectEditForm();
            form.Text = "编辑项目";
            form.ShowDialog(this);
        }

        private void buttonDelProject_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除选定项目吗？", "确认", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) 
            {
                return;
            }
            DoQuery();
        }

        private void buttonQuery_Click(object sender, EventArgs e)
        {
            DoQuery();
        }

        private void DoQuery()
        {
            listViewProject.Items.Clear();
        }

        /// <summary>
        /// STUB 需求调研
        /// </summary>
        public void LocateByProject() 
        {
            // 地图定位
            IEnvelope envelope = new EnvelopeClass();
            envelope = MapUtils.GetActiveView().Extent.Envelope;
            envelope.XMax += 1000;
            MapUtils.GetActiveView().Extent = envelope;
        }

        private void buttonGoto_Click(object sender, EventArgs e)
        {
            LocateByProject();
            Close();
        }

        private void buttonZoneQuery_Click(object sender, EventArgs e)
        {

        }
    }
}
