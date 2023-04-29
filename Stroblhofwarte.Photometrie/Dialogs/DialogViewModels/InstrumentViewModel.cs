using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.Dialogs.DialogViewModels
{
    public class InstrumentViewModel : BaseViewModel
    {
        #region Properties

        private ObservableCollection<Instruments> _instruments;

        public ObservableCollection<Instruments> InstrumentsList
        {
            get
            {
                return _instruments;
            }
            set
            {
                _instruments = value;
                OnPropertyChanged("InstrumentsList");
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
            foreach (Instruments f in InstrumentsList)
            {
                try
                {
                    InstrumentObject obj = new InstrumentObject()
                    {
                        Telescope = f.Telescope,
                        Instrument = f.Instrument,
                        Gain = Convert.ToInt32(f.Gain, CultureInfo.InvariantCulture),
                        Offset = Convert.ToInt32(f.Offset, CultureInfo.InvariantCulture),
                        Binning = f.Binning,
                        SetTemp = Convert.ToDouble(f.SetTemp, CultureInfo.InvariantCulture),
                    };
                    string hash = Stroblhofwarte.AperturePhotometry.Instruments.Instance.Hash(obj);
                    Stroblhofwarte.AperturePhotometry.Instruments.Instance.Update(hash, Convert.ToDouble(f.Gain_e_ADU, CultureInfo.InvariantCulture),
                        Convert.ToDouble(f.ReadoutNoise, CultureInfo.InvariantCulture),
                        Convert.ToDouble(f.DarkCurrent, CultureInfo.InvariantCulture));
                    Stroblhofwarte.AperturePhotometry.Instruments.Instance.Update(hash, Convert.ToInt32(f.Aperture, CultureInfo.InvariantCulture),
                        Convert.ToInt32(f.InnerAnnulus, CultureInfo.InvariantCulture),
                        Convert.ToInt32(f.OuterAnnulus, CultureInfo.InvariantCulture));
                }
                catch (Exception ex)
                {
                    // Wrong format given could not be added!
                }

            }
            Stroblhofwarte.AperturePhotometry.Instruments.Instance.Save();

        }

        #endregion


        #region Ctor

        public InstrumentViewModel()
        {
            InstrumentsList = new ObservableCollection<Instruments>();

            try
            {
                // Check first if a new instrument is in the image
                string newTelescope = StroblhofwarteImage.Instance.GetTelescope();
                string newInstrument = StroblhofwarteImage.Instance.GetInstrument();
                double newGain = StroblhofwarteImage.Instance.GetSensorGain();
                double newOffset = StroblhofwarteImage.Instance.GetSensorOffset();
                string newBinning = StroblhofwarteImage.Instance.GetBinning();
                double newSetTemp = StroblhofwarteImage.Instance.GetSensorSetTemp();
                InstrumentObject newobj = new InstrumentObject()
                {
                    Telescope = newTelescope,
                    Instrument = newInstrument,
                    Gain = newGain,
                    Offset = newOffset,
                    Binning = newBinning,
                    SetTemp = newSetTemp
                };
                string hash = Stroblhofwarte.AperturePhotometry.Instruments.Instance.Hash(newobj);
                if (Stroblhofwarte.AperturePhotometry.Instruments.Instance.Get(hash) == null)
                {
                    Stroblhofwarte.AperturePhotometry.Instruments.Instance.Create(newobj);

                }
            } catch (Exception ex)
            {
                // This will simplynot work if no image is loaded!
            }
            // Load all known instruments:
            List<InstrumentObject> instruments = Stroblhofwarte.AperturePhotometry.Instruments.Instance.InstrumentsList;
            foreach (InstrumentObject f in instruments)
            {
                Instruments obj = new Instruments()
                {
                    Telescope = f.Telescope,
                    Instrument = f.Instrument,
                    Gain = f.Gain.ToString(CultureInfo.InvariantCulture),
                    Offset = f.Offset.ToString(CultureInfo.InvariantCulture),
                    Binning = f.Binning,
                    SetTemp = f.SetTemp.ToString(CultureInfo.InvariantCulture),
                    Gain_e_ADU = f.Gain_e_ADU.ToString(CultureInfo.InvariantCulture),
                    ReadoutNoise = f.ReadOutNoise.ToString(CultureInfo.InvariantCulture),
                    DarkCurrent = f.DarkCurrent.ToString(CultureInfo.InvariantCulture),
                    Aperture = f.Aperture.ToString(CultureInfo.InvariantCulture),
                    InnerAnnulus = f.InnerAnnulus.ToString(CultureInfo.InvariantCulture),
                    OuterAnnulus = f.OuterAnnulus.ToString(CultureInfo.InvariantCulture)
                };
                InstrumentsList.Add(obj);
            }
        }

        #endregion
    }
    public class Instruments : INotifyPropertyChanged
    {
        #region Properties

        private string _telescope;

        public string Telescope
        {
            get { return _telescope; }
            set
            {
                _telescope = value;
                OnPropertyChanged("Telescope");
            }
        }

        private string _instrument;

        public string Instrument
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
                OnPropertyChanged("Instrument");
            }
        }

        private string _gain;

        public string Gain
        {
            get { return _gain; }
            set
            {
                _gain = value;
                OnPropertyChanged("Gain");
            }
        }

        private string _offset;

        public string Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                OnPropertyChanged("Offset");
            }
        }

        private string _binning;

        public string Binning
        {
            get { return _binning; }
            set
            {
                _binning = value;
                OnPropertyChanged("Binning");
            }
        }

        private string _setTemp;

        public string SetTemp
        {
            get { return _setTemp; }
            set
            {
                _setTemp = value;
                OnPropertyChanged("SetTemp");
            }
        }

        private string _gain_e_ADU;

        public string Gain_e_ADU
        {
            get { return _gain_e_ADU; }
            set
            {
                _gain_e_ADU = value;
                OnPropertyChanged("Gain_e_ADU");
            }
        }

        private string _readoutNoise;

        public string ReadoutNoise
        {
            get { return _readoutNoise; }
            set
            {
                _readoutNoise = value;
                OnPropertyChanged("ReadoutNoise");
            }
        }

        private string _darkCurrent;

        public string DarkCurrent
        {
            get { return _darkCurrent; }
            set
            {
                _darkCurrent = value;
                OnPropertyChanged("DarkCurrent");
            }
        }

        private string _aperture;

        public string Aperture
        {
            get { return _aperture; }
            set
            {
                _aperture = value;
                OnPropertyChanged("Aperture");
            }
        }

        private string _innerAnnulus;

        public string InnerAnnulus
        {
            get { return _innerAnnulus; }
            set
            {
                _innerAnnulus = value;
                OnPropertyChanged("InnerAnnulus");
            }
        }

        private string _outerAnnulus;

        public string OuterAnnulus
        {
            get { return _outerAnnulus; }
            set
            {
                _outerAnnulus = value;
                OnPropertyChanged("OuterAnnulus");
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            Instruments f = obj as Instruments;
            if (f == null) return false;
            return f.Instrument == Instrument && f.Gain == Gain && f.Offset == Offset && f.SetTemp == SetTemp;
        }

        public override int GetHashCode()
        {
            return Instrument.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
