// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at &lt;your ArcGIS install location&gt;/DeveloperKit10.0/userestrictions.txt.
// 

using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;

namespace ESP.Main.Engine.Toc.Renderer
{

	public class RendererForm : System.Windows.Forms.Form
	{
		private ESRI.ArcGIS.Controls.AxSymbologyControl axSymbologyControl1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		private System.ComponentModel.Container components = null;
		public IStyleGalleryItem m_styleGalleryItem;

		public RendererForm()
		{

			InitializeComponent();

		}


		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RendererForm));
            this.axSymbologyControl1 = new ESRI.ArcGIS.Controls.AxSymbologyControl();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.axSymbologyControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // axSymbologyControl1
            // 
            this.axSymbologyControl1.Location = new System.Drawing.Point(10, 9);
            this.axSymbologyControl1.Name = "axSymbologyControl1";
            this.axSymbologyControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axSymbologyControl1.OcxState")));
            this.axSymbologyControl1.Size = new System.Drawing.Size(364, 285);
            this.axSymbologyControl1.TabIndex = 0;
            this.axSymbologyControl1.OnItemSelected += new ESRI.ArcGIS.Controls.ISymbologyControlEvents_Ax_OnItemSelectedEventHandler(this.axSymbologyControl1_OnItemSelected);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(384, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(134, 103);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(384, 267);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(115, 26);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "确定";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(384, 233);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(115, 25);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "取消";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // RendererForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(522, 299);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.axSymbologyControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RendererForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "设置渲染符号";
            this.Load += new System.EventHandler(this.frmSymbol_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axSymbologyControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void frmSymbol_Load(object sender, System.EventArgs e)
		{
			//Get the ArcGIS install location
            string sInstall = ESRI.ArcGIS.RuntimeManager.ActiveRuntime.Path;

			//Load the ESRI.ServerStyle file into the SymbologyControl
			axSymbologyControl1.LoadStyleFile(sInstall + "\\Styles\\ESRI.ServerStyle");
		}

		private void cmdCancel_Click(object sender, System.EventArgs e)
		{
			m_styleGalleryItem = null;
            // this.Close();
			this.Hide();
		}

		private void cmdOK_Click(object sender, System.EventArgs e)
		{
            // this.Close();
			this.Hide();
		}

		private void axSymbologyControl1_OnItemSelected(object sender, ESRI.ArcGIS.Controls.ISymbologyControlEvents_OnItemSelectedEvent e)
		{
			//Preview the selected item
			m_styleGalleryItem = (IStyleGalleryItem) e.styleGalleryItem;
			PreviewImage();
		}

		private void PreviewImage()
		{
			//Get and set the style class 
			ISymbologyStyleClass symbologyStyleClass = axSymbologyControl1.GetStyleClass(axSymbologyControl1.StyleClass);

			//Preview an image of the symbol
			stdole.IPictureDisp picture = symbologyStyleClass.PreviewItem(m_styleGalleryItem, pictureBox1.Width, pictureBox1.Height);
			System.Drawing.Image image = System.Drawing.Image.FromHbitmap(new System.IntPtr(picture.Handle));
			pictureBox1.Image = image;
		}

		public IStyleGalleryItem GetItem(esriSymbologyStyleClass styleClass, ISymbol symbol)
		{
			m_styleGalleryItem = null;

			//Get and set the style class
			axSymbologyControl1.StyleClass = styleClass;
			ISymbologyStyleClass symbologyStyleClass = axSymbologyControl1.GetStyleClass(styleClass);

			//Create a new server style gallery item with its style set
			IStyleGalleryItem styleGalleryItem = new ServerStyleGalleryItem();
			styleGalleryItem.Item = symbol;
			styleGalleryItem.Name = "mySymbol";

			//Add the item to the style class and select it
			symbologyStyleClass.AddItem(styleGalleryItem, 0);
			symbologyStyleClass.SelectItem(0);

			//Show the modal form
			this.ShowDialog();

			return m_styleGalleryItem;
		}
	}
}
