using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace EPS.Ui.Loading
{
    public partial class LoadingForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern void FlashWindowEx(IntPtr hWnd, bool isMax);

        Control parentControl = null;

        public delegate void ShowFormEvent();

        public ShowFormEvent ShowFormDialog;

        public LoadingForm(Control control)
        {
            InitializeComponent();
            this.ShowFormDialog = this.ShowForm;
            this.parentControl = control;
            this.parentControl.Enter += new EventHandler(parentControl_GotFocus);
        }

        void parentControl_GotFocus(object sender, EventArgs e)
        {
            this.Activate();
        }

        public void ShowForm()
        {
            this.Show(this.parentControl);
            Application.DoEvents();
            this.Activate();
            NativeWindow window = NativeWindow.FromHandle(this.Handle);

            //FlashWindowEx(this.Handle, true);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
        }

        protected override void OnCreateControl()
        {
            base.CenterToParent();
            base.OnCreateControl();
        }

        private void frmLoadingBaseForm_Leave(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Select();
        }
    }
}