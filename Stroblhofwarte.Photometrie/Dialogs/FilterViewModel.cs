using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.ViewModel;
using Stroblhofwarte.VSPAPI.Data;
using Stroblhofwarte.VSPAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Xml.Linq;

namespace Stroblhofwarte.Photometrie.Dialogs
{
    public class FilterViewModel : BaseViewModel
    {
        #region Properties

        private ObservableCollection<Filters> _filter;

        public ObservableCollection<Filters> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                OnPropertyChanged("_filter");
            }
        }

        #endregion

        #region Commands
        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                {
                    openCommand = new RelayCommand(param => this.Open(), param => this.CanOpen());
                }
                return openCommand;
            }
        }

        private bool CanOpen()
        {
            return true;
        }

        private void Open()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            dlg.ValidateNames = true;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            // Always default to Folder Selection.
            dlg.FileName = "Filter database Selection.";
            if (dlg.ShowDialog() == true)
            {
                if(!Load(dlg.FileName))
                {
                    Save(dlg.FileName);
                }
                Properties.Settings.Default.FilterFile = dlg.FileName;
                Properties.Settings.Default.Save();
            }
        }


        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(param => this.Save(), param => this.CanSave());
                }
                return saveCommand;
            }
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            Save(Properties.Settings.Default.FilterFile);
        }

        #endregion

        #region Ctor

        public FilterViewModel()
        {
            Filter = new ObservableCollection<Filters>();

            string filterfilename = Properties.Settings.Default.FilterFile;
            if (!Load(filterfilename))
            {
                Filter.Add(new Filters() { AAVSOName = "Johnson U", AAVSOFilter = "U", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Johnson B", AAVSOFilter = "B", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Johnson V", AAVSOFilter = "V", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Johnson R", AAVSOFilter = "RJ", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Johnson I", AAVSOFilter = "IJ", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Cousins R", AAVSOFilter = "R", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Cousins I", AAVSOFilter = "I", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Near-Infrared J", AAVSOFilter = "J", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Near-Infrared H", AAVSOFilter = "H", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Near-Infrared K", AAVSOFilter = "K", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Sloan u", AAVSOFilter = "SU", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Sloan g", AAVSOFilter = "SG", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Sloan r", AAVSOFilter = "SR", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Sloan i", AAVSOFilter = "SI", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Sloan z", AAVSOFilter = "SZ", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren u", AAVSOFilter = "STU", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren v", AAVSOFilter = "STV", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren b", AAVSOFilter = "STB", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren y", AAVSOFilter = "STY", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren Hbw", AAVSOFilter = "STHBW", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Stromgren Hbn", AAVSOFilter = "STHBN", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Optec Wing A", AAVSOFilter = "MA", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Optec Wing B", AAVSOFilter = "MB", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Optec Wing C (or I)", AAVSOFilter = "MI", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "PanSTARSS z-short", AAVSOFilter = "ZS", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "PanSTARRS Y", AAVSOFilter = "Y", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "DSLR Green", AAVSOFilter = "TG", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "DSLR Blue", AAVSOFilter = "TB", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "DSLR Red", AAVSOFilter = "TR", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Clear reduced to V", AAVSOFilter = "CV", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Clear reduced to R", AAVSOFilter = "CR", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Clear with blue-blocking", AAVSOFilter = "CBB", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Halpha", AAVSOFilter = "HA", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Halpha Continuum", AAVSOFilter = "HAC", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Blue visual filter", AAVSOFilter = "Blue-vis.", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Green visual filter", AAVSOFilter = "Green-vis.", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Red visual filter", AAVSOFilter = "Red-vis.", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Orange filter", AAVSOFilter = "Orange", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Yellow visual filter", AAVSOFilter = "Yellow-vis.", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Visual", AAVSOFilter = "Vis.", Filter = "" });
                Filter.Add(new Filters() { AAVSOName = "Other", AAVSOFilter = "O", Filter = "" });
            }
        }

        #endregion

        public bool Load(string csvPath)
        {
            try
            {

                Filter = new ObservableCollection<Filters>();
                string[] lines = File.ReadAllLines(csvPath);

                foreach (string line in lines)
                {
                    if (line.StartsWith("#")) continue;
                    string[] fields = line.Split(";");
                    if (fields.Length != 3) continue;
                    Filter.Add(new Filters() { AAVSOName = fields[0], AAVSOFilter = fields[1], Filter = fields[2] });
                }
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }

        public bool Save(string csvPath)
        {
            try
            {
                using (StreamWriter writetext = new StreamWriter(csvPath))
                {
                    foreach(Filters filter in Filter)
                    {
                        string line = filter.AAVSOName + ";" + filter.AAVSOFilter + ";" + filter.Filter;
                        writetext.WriteLine(line);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

    public class Filters: INotifyPropertyChanged
    {
        #region Properties

        private string _aavsoName;

        public string AAVSOName
        {
            get { return _aavsoName; }
            set
            {
                _aavsoName = value;
                OnPropertyChanged("AAVSOName");
            }
        }

        private string _aavsoFilter;

        public string AAVSOFilter
        {
            get { return _aavsoFilter; }
            set
            {
                _aavsoFilter = value;
                OnPropertyChanged("AAVSOFilter");
            }
        }
        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnPropertyChanged("Filter");
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
