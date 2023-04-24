using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ImageInfoViewModel : DockWindowViewModel
    {
        #region Properties

        private ObservableCollection<ImageInfoEntry> _info;

        public ObservableCollection<ImageInfoEntry> Info
        {
            get
            {
                return _info;
            }
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        #endregion

        #region CTor

        public ImageInfoViewModel()
        {
            Info = new ObservableCollection<ImageInfoEntry>();
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
            ContentId = "ImageInfoViewModel";
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            Info.Clear();
            ImageInfoEntry entry = new ImageInfoEntry()
            {
                Object = StroblhofwarteImage.Instance.GetObject(),
                Exposure = StroblhofwarteImage.Instance.GetExposureTime().ToString(CultureInfo.InvariantCulture),
                Instrument = StroblhofwarteImage.Instance.GetInstrument(),
                Filter = StroblhofwarteImage.Instance.GetFilter(),
                ObservationDate = StroblhofwarteImage.Instance.GetObservationTimeUTC(),
                Telescope = StroblhofwarteImage.Instance.GetTelescope(),
                FocalLength = StroblhofwarteImage.Instance.GetFocalLength(),
                FocalRatio = StroblhofwarteImage.Instance.GetFocalRatio(),
                JD = StroblhofwarteImage.Instance.GetJD()
            };
            Info.Add(entry);
        }

        #endregion
    }

    public class ImageInfoEntry : INotifyPropertyChanged
    {
        #region Properties

        private string _object;

        public string Object
        {
            get { return _object; }
            set
            {
                _object = value;
                OnPropertyChanged("Object");
            }
        }

        public string _instrument;
        public string Instrument
        {
            get => _instrument;
            set
            {
                _instrument = value;
                OnPropertyChanged("Instrument");
            }
        }


        public string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged("Filter");
            }
        }

        public string _exposure;
        public string Exposure
        {
            get => _exposure + "s";
            set
            {
                _exposure = value;
                OnPropertyChanged("Exposure");
            }
        }

        public DateTime _observationDate;
        public DateTime ObservationDate
        {
            get => _observationDate;
            set
            {
                _observationDate = value;
                OnPropertyChanged("ObservationDate");
            }
        }

        public double _jd;
        public double JD
        {
            get => _jd;
            set
            {
                _jd = value;
                OnPropertyChanged("JD");
            }
        }

        public string _telescope;
        public string Telescope
        {
            get => _telescope;
            set
            {
                _telescope = value;
                OnPropertyChanged("Telescope");
            }
        }

        public string OpticalData
        {
            get => FocalLength.ToString("0.#", CultureInfo.InvariantCulture) + "mm / f" + FocalRatio.ToString("0.#", CultureInfo.InvariantCulture);
        }

        public double _focalLength;
        public double FocalLength
        {
            get => _focalLength;
            set
            {
                _focalLength = value;
                OnPropertyChanged("FocalLength");
                OnPropertyChanged("OpticalData");
            }
        }

        public double _focalRatio;

        public double FocalRatio
        {
            get => _focalRatio;
            set
            {
                _focalRatio = value;
                OnPropertyChanged("FocalRatio");
                OnPropertyChanged("OpticalData");
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
