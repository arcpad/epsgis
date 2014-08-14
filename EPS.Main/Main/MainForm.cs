using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS;
using EPS.Engine.TOC;
using EPS.Engine.Map;
using EPS.Engine.Command;
using EPS.Utilities;
using EPS.Data;
using EPS.Utils;
using EPS.Main.Tools;
using EPS.Biz;

namespace EPS.Main
{
    public partial class MainForm : RibbonForm
    {
        private ITOCControl2 m_tocControl;
        private IMapControl3 m_mapControl = null;
        private IPageLayoutControl2 m_pageLayoutControl = null;
        private ControlsSynchronizer m_controlsSynchronizer = null;
        private string m_documentFileName = string.Empty;
        private IToolbarMenu m_menuMap;
        private IToolbarMenu m_menuLayer;

        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            m_instance = this;
        }

        public IMapControl3 MapControl {
            get { return m_mapControl; }
        }

        public ESRI.ArcGIS.Controls.AxMapControl AxMapCtrl
        {
            get { return axMapControl1; }
        }

        private static MainForm m_instance;
        public static MainForm Instance {
            get { return m_instance; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private Color GetRandomColor(Random r)
        {
            if (r == null)
            {
                r = new Random(DateTime.Now.Millisecond);
            }

            return Color.FromKnownColor((KnownColor)r.Next(1, 150));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //get a reference to the MapControl and the PageLayoutControl
            m_tocControl = (ITOCControl2)axTOCControl1.Object;
            m_mapControl = (IMapControl3)axMapControl1.Object;
            m_pageLayoutControl = (IPageLayoutControl2)axPageLayoutControl1.Object;

            //initialize the controls synchronization class
            m_controlsSynchronizer = new ControlsSynchronizer(m_mapControl, m_pageLayoutControl);

            //bind the controls together (both point at the same map) and set the MapControl as the active control
            m_controlsSynchronizer.BindControls(true);

            //add the framework controls (TOC and Toolbars) in order to synchronize then when the
            //active control changes (call SetBuddyControl)
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl1.Object);
            m_controlsSynchronizer.AddFrameworkControl(axTOCControl1.Object);

            //add the Open Map Document command onto the toolbar
            OpenNewMapDocument openMapDoc = new OpenNewMapDocument(m_controlsSynchronizer);
            axToolbarControl1.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);

            //Add custom commands to the map menu
            m_menuMap = new ToolbarMenuClass();
            m_menuMap.AddItem(new LayerVisibility(), 1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuMap.AddItem(new LayerVisibility(), 2, 1, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuMap.AddItem(new BufferAnalysis(), 3, 2, false, esriCommandStyles.esriCommandStyleTextOnly);
            
            //Add pre-defined menu to the map menu as a sub menu 
            m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 2, true);
            //Add custom commands to the map menu
            m_menuLayer = new ToolbarMenuClass();
            m_menuLayer.AddItem(new ZoomToLayer(), -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new RemoveLayer(), -1, 1, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 1, 2, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 2, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new ScaleThresholds(), 3, 4, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerSelectable(), 1, 5, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerSelectable(), 2, 6, false, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerRendering(), -1, 7, true, esriCommandStyles.esriCommandStyleTextOnly);
            m_menuLayer.AddItem(new LayerProperties(), -1, 8, false, esriCommandStyles.esriCommandStyleTextOnly);

            axToolbarControl1.AddItem(new IdentifyTool(), -1, 0, false, 0, esriCommandStyles.esriCommandStyleIconOnly); 
            
            
            //Set the hook of each menu
            m_menuLayer.SetHook(m_mapControl);
            m_menuMap.SetHook(m_mapControl);
        }

        private void ribbonOrbOptionButton1_Click(object sender, EventArgs e)
        {
            try
            {
                User user = DbUtils.Get<User>("GetById", 2);
                MessageBox.Show(user.userName);
            }
            catch (Exception ex) 
            {
                Logger.Debug(ex.Message);
            }

            return;
            // EPSUtils.RendererFeatureLayer(null, true);
            // Application.Exit();
            // Close();
        }

        private void ribbonButton2_Click(object sender, EventArgs e)
        {
           //Console.WriteLine("clicked");
           // TestForm t = new TestForm();
           // t.ShowDialog(this);
        }

        private void ribbonButton5_Click(object sender, EventArgs e)
        {
           //Console.WriteLine("clicked");
           // TestForm t = new TestForm();
           // t.Show(this);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) //map view
            {
                //activate the MapControl and deactivate the PageLayoutControl
                m_controlsSynchronizer.ActivateMap();
            }
            else //layout view
            {
                //activate the PageLayoutControl and deactivate the MapControl
                m_controlsSynchronizer.ActivatePageLayout();
            }
        }

        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            statusBarXYUnits.Text = string.Format("{0} {1} {2}", e.mapX.ToString("#######.###"), e.mapY.ToString("#######.###"), axMapControl1.MapUnits.ToString().Substring(4));
        }

        private void axPageLayoutControl1_OnMouseMove(object sender, IPageLayoutControlEvents_OnMouseMoveEvent e)
        {
            statusBarXYUnits.Text = string.Format("{0} {1} {2}", e.pageX.ToString("###.##"), e.pageY.ToString("###.##"), axPageLayoutControl1.Page.Units.ToString().Substring(4));
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button != 2) return;

            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null; ILayer layer = null;
            object other = null; object index = null;

            //Determine what kind of item is selected
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);

            //Ensure the item gets selected 
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_tocControl.SelectItem(map, null);
            else
                m_tocControl.SelectItem(layer, null);

            //Set the layer into the CustomProperty (this is used by the custom layer commands)			
            m_mapControl.CustomProperty = layer;

            //Popup the correct context menu
            if (item == esriTOCControlItem.esriTOCControlItemMap) m_menuMap.PopupMenu(e.x + 3, e.y + 1, m_tocControl.hWnd);
            if (item == esriTOCControlItem.esriTOCControlItemLayer) m_menuLayer.PopupMenu(e.x + 3, e.y + 1, m_tocControl.hWnd);
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void OnProjectMgr(object sender, EventArgs e)
        {
            ProjectManageForm form = new ProjectManageForm();
            form.ShowDialog(this);
        }
    }
}