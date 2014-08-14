using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace EPS.Engine.Utils
{
    /// <summary>
    /// arcengine 使用styleserver文件管理符号，以下为功能函数的包装
    /// </summary>
    public static class StyleUtils
    {
        public static string _LINESYMBOLS = "Line Symbols";
        public static string _MARKERSYMBOLS = "Marker Symbols";
        public static string _FILLSYMBOLS = "Fill Symbols";
        public static string _TEXTSYMBOL = "Text Symbols";
        public static string _ALLSYMBOLS = "全部符号";
        //public static string _STYLEFILE = "Style\\ESRI.ServerStyle";//"electric.ServerStyle";
        public static string _STYLEFILE = "Style\\土地利用现状符号填充.ServerStyle";

        /// <summary>
        /// 获取指定种类，指定符号id的符号
        /// </summary>
        /// <param name="sGallery">符号种类（e.g.填充颜色）</param>
        /// <param name="id">符号id</param>
        /// <returns>符号</returns>
        public static ISymbol GetStyleSymbol(string sGallery, int id)
        {
            try
            {
                IStyleGallery pStyleGallery = new ServerStyleGalleryClass();
                IStyleGalleryStorage pStyleGalleryStorage = pStyleGallery as IStyleGalleryStorage;

                for (int i = pStyleGalleryStorage.FileCount - 1; i >= 0; i--)
                    pStyleGalleryStorage.RemoveFile(pStyleGalleryStorage.get_File(i));

                pStyleGalleryStorage.AddFile(_STYLEFILE);
                IEnumStyleGalleryItem pEnumStyleItem = pStyleGallery.get_Items(sGallery, _STYLEFILE, "");
                pEnumStyleItem.Reset();
                IStyleGalleryItem mStyleItem = pEnumStyleItem.Next();
                IStyleGalleryClass mStyleClass = null;
                for (int i = 0; i < pStyleGallery.ClassCount; i++)
                {
                    mStyleClass = pStyleGallery.get_Class(i);
                    string str = mStyleClass.Name;
                    if (str == sGallery)
                        break;
                }

                ISymbol result = null;
                while (mStyleItem != null)
                {
                    if (mStyleItem.ID == id)
                    {
                        result = mStyleItem.Item as ISymbol;
                        break;
                    }
                    mStyleItem = pEnumStyleItem.Next();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumStyleItem);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将符号种类中指定的符号项（mStyleGlyItem）转换为指定大小的位图
        /// </summary>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <param name="mStyleGlyCs"></param>
        /// <param name="mStyleGlyItem"></param>
        /// <returns></returns>
        public static Bitmap StyleGalleryItemToBmp(int iWidth, int iHeight, IStyleGalleryClass mStyleGlyCs, IStyleGalleryItem mStyleGlyItem)
        {
            // 建立符合规格的内存图片
            Bitmap bmp = new Bitmap(iWidth, iHeight);
            Graphics gImage = Graphics.FromImage(bmp);
            // 建立对应的符号显示范围
            tagRECT rect = new tagRECT();
            rect.right = bmp.Width;
            rect.bottom = bmp.Height;
            // 生成预览
            System.IntPtr hdc = new IntPtr();
            hdc = gImage.GetHdc();
            // 在图片上绘制符号
            mStyleGlyCs.Preview(mStyleGlyItem.Item, hdc.ToInt32(), ref rect);
            gImage.ReleaseHdc(hdc);
            gImage.Dispose();
            bmp.MakeTransparent(Color.White);
            return bmp;
        }

        /// <summary>
        /// 读取ServerStyle中的文件并显示
        /// </summary>
        /// <param name="StyleGalleryClass">符号文件的种类</param>
        /// <param name="CategoryClass">符号的种类</param>
        /// <param name="cbxCategory">符号文件的种类的容器</param>
        /// <param name="lvSymbol">符号位图的容器</param>
        public static void ReadServerStyle(string StyleGalleryClass, string CategoryClass, ComboBox cbxCategory, ListView lvSymbol)
        {
            lvSymbol.Items.Clear();
            lvSymbol.LargeImageList.Images.Clear();
            IStyleGallery pStyleGallery = new ServerStyleGalleryClass();
            IStyleGalleryStorage pStyleGalleryStorage = pStyleGallery as IStyleGalleryStorage;
            // 增加符号文件
            //pStyleGalleryStorage.TargetFile = _STYLEFILE;
            for (int i = pStyleGalleryStorage.FileCount - 1; i >= 0; i--)
            {
                pStyleGalleryStorage.RemoveFile(pStyleGalleryStorage.get_File(i));
            }
            pStyleGalleryStorage.AddFile(_STYLEFILE);
            // 根据当前符号的类别和文件得到符号的枚举循环子,符号类别包括Fill Symbol,Line Symbol等
            IEnumStyleGalleryItem pEnumStyleItem = pStyleGallery.get_Items(StyleGalleryClass, _STYLEFILE, "");
            // 得到符号文件类别的各个条目,增加到一个Combox中
            if (cbxCategory != null)
            {
                cbxCategory.Items.Clear();
                cbxCategory.Items.Add(_ALLSYMBOLS);
                IEnumBSTR pEnumBSTR = pStyleGallery.get_Categories(StyleGalleryClass);
                pEnumBSTR.Reset();
                string Category = "";
                Category = pEnumBSTR.Next();
                while (Category != null)
                {
                    cbxCategory.Items.Add(Category);
                    Category = pEnumBSTR.Next();
                }
            }

            pEnumStyleItem.Reset();
            IStyleGalleryItem mStyleItem = pEnumStyleItem.Next();
            IStyleGalleryClass mStyleClass = null;
            for (int i = 0; i < pStyleGallery.ClassCount; i++)
            {
                mStyleClass = pStyleGallery.get_Class(i);
                string str = mStyleClass.Name;
                if (str == StyleGalleryClass)
                    break;
            }

            // 得到各个符号并转化为图片
            int ImageIndex = 0;
            ImageList Largeimage = lvSymbol.LargeImageList;

            if (CategoryClass == "" || CategoryClass == _ALLSYMBOLS)
            {
                while (mStyleItem != null)
                {
                    // 调用另一个类的方法将符号转化为图片
                    Bitmap bmpB = StyleGalleryItemToBmp(32, 32, mStyleClass, mStyleItem);
                    Largeimage.Images.Add(bmpB);
                    ListViewItem lvItem = new ListViewItem(new string[] { mStyleItem.Name, mStyleItem.ID.ToString(), mStyleItem.Category }, ImageIndex);
                    lvSymbol.Items.Add(lvItem);
                    ImageIndex++;
                    mStyleItem = pEnumStyleItem.Next();
                }
            }
            else
            {
                while (mStyleItem != null)
                {
                    if (CategoryClass == mStyleItem.Category)
                    {
                        // 调用另一个类的方法将符号转化为图片
                        Bitmap bmpB = StyleGalleryItemToBmp(32, 32, mStyleClass, mStyleItem);
                        Largeimage.Images.Add(bmpB);
                        ListViewItem lvItem = new ListViewItem(new string[] { mStyleItem.Name, mStyleItem.ID.ToString(), mStyleItem.Category }, ImageIndex);
                        lvSymbol.Items.Add(lvItem);
                        ImageIndex++;
                    }
                    mStyleItem = pEnumStyleItem.Next();
                }
            }
            // 必须采用此方式进行释放，第二次才能正常读取
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumStyleItem);
        }

        /// <summary>
        /// 弃用
        /// </summary>
        public static void TrueType2Style()
        {
            /*
            IStyleGallery pStyleGallery; // Style文件的编辑环境
            IStyleGalleryItem pStyleGalleryItem; // 符号库中的一个符号队形
            
            // Dim pMarkerSymbolStyleGalleryClass As IStyleGalleryClass
            IEnumStyleGalleryItem pItems; // 一组符号           
            IStyleGalleryStorage pStylStor; // 管理编辑环境中的文件对象
            ICharacterMarkerSymbol pCharMarkerSym; // 将要添加到Style文件中的符号
            IFont pFont; // 字体      
            string pFilePath; // 自定义Style文件的路径
            string pTargetFile; // 目标文件
        
            // 将自定义Style文件添加到StyleGallery
            pStylStor = StyleGallery;
            pStyleGallery = pStylStor;

            pFilePath = pStylStor.DefaultStylePath + "CustomStyle.style";
    
            pTargetFile = pStylStor.TargetFile;
            if (pTargetFile == pFilePath)
            {
                // 系统会默认一个路径
            }
            else
            {
                pStylStor.TargetFile = pFilePath;
                pStylStor.AddFile(pFilePath);
            }
       
            // 创建各个符号对象
            pFont = SystemFont;
            pFont.Name = "Cityblueprint";
            pFont.Italic = True;
            long pCount;
    
            IColor pColor;
            pColor = new RgbColor();
            pColor.RGB = RGB(255, 0, 0);
    
            //要加载所有的字体中的符号需要你记下字体中的符号数目

            long i = 0;
            pCharMarkerSym = new CharacterMarkerSymbolClass();    
           
            pCharMarkerSym.Angle = 0;
            pCharMarkerSym.Font = pFont;
            pCharMarkerSym.CharacterIndex = i;
            pCharMarkerSym.Color = pColor;
            pCharMarkerSym.size = 20;
            pCharMarkerSym.XOffset = 0;
            pCharMarkerSym.YOffset = 0;
    
    
            while (pCharMarkerSym != null)
            {
                pStyleGalleryItem = new StyleGalleryItemClass();
                pStyleGalleryItem.Category = "Default";
                pStyleGalleryItem.Name = "Try" + CStr(i);
                pStyleGalleryItem.Item = pCharMarkerSym;

                // 将创建的符号添加到指定的Style文件中去
                pStyleGallery.AddItem(pStyleGalleryItem);
                i++;
                if (i >= 400)
                  break;
 
                pCharMarkerSym.CharacterIndex = i;
            }
  
            // 删除添加的条目
            pItems = pStyleGallery.Items("Marker Symbols", pFilePath, "Default");
    
            pItems.Reset();
    
            IStyleGalleryItem pItem;
            long j = 0;
    
            pItem = pItems.Next();
            while (pItem != null)
            {
                pStyleGallery.RemoveItem(pItem); 
                pItem = pItems.Next();
                j++;
            }
    
            // 清空内存
            pStylStor.RemoveFile(pFilePath);
            pStyleGallery = null;
            pStyleGalleryItem = null;
            pCharMarkerSym = null;
            pItems = null;
            pFont = null;
            pStylStor = null;
            pColor = null;
            pItem = null;
            */
        }
    }
}
