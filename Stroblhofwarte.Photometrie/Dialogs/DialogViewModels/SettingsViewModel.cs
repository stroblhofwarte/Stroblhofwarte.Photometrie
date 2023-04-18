using Stroblhofwarte.Config;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.Dialogs.DialogViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Properties
        public string AstrometrynetHost
        {
            get { return GlobalConfig.Instance.AstrometrynetHost; }
            set {
                GlobalConfig.Instance.AstrometrynetHost = value;
                OnPropertyChanged("AstrometrynetHost"); }
        }
        public string AstrometrynetKey
        {
            get { return GlobalConfig.Instance.AstrometrynetKey; }
            set {
                GlobalConfig.Instance.AstrometrynetKey = value;
                OnPropertyChanged("AstrometrynetKey"); }
        }

        public string AAVSOFov
        {
            get { return GlobalConfig.Instance.AAVSOFov.ToString(); }
            set
            {
                value.Replace(",", ".");
                try
                {
                    GlobalConfig.Instance.AAVSOFov = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                } catch(Exception ex)
                {
                    GlobalConfig.Instance.AAVSOFov = 60.0;
                }
                OnPropertyChanged("AAVSOFov");
            }
        }

        public string AAVSOLimitMag
        {
            get { return GlobalConfig.Instance.AAVSOLimitMag.ToString(); }
            set
            {
                value.Replace(",", ".");
                try
                {
                    GlobalConfig.Instance.AAVSOLimitMag = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.AAVSOLimitMag = 14.5;
                }
                OnPropertyChanged("AAVSOLimitMag");
            }
        }
        public string MagnificationN
        {
            get { return GlobalConfig.Instance.MagnificationN.ToString(); }
            set
            {
                try
                {
                    GlobalConfig.Instance.MagnificationN = Convert.ToInt32(value);
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.MagnificationN = 300;
                }
                OnPropertyChanged("MagnificationN");
            }
        }

        public string FilterDatabasePath
        {
            get { return GlobalConfig.Instance.FilterDatabasePath; }
            set
            {
                try
                {
                    GlobalConfig.Instance.FilterDatabasePath = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.FilterDatabasePath = "";
                }
                OnPropertyChanged("FilterDatabasePath");
            }
        }

        #endregion

        #region Commands

        private RelayCommand _browseFilterPathCommand;
        public ICommand BrowseFilterPathCommand
        {
            get
            {
                if (_browseFilterPathCommand == null)
                {
                    _browseFilterPathCommand = new RelayCommand(param => this.BrowseFilterPath(), param => this.CanBrowseFilterPath());
                }
                return _browseFilterPathCommand;
            }
        }

        private bool CanBrowseFilterPath()
        {
            return true;
        }

        private void BrowseFilterPath()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            dlg.ValidateNames = false;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            // Always default to Folder Selection.
            dlg.FileName = "Folder Selection.";
            if (dlg.ShowDialog() == true)
            {
                string folderPath = Path.GetDirectoryName(dlg.FileName);
                FilterDatabasePath = folderPath;
            }
        }
        #endregion

        #region Ctor
        public SettingsViewModel()
        {
            AstrometrynetHost = GlobalConfig.Instance.AstrometrynetHost;
            AstrometrynetKey = GlobalConfig.Instance.AstrometrynetKey;
            AAVSOFov = GlobalConfig.Instance.AAVSOFov.ToString(CultureInfo.InvariantCulture);
            AAVSOLimitMag = GlobalConfig.Instance.AAVSOLimitMag.ToString(CultureInfo.InvariantCulture);
            MagnificationN = GlobalConfig.Instance.MagnificationN.ToString();
            FilterDatabasePath = GlobalConfig.Instance.FilterDatabasePath;
        }

        #endregion

    }
}
