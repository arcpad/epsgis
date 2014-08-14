using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace EPS.Ui.ListView
{
    /// <summary>
    /// OwnerDraw for ListView Ctrl
    /// </summary>
    public class ListViewEx : System.Windows.Forms.ListView
    {
        private System.ComponentModel.Container components = null;

        private Control _editingControl;
        private ListViewItem _editItem;
        private int _editSubItem;
        private bool[] _boolcols;
        public void SetBoolColumn(int col, bool check)
        {
            if (_boolcols == null)
                _boolcols = new bool[this.Columns.Count];

            _boolcols[col] = check;
        }

        public bool[] BoolColumn
        {
            get { return _boolcols; }
        }

        public ListViewEx()
        {
            InitializeComponent();

            this.DoubleBuffered = true; // kill nasty flickers. hiss... me hates 'em
            base.FullRowSelect = true;
            base.View = View.Details;
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            ListViewItem item;
            ListViewItem.ListViewSubItem subitem = null;
            subitem = GetSubItemAt(e.X, e.Y, out item);
            if (subitem != null)
            {
                if (subitem.Tag == "0")
                    subitem.Tag = "1";
                else
                    subitem.Tag = "0";

                Invalidate(subitem.Bounds);
            }

            base.OnMouseUp(e);
        }
        public ListViewItem.ListViewSubItem GetSubItemAt(int x, int y, out ListViewItem item)
        {
            item = this.GetItemAt(x, y);

            ListViewHitTestInfo hitinf = this.HitTest(new Point(x, y));
            item = hitinf.Item;
            return hitinf.SubItem;
        }
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (this.FullRowSelect)
            {
                // draw select item
                if (this.SelectedItems.Count == 1)
                {
                    if (e.Item == this.SelectedItems[0])
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.LightBlue), e.Bounds);
                    }
                }
            }
            else
            { 
            
            }
            base.OnDrawItem(e);
        }
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            for (int i = 0; i < BoolColumn.Length; i++)
            {
                if (e.ColumnIndex == i)
                {
                    if (BoolColumn[i])
                    {
                        DrawSubItem(e.Graphics, e.SubItem);
                    }
                    else
                    {
                        e.DrawText();
                    }
                    break;
                }
            }
            base.OnDrawSubItem(e);
        }
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }
        protected virtual void DrawSubItem(Graphics g, ListViewItem.ListViewSubItem subitem)
        {
            //Graphics g = Graphics.FromHwnd(this.Handle);
            Rectangle bound = subitem.Bounds;
            bound.Width = Convert.ToInt32(bound.Width / 2);
            bound.X += bound.Width - 4;
            //bound.Offset(6, 0);
            System.Drawing.Font font = new Font(this.Font.FontFamily, 11);
            //g.DrawString("¡õ", font, new SolidBrush(Color.Black), bound);
            if (subitem.Tag == "0")
            {
                g.DrawString("¡õ", font, new SolidBrush(Color.Black), bound);
            }
            else if (subitem.Tag == "1")
            {
                g.DrawString("¡õ", font, new SolidBrush(Color.Black), bound);
                g.DrawString("¡Ì", this.Font, new SolidBrush(Color.Black), bound);
            }
        }
    }
}
