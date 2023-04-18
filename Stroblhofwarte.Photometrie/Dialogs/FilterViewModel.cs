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
using Stroblhofwarte.AperturePhotometry;
using Microsoft.VisualBasic;

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
            foreach(Filters f in Filter)
            {
                Stroblhofwarte.AperturePhotometry.Filter.Instance.UpdateFilter(f.AAVSOFilter, f.Filter);
            }
            Stroblhofwarte.AperturePhotometry.Filter.Instance.Save();
        }

        #endregion

        #region Ctor

        public FilterViewModel()
        {
            Filter = new ObservableCollection<Filters>();

            List<FilterObject> filters = Stroblhofwarte.AperturePhotometry.Filter.Instance.Filters;
            foreach(FilterObject f in filters)
            {
                Filter.Add(new Filters() { AAVSOName = f.AAVSOName, AAVSOFilter = f.AAVSOShort, Filter = f.MyFilter });
            }
        }

        #endregion

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

        public override bool Equals(object obj)
        {
            Filters f = obj as Filters;
            if (f == null) return false;
            return f.AAVSOFilter == AAVSOFilter;
        }

        public override int GetHashCode()
        {
            return AAVSOFilter.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
