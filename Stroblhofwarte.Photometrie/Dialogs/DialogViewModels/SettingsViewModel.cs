using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private string _aavsoFov;
        public string AAVSOFov
        {
            get { return _aavsoFov; }
            set
            {
                _aavsoFov = value;
                _aavsoFov.Replace(",", ".");
                try
                {
                    Properties.Settings.Default.AAVSOFov = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                } catch(Exception ex)
                {
                    Properties.Settings.Default.AAVSOFov = 60.0;
                }
                Properties.Settings.Default.Save();
                OnPropertyChanged("AAVSOFov");
            }
        }

        private string _aavsoLimitMag;
        public string AAVSOLimitMag
        {
            get { return _aavsoLimitMag; }
            set
            {
                _aavsoLimitMag = value;
                _aavsoLimitMag.Replace(",", ".");
                try
                {
                    Properties.Settings.Default.AAVSOLimitMag = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Properties.Settings.Default.AAVSOLimitMag = 14.5;
                }
                Properties.Settings.Default.Save();
                OnPropertyChanged("AAVSOLimitMag");
            }
        }

        private string _magnificationN;
        public string MagnificationN
        {
            get { return _magnificationN; }
            set
            {
                _magnificationN = value;
               
                try
                {
                    Properties.Settings.Default.MagnificationN = Convert.ToInt32(value);
                }
                catch (Exception ex)
                {
                    Properties.Settings.Default.MagnificationN = 300;
                }
                Properties.Settings.Default.Save();
                OnPropertyChanged("MagnificationN");
            }
        }

        #endregion

        #region Ctor
        public SettingsViewModel()
        {
            AstrometrynetHost = Properties.Settings.Default.AstrometrynetHost;
            AstrometrynetKey = Properties.Settings.Default.AstrometrynetKey;
            AAVSOFov = Properties.Settings.Default.AAVSOFov.ToString(CultureInfo.InvariantCulture);
            AAVSOLimitMag = Properties.Settings.Default.AAVSOLimitMag.ToString(CultureInfo.InvariantCulture);
            MagnificationN = Properties.Settings.Default.MagnificationN.ToString();
        }

        #endregion

    }
}
