using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace EPS.Ui.Text
{
    /// <summary>
    /// 文本框
    /// </summary>
    /// <author>hao.w</author>
    [System.Drawing.ToolboxBitmap(typeof(System.Windows.Forms.TextBox))]
    public class EpsTextBox : System.Windows.Forms.TextBox
    {
        /// <summary>
        /// 字符串格式
        /// </summary>
        private TextFormat textFormat = TextFormat.Description;

        /// <summary>
        /// 字符串格式
        /// </summary>
        [Description("字符串格式"), Category("Behavior"), DefaultValue(TextFormat.Description)]
        public TextFormat TextFormt
        {
            get { return textFormat; }
            set { textFormat = value; }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (textFormat != TextFormat.Description)
            {
                switch (textFormat)
                {
                    case TextFormat.Integer:
                    case TextFormat.Decimal:
                        if (e.KeyValue == 189 || e.KeyValue == 109)
                        {
                            int num = base.Text.IndexOf('-');
                            if (num > 0)
                            {
                                base.Text = base.Text.Remove(num, 1);
                                base.SelectionStart = num;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (textFormat != TextFormat.Description)
            {
                switch (textFormat)
                {
                    case TextFormat.Digital:
                        if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
                        {
                            e.Handled = true;
                        }
                        break;
                    case TextFormat.Integer:
                        if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar) && (e.KeyChar != '-' || e.KeyChar == '-' && base.Text.IndexOf('-') != 0))
                        {
                            e.Handled = true;
                        }
                        break;
                    case TextFormat.Decimal:
                        if (e.KeyChar != '-')
                        {
                            if (e.KeyChar != '\b'
                                && (e.KeyChar != '.' || e.KeyChar == '.' && base.Text.IndexOf('.') > 0)
                                && !Char.IsDigit(e.KeyChar))
                            {
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            string temp = base.Text;

                            if (e.KeyChar != '-' || e.KeyChar == '-' && base.Text.IndexOf('-') == 0)
                            {
                                e.Handled = true;
                            }
                            System.Windows.Forms.Message m = new System.Windows.Forms.Message();
                            m.HWnd = this.Handle;
                            m.Msg = 0x103;
                            base.WndProc(ref m);
                            if (temp.IndexOf("-") > 0)
                            {
                                base.Text = temp;
                            }
                        }
                        break;
                    case TextFormat.Word:
                        if (e.KeyChar != '\b' && Char.IsPunctuation(e.KeyChar) || Char.IsSymbol(e.KeyChar) || Char.IsWhiteSpace(e.KeyChar))
                        {
                            e.Handled = true;
                        }
                        break;
                    case TextFormat.MutilWord:
                        if (e.KeyChar != '\b' && Char.IsPunctuation(e.KeyChar) && e.KeyChar != '_' || Char.IsSymbol(e.KeyChar))
                        {
                            e.Handled = true;
                        }
                        break;
                    case TextFormat.Description:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}