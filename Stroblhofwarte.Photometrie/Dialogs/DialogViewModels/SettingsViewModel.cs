#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

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

        public bool ScratchPadActive
        {
            get { return GlobalConfig.Instance.ScratchPadActive; }
            set
            {
                GlobalConfig.Instance.ScratchPadActive = value;
                OnPropertyChanged("ScratchPadActive");
            }
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

        public string StandardFieldsPath
        {
            get { return GlobalConfig.Instance.StandardFieldsPath; }
            set
            {
                try
                {
                    GlobalConfig.Instance.StandardFieldsPath = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.StandardFieldsPath = "";
                }
                OnPropertyChanged("StandardFieldsPath");
            }
        }

        public string OBSCODE
        {
            get { return GlobalConfig.Instance.OBSCODE; }
            set
            {
                try
                {
                    GlobalConfig.Instance.OBSCODE = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.OBSCODE = "";
                }
                OnPropertyChanged("OBSCODE");
            }
        }

        public string OBSTYPE
        {
            get { return GlobalConfig.Instance.OBSTYPE; }
            set
            {
                try
                {
                    GlobalConfig.Instance.OBSTYPE = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.OBSTYPE = "";
                }
                OnPropertyChanged("OBSTYPE");
            }
        }

        public string AstapExe
        {
            get { return GlobalConfig.Instance.AstapExe; }
            set
            {
                try
                {
                    GlobalConfig.Instance.AstapExe = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.AstapExe = "";
                }
                OnPropertyChanged("AstapExe");
            }
        }

        public string AstapArgs
        {
            get { return GlobalConfig.Instance.AstapArgs; }
            set
            {
                try
                {
                    GlobalConfig.Instance.AstapArgs = value;
                }
                catch (Exception ex)
                {
                    GlobalConfig.Instance.AstapArgs = "";
                }
                OnPropertyChanged("AstapArgs");
            }
        }

        public bool Astap
        {
            get { return GlobalConfig.Instance.Astap; }
            set
            {
                GlobalConfig.Instance.Astap = value;
                GlobalConfig.Instance.Astrometry = false;
                OnPropertyChanged("Astap");
                OnPropertyChanged("Astrometry");
            }
        }

        public bool Astrometry
        {
            get { return GlobalConfig.Instance.Astrometry; }
            set
            {
                GlobalConfig.Instance.Astrometry = value;
                GlobalConfig.Instance.Astap = false;
                OnPropertyChanged("Astap");
                OnPropertyChanged("Astrometry");
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

        private RelayCommand _browseStandardFieldsPathCommand;
        public ICommand BrowseStandardFieldsPathCommand
        {
            get
            {
                if (_browseStandardFieldsPathCommand == null)
                {
                    _browseStandardFieldsPathCommand = new RelayCommand(param => this.BrowseStandardFieldsPath(), param => this.CanBrowseStandardFieldsPath());
                }
                return _browseStandardFieldsPathCommand;
            }
        }

        private bool CanBrowseStandardFieldsPath()
        {
            return true;
        }

        private void BrowseStandardFieldsPath()
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
                StandardFieldsPath = folderPath;
            }
        }

        private RelayCommand _browseAstapExe;
        public ICommand BrowseAstapExe
        {
            get
            {
                if (_browseAstapExe == null)
                {
                    _browseAstapExe = new RelayCommand(param => this.BrowseAstap(), param => this.CanStapExe());
                }
                return _browseAstapExe;
            }
        }

        private bool CanStapExe()
        {
            return true;
        }

        private void BrowseAstap()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            dlg.ValidateNames = false;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            // Always default to Folder Selection.
            dlg.FileName = "astap.exe";
            if (dlg.ShowDialog() == true)
            {
                string folderPath = dlg.FileName;
                AstapExe = folderPath;
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
            StandardFieldsPath = GlobalConfig.Instance.StandardFieldsPath;
        }

        #endregion

    }
}
