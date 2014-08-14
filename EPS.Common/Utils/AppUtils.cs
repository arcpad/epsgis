using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace EPS.Utils
{
    // 以下非托管函数，定义根据名称查询msdn
    [StructLayout(LayoutKind.Sequential)]
    public struct ICONINFO
    {
        public bool fIcon;
        public Int32 xHotspot;
        public Int32 yHotspot;
        public IntPtr hbmMask;
        public IntPtr hbmColor;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public Int32 x;
        public Int32 y;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CURSORINFO
    {
        public Int32 cbSize;
        public Int32 flags;
        public IntPtr hCursor;
        public POINT ptScreenPos;
    }

    /// <summary>
    /// 定义了辅助键的名称(将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值)
    /// </summary>
    [Flags()]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Ctrl = 2,
        Shift = 4,
        WindowsKey = 8
    }

    /// <summary>
    /// Win32 API Native and .NET API utilities
    /// </summary>
    public static class AppUtils
    {
        // 以下非托管函数，定义根据名称查询msdn
        public const int WM_ACTIVATE = 0x006;
        public const int WM_ACTIVATEAPP = 0x01C;
        public const int WM_NCACTIVATE = 0x086;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public const int AW_HOR_POSITIVE = 0x00000001;
        public const int AW_HOR_NEGATIVE = 0x00000002;
        public const int AW_VER_POSITIVE = 0x00000004;
        public const int AW_VER_NEGATIVE = 0x00000008;
        public const int AW_CENTER = 0x00000010;
        public const int AW_HIDE = 0x00010000;
        public const int AW_ACTIVATE = 0x00020000;
        public const int AW_SLIDE = 0x00040000;
        public const int AW_BLEND = 0x00080000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CreateIconIndirect")]
        public static extern IntPtr CreateIconIndirect(ref ICONINFO piconinfo);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        public static extern bool DestroyIcon(IntPtr piconinfo);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int SendMessage(IntPtr handle, int msg, int wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public extern static int PostMessage(IntPtr handle, int msg, int wParam, IntPtr lParam);

        [DllImport("user32")]
        public extern static void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32")]
        public extern static bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32")]
        public extern static bool SetActiveWindow(IntPtr handle);

        [DllImport("user32")]
        public extern static bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32")]
        public extern static bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32")]
        public extern static long GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32")]
        public extern static bool AttachThreadInput(long idAttach, long idAttachTo, bool fAttach);

        [DllImport("user32")]
        public extern static IntPtr GetForegroundWindow();

        /// <summary>
        ///  如果函数执行成功，返回值不为0。
        ///  如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID(不能与其它ID重复)</param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="key">定义热键的内容</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            KeyModifiers fsModifiers,
            Keys key
            );

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                 //要取消热键的窗口的句柄
            int id                       //要取消热键的ID
            );

        /// <summary>
        /// 设置光标的中心点（即热点，如十字光标）
        /// </summary>
        /// <param name="pCursor">光标</param>
        /// <param name="hotSpot">光标热点</param>
        /// <returns>新光标</returns>
        public static System.Windows.Forms.Cursor SetCursorHotSpot(System.Windows.Forms.Cursor pCursor, System.Drawing.Point hotSpot)
        {
            IntPtr hicon = CopyIcon(pCursor.Handle);
            ICONINFO icInfo;
            System.Windows.Forms.Cursor pCur = null;
            if (GetIconInfo(hicon, out icInfo))
            {
                icInfo.xHotspot = hotSpot.X;
                icInfo.yHotspot = hotSpot.Y;
                IntPtr hcursor = CreateIconIndirect(ref icInfo);
                pCur = new System.Windows.Forms.Cursor(hcursor);
            }
            return pCur;
        }

        /// <summary>
        /// 根据资源名称或取资源（资源必须为embeded）
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <returns>流</returns>
        public static Stream GetResource(string resourceName)
        {
            Stream stream = null;
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                stream = asm.GetManifestResourceStream("ConfigTool.Resources." + resourceName);
            }
            catch (Exception)
            {
                stream = null;
            }
            return stream;
        }

        /// <summary>
        /// 根据图像的名称或取图像（资源必须为embeded）
        /// </summary>
        /// <param name="resouceName">资源名称</param>
        /// <returns>图像</returns>
        public static Image GetImage(string resouceName)
        {
            Image image = null;
            Stream stream = null;
            try
            {
                stream = GetResource(resouceName);
                image = Image.FromStream(stream);
            }
            catch (Exception)
            {
                image = null;
            }
            return image;
        }

        /// <summary>
        /// 根据资源名称获取图像列表（资源必须为embeded）
        /// </summary>
        /// <param name="resourceName">图像名称</param>
        /// <param name="size">宽*高（如16*16）</param>
        /// <returns>图像列表</returns>
        public static ImageList GetImageList(string resourceName, Size size)
        {
            try
            {
                Stream stream = GetResource(resourceName);
                Bitmap bitmap = new Bitmap(stream);
                Color color = bitmap.GetPixel(0, 0);
                bitmap.MakeTransparent(color);
                ImageList imagelist = new ImageList();
                imagelist.Images.AddStrip(bitmap);
                return imagelist;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 根据光标名称获取光标（资源必须为embeded）
        /// </summary>
        /// <param name="resourceName"> 根据光标</param>
        /// <returns>光标</returns>
        public static System.Windows.Forms.Cursor GetCursor(string resourceName)
        {
            Stream stream = null;
            System.Windows.Forms.Cursor pCursor = null;
            try
            {
                stream = GetResource(resourceName);
                pCursor = new System.Windows.Forms.Cursor(stream);
            }
            catch (Exception)
            {
                pCursor = null;
            }
            return pCursor;
        }

        /// <summary>
        /// 释放com组件（接口）
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseComObject(object obj)
        {
            if (obj == null)
                return;

            int refcount = 0;
            do
            {
                refcount = Marshal.ReleaseComObject(obj);
            }
            while (refcount > 0);
        }
    }
}
