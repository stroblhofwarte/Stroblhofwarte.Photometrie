using Stroblhofwarte.Photometrie.Dialogs.DialogViewModels;
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
using System.Windows.Shapes;

namespace Stroblhofwarte.Photometrie.Dialogs
{
    /// <summary>
    /// Interaktionslogik für DialogTransformationView.xaml
    /// </summary>
    public partial class DialogTransformationView : Window
    {
        public DialogTransformationView()
        {
            InitializeComponent();
            this.DataContext = new DialogTransformationViewModel();
        }
    }
}
