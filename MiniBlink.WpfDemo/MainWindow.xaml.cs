using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoDllProxy;
using MiniBlink.Share;

namespace MiniBlink.WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }
        
        // private void TitleChanged(IntPtr webview, IntPtr param, IntPtr title)
        // {
        //     
        //         var t= title.WKEToUTF8String();
        //     
        // }
    }

    // public static class Exts
    // {
    //     public static string WKEToUTF8String(this IntPtr ptr)
    //     {
    //         return MBApi2.wkeGetString(ptr);
    //     }
    //     public static string ToUTF8String(this IntPtr ptr)
    //     {
    //         var data = new List<byte>();
    //         var off = 0;
    //         while (true)
    //         {
    //             var ch = Marshal.ReadByte(ptr, off++);
    //             if (ch == 0)
    //             {
    //                 break;
    //             }
    //             data.Add(ch);
    //         }
    //         return Encoding.UTF8.GetString(data.ToArray());
    //     }
    //
    // }

}