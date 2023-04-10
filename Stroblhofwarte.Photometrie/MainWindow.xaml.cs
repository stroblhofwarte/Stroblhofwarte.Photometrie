using JPFITS;
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

namespace Stroblhofwarte.Photometrie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private uint[,] TBTC_IMAGE;
        public MainWindow()
        {
            InitializeComponent();

            int[,] arr;
            FITSImage img = new FITSImage("C:\\Users\\othmar\\Nextcloud\\Transfer\\M13_22.4.2021_RGB_10s_C9.25_f6.3_ASI16000.fit", TBTC_IMAGE, true, true);
        }
    }
}
