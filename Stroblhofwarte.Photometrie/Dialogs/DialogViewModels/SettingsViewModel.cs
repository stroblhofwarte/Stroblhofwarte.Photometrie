using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Stroblhofwarte.Photometrie.Dialogs.DialogViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties

        private string _astrometrynetHost;
        public string AstrometrynetHost
        {
            get { return _astrometrynetHost; }
            set { _astrometrynetHost = value;
                Properties.Settings.Default.AstrometrynetHost = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged("AstrometrynetHost"); }
        }

        private string _astrometrynetKey;
        public string AstrometrynetKey
        {
            get { return _astrometrynetKey; }
            set { _astrometrynetKey = value;
                Properties.Settings.Default.AstrometrynetKey = value;
                Properties.Settings.Default.Save(); 
                OnPropertyChanged("AstrometrynetKey"); }
        }

        #endregion

        #region Ctor
        public SettingsViewModel()
        {
            AstrometrynetHost = Properties.Settings.Default.AstrometrynetHost;
            AstrometrynetKey = Properties.Settings.Default.AstrometrynetKey;
        }

        #endregion

    }
}
