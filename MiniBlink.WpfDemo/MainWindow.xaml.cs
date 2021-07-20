using System;
using System.Collections.Generic;
using System.Linq;
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
using Miniblink;

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
             var api =  DllModuleBuilder.Create().Build<IMiniBlink>();
              api.Initialize();
           var r=   api.IsInitialize();
         var v= api.GetVersion();
          //    MBApi2.wkeInitialize();
          // var r2 = MBApi2.wkeIsInitialize();
        }
    }
}