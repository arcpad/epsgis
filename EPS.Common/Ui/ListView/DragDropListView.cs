using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace EPS.Ui.ListView
{
    /// <summary>
    /// 支持ListView的Item的拖动
    /// </summary>
    public class DragDropListView : System.Windows.Forms.ListView
    {
        private const int WM_PAINT = 0x000F;

        /// <summary>
        /// 拖动开关

        /// </summary>
        private bool m_bAllowReorder = true;
        public bool AllowReorder
        {
            get
            {
                return this.m_bAllowReorder;
            }
            set
            {
                this.m_bAllowReorder = value;
                base.AllowDrop = value;
            }
        }

        /// <summary>
        /// 屏蔽自动排序功能
        /// </summary>
        public new SortOrder Sorting
        {
            get
            {
                return SortOrder.None;
            }
            set
            {
                base.Sorting = SortOrder.None;
            }
        }

        /// <summary>
        /// 屏蔽View风格
        /// </summary>
        public new View View
        {
            get { return View.Details; }
            set { base.View = View.Details; }
        }
        
        public DragDropListView()
            : base()
        {
            // Reduce flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.View = View.Details;
            this.FullRowSelect = true;
            this.AllowReorder = true;
        }

        // 拖动结束
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            if (!this.AllowReorder)
                return;

            if (base.SelectedItems.Count == 0)
                return;

            Point cp = base.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = base.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
                return;

            int dropIndex = DropIndex;
            ArrayList insertItems =
                new ArrayList(base.SelectedItems.Count);
            foreach (ListViewItem item in base.SelectedItems)
                insertItems.Add(item.Clone());

            // 添加
            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                ListViewItem insertItem = (ListViewItem)insertItems[i];
                base.Items.Insert(dropIndex, insertItem);
            }
            // 删除
            foreach (ListViewItem removeItem in base.SelectedItems)
            {
                base.Items.Remove(removeItem);
            }
            // 选中
            for (int i = 0; i < insertItems.Count; i++)
            {
                ListViewItem insertItem = (ListViewItem)insertItems[i];
                insertItem.Selected = true;
            }
            DropIndex = -1;
            this.Invalidate();
        }

        // 拖动过程
        protected override void OnDragOver(DragEventArgs e)
        {
            if (!this.AllowReorder)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            try
            {
                // 计算插入位置
                Point cp = base.PointToClient(new Point(e.X, e.Y));
                ListViewItem hoverItem = base.GetItemAt(cp.X, cp.Y);
                if (hoverItem == null)
                {
                    e.Effect = DragDropEffects.None;
                    DropIndex = -1;
                    return;
                }
                base.EnsureVisible(hoverItem.Index);
                DropIndex = hoverItem.Index;
                Rectangle rc = hoverItem.GetBounds(ItemBoundsPortion.Entire);
                if (cp.Y > rc.Top + (rc.Height / 2))
                {
                    DropIndex = hoverItem.Index + 1;
                }

                // 选集中的位置不可插入
                foreach (ListViewItem moveItem in base.SelectedItems)
                {
                    if (moveItem.Index == hoverItem.Index)
                    {
                        e.Effect = DragDropEffects.None;
                        hoverItem.EnsureVisible();
                        DropIndex = -1;
                        return;
                    }
                }
                base.OnDragOver(e);
                e.Effect = DragDropEffects.Move;
            }
            finally
            {
                this.Invalidate();
            }
        }

        // 开始拖动
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            DropIndex = -1;
            if (!this.AllowReorder)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            base.OnDragEnter(e);
            e.Effect = DragDropEffects.Move;
        }

        // 开启拖动
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            if (!this.AllowReorder)
                return;

            base.DoDragDrop("", DragDropEffects.Move);
        }

        // 目标位置
        private int DropIndex = -1;
       
        // 自绘
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // We have to take this way (instead of overriding OnPaint()) because the ListView is just a wrapper
            // around the common control ListView and unfortunately does not call the OnPaint overrides.
            if (m.Msg == WM_PAINT)
            {
                if (DropIndex >= 0 && DropIndex < Items.Count)
                {
                    Rectangle rc =
                        Items[DropIndex].GetBounds(ItemBoundsPortion.Entire);
                    DrawInsertionLine(rc.Left, rc.Right, rc.Top);
                }
                else if (DropIndex == Items.Count)
                {
                    Rectangle rc =
                        Items[DropIndex - 1].GetBounds(ItemBoundsPortion.Entire);
                    DrawInsertionLine(rc.Left, rc.Right, rc.Bottom);
                }
            }
        }

        /// <summary>
        /// Draw a line with insertion marks at each end
        /// </summary>
        /// <param name="X1">Starting position (X) of the line</param>
        /// <param name="X2">Ending position (X) of the line</param>
        /// <param name="Y">Position (Y) of the line</param>
        private void DrawInsertionLine(int X1, int X2, int Y)
        {
            using (Graphics g = this.CreateGraphics())
            {
                g.DrawLine(Pens.Red, X1, Y, X2 - 1, Y);

                Point[] leftTriangle = new Point[3] {
                            new Point(X1,      Y-4),
                            new Point(X1 + 7,  Y),
                            new Point(X1,      Y+4)
                        };
                Point[] rightTriangle = new Point[3] {
                            new Point(X2,     Y-4),
                            new Point(X2 - 8, Y),
                            new Point(X2,     Y+4)
                        };
                g.FillPolygon(Brushes.Red, leftTriangle);
                g.FillPolygon(Brushes.Red, rightTriangle);
            }
        }
    }
}
