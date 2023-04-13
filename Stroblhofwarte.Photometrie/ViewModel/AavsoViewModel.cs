using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using Stroblhofwarte.VSPAPI;
using Stroblhofwarte.VSPAPI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class AavsoViewModel : DockWindowViewModel
    {
        #region Properties
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
            _refStars.Clear();
            StroblhofwarteImage.Instance.ClearAnnotation();
            await Task.Factory.StartNew(() =>
            {

                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    WaitAnimation = Visibility.Visible;
                }));

                StroblhofwarteVSPAPI aavso = new StroblhofwarteVSPAPI();
                Task<VariableStar> result = aavso.GetAAVSOData("TV Boo", "60.0", "14.5");

                VariableStar star = result.Result;
                Name = star.Var.Name;
                Auid = star.Var.Auid;
                Coor = star.Var.StarCoordinates;
                StroblhofwarteImage.Instance.AddAnnotation(Name, Coor);

                foreach (Star s in star.ReferenceStars)
                {
                    ReferenceStar reference = new ReferenceStar();
                    reference.Name = s.Name;
                    reference.AUID = s.Auid;
                    reference.Coor = s.StarCoordinates;
                    foreach(Mag m in s.Magnitudes)
                    {
                        if(m.Band == "V")
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
        }

        #endregion
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
