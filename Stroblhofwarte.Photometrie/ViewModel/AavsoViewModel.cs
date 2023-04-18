using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.Dialogs;
using Stroblhofwarte.VSPAPI;
using Stroblhofwarte.VSPAPI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class AavsoViewModel : DockWindowViewModel
    {
        #region Properties
        private VariableStar _varStar;

        private ObservableCollection<ReferenceStar> _refStars;

        public ObservableCollection<ReferenceStar> RefStars
        {
            get
            {
                return _refStars;
            }
            set
            {
                _refStars = value;
                OnPropertyChanged("RefStars");
            }
        }

        private ObservableCollection<Filters> _filters;
        public ObservableCollection<Filters> Filters
        {
            get
            {
                _filters = new ObservableCollection<Filters>();
                foreach (FilterObject f in Stroblhofwarte.AperturePhotometry.Filter.Instance.Filters)
                {
                    _filters.Add(new Filters() { AAVSOName = f.AAVSOName, AAVSOFilter = f.AAVSOShort, Filter = f.MyFilter });
                }
                return _filters;
            }
        }

        private string _filter;
        public string Filter
        {
            set { 
                _filter = value;
                OnPropertyChanged("Filter");
                OnPropertyChanged("FilterTransition");
            }
            get
            {
                return _filter;
            }
        }

        private Filters _selectedItem;
        public Filters AAVSOFilterChange
        {
            get { 
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                Filter = value.AAVSOFilter;
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    OnPropertyChanged("AAVSOFilterChange");
                }));
                if(_varStar != null)
                    UpdateData();
            }
        }

        public string FilterTransition
        {
            get { return _filter + " -> " + Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToMyFilter(_filter); }
        }

        private Visibility _waitAnimation = Visibility.Hidden;
        public Visibility WaitAnimation
        {
            get { return _waitAnimation; }
            set
            {
                _waitAnimation = value;
                OnPropertyChanged("WaitAnimation");
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _requestName = string.Empty;
        public string RequestName
        {
            get { return _requestName; }
            set
            {
                _requestName = value;
                OnPropertyChanged("RequestName");
            }
        }

        private string _auid = string.Empty;
        public string Auid
        {
            get { return _auid; }
            set
            {
                _auid = value;
                OnPropertyChanged("Auid");
            }
        }

        private Coordinates _coor;
        public Coordinates Coor
        {
            get { return _coor; }
            set
            {
                _coor = value;
                OnPropertyChanged("Coor");
            }
        }

        #endregion
        #region Commands

        private RelayCommand searchNamedCommand;
        public ICommand SearchNamedCommand
        {
            get
            {
                if (searchNamedCommand == null)
                {
                    searchNamedCommand = new RelayCommand(param => this.SearchNamed(), param => this.CanSearchNamed());
                }
                return searchNamedCommand;
            }
        }

        private bool CanSearchNamed()
        {
            return true;
        }

        private async void SearchNamed()
        {
            
            StroblhofwarteImage.Instance.ClearAnnotation();
            await Task.Factory.StartNew(() =>
            {

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    WaitAnimation = Visibility.Visible;
                }));

                StroblhofwarteVSPAPI aavso = new StroblhofwarteVSPAPI();
                Task<VariableStar> result = aavso.GetAAVSOData(RequestName, 
                    Properties.Settings.Default.AAVSOFov.ToString(CultureInfo.InvariantCulture),
                    Properties.Settings.Default.AAVSOLimitMag.ToString(CultureInfo.InvariantCulture));

                _varStar = result.Result;
                UpdateData();


                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    WaitAnimation = Visibility.Hidden;
                }));

            });
            
        }

        private RelayCommand searchPlateCenterCommand;
        public ICommand SearchPlateCenterCommand
        {
            get
            {
                if (searchPlateCenterCommand == null)
                {
                    searchPlateCenterCommand = new RelayCommand(param => this.SearchPlateCenter(), param => this.CanSearchPlateCenter());
                }
                return searchPlateCenterCommand;
            }
        }

        private bool CanSearchPlateCenter()
        {
            if (!StroblhofwarteImage.Instance.IsValid) return false;
            if (StroblhofwarteImage.Instance.WCS != null) return true;
            return false;
        }

        private async void SearchPlateCenter()
        {
           
            StroblhofwarteImage.Instance.ClearAnnotation();
            await Task.Factory.StartNew(() =>
            {

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    WaitAnimation = Visibility.Visible;
                }));

                StroblhofwarteVSPAPI aavso = new StroblhofwarteVSPAPI();
               
                Coordinates coor = StroblhofwarteImage.Instance.WCS.GetCoordinates(StroblhofwarteImage.Instance.Width / 2, StroblhofwarteImage.Instance.Height / 2);
                Task<VariableStar> result = aavso.GetAAVSOData(coor.RADegrees, coor.Dec,
                    Properties.Settings.Default.AAVSOFov.ToString(CultureInfo.InvariantCulture),
                    Properties.Settings.Default.AAVSOLimitMag.ToString(CultureInfo.InvariantCulture));
                _varStar = result.Result;
                UpdateData();


                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    WaitAnimation = Visibility.Hidden;
                }));

            });

        }
        #endregion

        #region Ctor

        public AavsoViewModel()
        {
            _refStars = new ObservableCollection<ReferenceStar>();
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            RequestName = StroblhofwarteImage.Instance.GetObject();
            Filter = Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToAAVSOFilter(StroblhofwarteImage.Instance.GetFilter());
            foreach(Filters f in Filters)
            {
                if(Filter == f.AAVSOFilter)
                {
                    AAVSOFilterChange = f;
                    break;
                }
            }
        }

        #endregion

        private void UpdateData()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                _refStars.Clear();
            }));
            if (_varStar == null) return;
            Name = _varStar.Var.Name;
            Auid = _varStar.Var.Auid;
            Coor = _varStar.Var.StarCoordinates;
            StroblhofwarteImage.Instance.AddAnnotation(Name, Coor);

            foreach (Star s in _varStar.ReferenceStars)
            {
                ReferenceStar reference = new ReferenceStar();
                reference.Name = s.Name;
                reference.AUID = s.Auid;
                reference.Coor = s.StarCoordinates;
                foreach (Mag m in s.Magnitudes)
                {
                    if (m.Band == Filter)
                    {
                        reference.MAG = m.Magnitude;
                    }
                }
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    _refStars.Add(reference);
                    StroblhofwarteImage.Instance.AddAnnotation(reference.Name, reference.Coor);
                }));

            }
        }
    }

    public class ReferenceStar : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        private Coordinates _coor;
        public Coordinates Coor
        {
            get { return _coor; }
            set { _coor = value; OnPropertyChanged("Coor"); }
        }

        private string _auid;
        public string AUID
        {
            get { return _auid; }
            set { _auid = value; OnPropertyChanged("AUID"); }
        }

        private double _mag;
        public double MAG
        {
            get { return _mag; }
            set { _mag = value; OnPropertyChanged("MAG"); }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; OnPropertyChanged("Comment"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
