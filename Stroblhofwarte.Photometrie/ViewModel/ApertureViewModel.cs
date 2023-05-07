using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Astrometry.Solver.AstrometryNet;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;
using Stroblhofwarte.Photometrie.DataPackages;
using Stroblhofwarte.Photometrie.FileFormats;
using Stroblhofwarte.AperturePhotometry.StandardFields;
using Stroblhofwarte.Photometrie.View;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Stroblhofwarte.Photometrie.Dialogs;
using Stroblhofwarte.Photometrie.Dialogs.DialogViewModels;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public enum enumPhotoState
    {
        INIT,
        VAR,
        COMP,
        CHECK,
        DONE
    }
    public class ApertureViewModel : DockWindowViewModel
    {
        private string PERMADB = "mymeasurements.db";
        #region Events

        public event EventHandler NewMeasurement;



        #endregion

        #region Properties
        private MeasurementResult _measVar;
        private MeasurementResult _measComp;
        private MeasurementResult _measCheck;
        private bool _apertureChanged = false;
        private ApertureMeasurementEntry _currentMeas;

        private ObservableCollection<ApertureMeasurementEntry> _measures = new ObservableCollection<ApertureMeasurementEntry>();
        public ObservableCollection<ApertureMeasurementEntry> Measures
        {
            get { return _measures; }
        }


        private enumPhotoState _photoState;
        public enumPhotoState PhotoState
        {
            get { return _photoState; }
            set
            {
                _photoState = value;
                if (_photoState == enumPhotoState.VAR)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    VarStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFF7E627");
                    CompStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                    CheckStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                }
                if (_photoState == enumPhotoState.COMP)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    VarStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                    CompStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFF7E627");
                    CheckStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                }
                if (_photoState == enumPhotoState.CHECK)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    VarStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                    CompStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFADADAD");
                    CheckStatColor = (System.Windows.Media.Brush)converter.ConvertFromString("#FFF7E627");
                }
            }
        }

        private int _centroidSearchRadius = 30;

        private System.Drawing.Point _starCentroid = new System.Drawing.Point(0, 0);

        public BitmapImage ImageSource
        {
            get
            {
                if (!StroblhofwarteImage.Instance.IsValid) return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    int magN = Properties.Settings.Default.MagnificationN;
                    ushort[] raw = StroblhofwarteImage.Instance.GetSubimagRaw(_starCentroid, magN, magN);
                    Bitmap bitmap = StroblhofwarteImage.Instance.GetSubimage(_starCentroid, magN, magN);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Green, 1.0f),
                        (int)(magN / 2 - ApertureSize),
                        (int)(magN / 2 - ApertureSize),
                        (int)(2 * ApertureSize),
                        (int)(2 * ApertureSize));
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f),
                        (int)(magN / 2 - AnnulusInnerRadius),
                        (int)(magN / 2 - AnnulusInnerRadius),
                        (int)(2 * AnnulusInnerRadius),
                        (int)(2 * AnnulusInnerRadius));
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f),
                        (int)(magN / 2 - AnnulusOuterRadius),
                        (int)(magN / 2 - AnnulusOuterRadius),
                        (int)(2 * AnnulusOuterRadius),
                        (int)(2 * AnnulusOuterRadius));

                    if (_showProfile)
                    {
                        // Draw star profile x:
                        int max = 0;
                        int min = int.MaxValue;
                        for (int x = magN / 2 - 20; x < magN / 2 + 20; x++)
                        {
                            int y = raw[(magN / 2) * magN + x];
                            if (y > max) max = y;
                            if (y < min) min = y;
                        }
                        double f = (max - min) / ((magN / 2) - 50);
                        double oldY = 0.0;
                        System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f);
                        for (int x = magN / 2 - 20; x < magN / 2 + 20; x++)
                        {
                            double y = (double)raw[(magN / 2) * magN + x];
                            y = (y - min) / f;
                            g.DrawLine(p, x - 1, (magN - 20) - (int)oldY, x, (magN - 20) - (int)y);
                            oldY = y;
                        }
                    }
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    //bitmapimage.DecodePixelWidth = _fitsImage.Width();
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        private string _stepInfo;
        public string StepInfo
        {
            get { return _stepInfo; }
            set
            {
                _stepInfo = value;
                OnPropertyChanged("StepInfo");
            }
        }

        
        private bool _showProfile;
        public bool ShowProfile

        {
            get { return _showProfile; }
            set
            {
                _showProfile = value;
                OnPropertyChanged("ShowProfile");
                OnPropertyChanged("ImageSource");
            }
        }

        private System.Windows.Media.Brush _varStatColor;
        public System.Windows.Media.Brush VarStatColor

        {
            get { return _varStatColor; }
            set
            {
                _varStatColor = value;
                OnPropertyChanged("VarStatColor");
            }
        }

        private System.Windows.Media.Brush _compStatColor;
        public System.Windows.Media.Brush CompStatColor

        {
            get { return _compStatColor; }
            set
            {
                _compStatColor = value;

                OnPropertyChanged("CompStatColor");
            }
        }

        private System.Windows.Media.Brush _checkStatColor;
        public System.Windows.Media.Brush CheckStatColor

        {
            get { return _checkStatColor; }
            set
            {
                _checkStatColor = value;

                OnPropertyChanged("CheckStatColor");
            }
        }


        private double _z;
        public double Z
        {
            get { return _z; }
            set
            {
                _z = value;
                OnPropertyChanged("Z");
            }
        }

        private double _varMag;
        public double VarMag
        {
            get { return _varMag; }
            set
            {
                _varMag = value;
                OnPropertyChanged("VarMag");
                OnPropertyChanged("VarMagStr");
            }
        }

        public string VarMagStr { get { return _varMag.ToString("0.##", CultureInfo.InvariantCulture); } }

        private double _varError;
        public double VarError
        {
            get { return _varError; }
            set
            {
                _varError = value;
                OnPropertyChanged("VarError");
                OnPropertyChanged("VarErrorStr");
            }
        }
        public string VarErrorStr { get { return _varError.ToString("0.#####", CultureInfo.InvariantCulture); } }
        private double _compError;
        public double CompError
        {
            get { return _compError; }
            set
            {
                _compError = value;
                OnPropertyChanged("CompError");
                OnPropertyChanged("CompErrorStr");
            }
        }

        public string CompErrorStr { get { return _compError.ToString("0.#####", CultureInfo.InvariantCulture); } }

        private double _compMag;
        public double CompMag
        {
            get { return _compMag; }
            set
            {
                _compMag = value;
                OnPropertyChanged("CompMag");
                OnPropertyChanged("CompMagStr");
            }
        }

        public string CompMagStr { get { return _compMag.ToString("0.##", CultureInfo.InvariantCulture); } }

        private double _checkMag;
        public double CheckMag
        {
            get { return _checkMag; }
            set
            {
                _checkMag = value;
                OnPropertyChanged("CheckMag");
                OnPropertyChanged("CheckMagStr");
            }
        }

        public string CheckMagStr { get { return _checkMag.ToString("0.##", CultureInfo.InvariantCulture); } }

        private double _referenceMag;
        public double ReferenceMag
        {
            get { return _referenceMag; }
            set
            {
                _referenceMag = value;
                Stroblhofwarte.AperturePhotometry.ApertureMeasure measure = new ApertureMeasure();
                Z = measure.GetZ(_measComp, _referenceMag);
                VarMag = measure.Magnitude(_measVar, Z);
                CheckMag = measure.Magnitude(_measCheck, Z);
                CompMag = measure.Magnitude(_measComp, Z);
                VarError = measure.Uncertenty(_measVar);
                CompError = measure.Uncertenty(_measComp);
                if(_currentMeas != null) _currentMeas.MagErr = VarError;
                if (_currentMeas != null) _currentMeas.CompErr = CompError;
                OnPropertyChanged("ReferenceMag");
            }
        }

        private double _checkReferenceMag;
        public double CheckReferenceMag
        {
            get { return _checkReferenceMag; }
            set
            {
                _checkReferenceMag = value;
                VarError = CheckMag - (double)(CheckReferenceMag);
                OnPropertyChanged("CheckReferenceMag");
            }
        }

        private double _mag;
        public double Mag
        {
            get { return _mag; }
            set
            {
                _mag = value;
                OnPropertyChanged("Mag");
                OnPropertyChanged("MagString");
            }
        }

        public string MagString
        {
            get { return _mag.ToString(CultureInfo.InvariantCulture) + " mag"; }
        }

        private int _apertureSize;
        public int ApertureSize
        {
            get { return _apertureSize; }
            set
            {
                _apertureSize = value;
                _apertureChanged = true;
                if (AnnulusInnerRadius <= value) AnnulusInnerRadius = value + 1;
                UpdateApertureMeasurement();
                PhotoState = enumPhotoState.INIT;
                OnPropertyChanged("ApertureSize");
                OnPropertyChanged("ImageSource");
            }
        }

        private double _annulusInnerRadius;
        public double AnnulusInnerRadius
        {
            get { return _annulusInnerRadius; }
            set
            {
                _annulusInnerRadius = value;
                if (AnnulusOuterRadius <= value) AnnulusOuterRadius = value + 1;
                _apertureChanged = true;
                OnPropertyChanged("AnnulusInnerRadius");
            }
        }

        private double _annulusOuterRadius;
        public double AnnulusOuterRadius
        {
            get { return _annulusOuterRadius; }
            set
            {
                _annulusOuterRadius = value;
                _apertureChanged = true;
                OnPropertyChanged("AnnulusOuterRadius");
            }
        }

        #endregion
        #region commandClear
        private RelayCommand commandClear;
        public ICommand CommandClear
        {
            get
            {
                if (commandClear == null)
                {
                    commandClear = new RelayCommand(param => this.Clear(), param => this.CanClear());
                }
                return commandClear;
            }
        }

        private bool CanClear()
        {
            if (Measures.Count > 0) return true;
            return false;
        }

        private async void Clear()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var result = await metroWindow.ShowMessageAsync("Measurements Clear",
                "All aperture measurements in the list will be deleted:",
                MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Negative) return;
            Measures.Clear();
        }
        #endregion

        #region commandSaveAperture
        private RelayCommand commandSaveAperture;
        public ICommand CommandSaveAperture
        {
            get
            {
                if (commandSaveAperture == null)
                {
                    commandSaveAperture = new RelayCommand(param => this.SaveAperture(), param => this.CanSaveAperture());
                }
                return commandSaveAperture;
            }
        }

        private bool CanSaveAperture()
        {
            return _apertureChanged;
        }

        private void SaveAperture()
        {
            // Store new size into the instrument setup:
            try
            {
                InstrumentObject instr = AperturePhotometry.Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
                (int)StroblhofwarteImage.Instance.GetSensorGain(),
                (int)StroblhofwarteImage.Instance.GetSensorOffset(),
                StroblhofwarteImage.Instance.GetBinning(),
                StroblhofwarteImage.Instance.GetSensorSetTemp());
                instr.Aperture = _apertureSize;
                instr.InnerAnnulus = _annulusInnerRadius;
                instr.OuterAnnulus = _annulusOuterRadius;
                AperturePhotometry.Instruments.Instance.Save();
                _apertureChanged = false;
            }
            catch (Exception ex)
            {
                // This happen when no image is loaded.
            }
        }
        #endregion

        #region commandAperturePlus
        private RelayCommand commandAperturePlus;
        public ICommand CommandAperturePlus
        {
            get
            {
                if (commandAperturePlus == null)
                {
                    commandAperturePlus = new RelayCommand(param => this.AperturePlus(), param => this.CanAperturePlus());
                }
                return commandAperturePlus;
            }
        }

        private bool CanAperturePlus()
        {
            return true;
        }

        private void AperturePlus()
        {
            ApertureSize++;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandApertureMinus
        private RelayCommand commandApertureMinus;
        public ICommand CommandApertureMinus
        {
            get
            {
                if (commandApertureMinus == null)
                {
                    commandApertureMinus = new RelayCommand(param => this.ApertureMinus(), param => this.CanApertureMinus());
                }
                return commandApertureMinus;
            }
        }

        private bool CanApertureMinus()
        {
            return true;
        }

        private void ApertureMinus()
        {
            ApertureSize--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandInnerPlus
        private RelayCommand commandInnerPlus;
        public ICommand CommandInnerPlus
        {
            get
            {
                if (commandInnerPlus == null)
                {
                    commandInnerPlus = new RelayCommand(param => this.InnerPlus(), param => this.CanInnerPlus());
                }
                return commandInnerPlus;
            }
        }

        private bool CanInnerPlus()
        {
            return true;
        }

        private void InnerPlus()
        {
            AnnulusInnerRadius++;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandInnerMinus
        private RelayCommand commandInnerMinus;
        public ICommand CommandInnerMinus
        {
            get
            {
                if (commandInnerMinus == null)
                {
                    commandInnerMinus = new RelayCommand(param => this.InnerMinus(), param => this.CanInnerMinus());
                }
                return commandInnerMinus;
            }
        }

        private bool CanInnerMinus()
        {
            return true;
        }

        private void InnerMinus()
        {
            AnnulusInnerRadius--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandOuterPlus
        private RelayCommand commandOuterPlus;
        public ICommand CommandOuterPlus
        {
            get
            {
                if (commandOuterPlus == null)
                {
                    commandOuterPlus = new RelayCommand(param => this.OuterPlus(), param => this.CanOuterPlus());
                }
                return commandOuterPlus;
            }
        }

        private bool CanOuterPlus()
        {
            return true;
        }

        private void OuterPlus()
        {
            AnnulusOuterRadius++;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandOuterMinus
        private RelayCommand commandOuterMinus;
        public ICommand CommandOuterMinus
        {
            get
            {
                if (commandOuterMinus == null)
                {
                    commandOuterMinus = new RelayCommand(param => this.OuterMinus(), param => this.CanOuterMinus());
                }
                return commandOuterMinus;
            }
        }

        private bool CanOuterMinus()
        {
            return true;
        }

        private void OuterMinus()
        {
            AnnulusOuterRadius--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandReport
        private RelayCommand commandReport;
        public ICommand CommandReport
        {
            get
            {
                if (commandReport == null)
                {
                    commandReport = new RelayCommand(param => this.Report(), param => this.CanReport());
                }
                return commandReport;
            }
        }

        private bool CanReport()
        {
            return true;
        }

        private void Report()
        {
            foreach (ApertureMeasurementEntry m in Measures)
            {
                AAVSOList newElement;
                AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                    StroblhofwarteImage.Instance.GetJD().ToString(CultureInfo.InvariantCulture),
                    m.Mag.ToString("0.####", CultureInfo.InvariantCulture),
                    m.MagErr.ToString("0.####", CultureInfo.InvariantCulture),
                    Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToAAVSOFilter(StroblhofwarteImage.Instance.GetFilter()),
                    "NO",
                    "STD",
                    StarDataRelay.Instance.CompName,
                    m.CompMag.ToString(CultureInfo.InvariantCulture),
                    StarDataRelay.Instance.CheckName,
                    m.KMag.ToString(CultureInfo.InvariantCulture),
                    "na",
                    "na",
                    StarDataRelay.Instance.ChartId,
                    "na", out newElement);
                
                // Store reported measurments also in the permanent db:
                AAVSOExtendedFileFormat permanentDb = new AAVSOExtendedFileFormat(PERMADB);
                permanentDb.Add(newElement);
            }

        }
        #endregion

        #region commandDel
        private RelayCommand commandDel;
        public ICommand CommandDel
        {
            get
            {
                if (commandDel == null)
                {
                    commandDel = new RelayCommand(param => this.Del(param), param => this.CanDel());
                }
                return commandDel;
            }
        }

        private bool CanDel()
        {
            return true;
        }

        private void Del(object o)
        {
            ApertureMeasurementEntry entry = o as ApertureMeasurementEntry;
            if (entry == null) return;
            if (o is ApertureMeasurementEntry)
            {
                int i = 0;
                int idx = -1;
                foreach(ApertureMeasurementEntry m in Measures)
                {
                    if(m.Ident == entry.Ident)
                    {
                        idx = i;
                        break;
                    }
                    i++;
                }
                if (idx != -1)
                    Measures.RemoveAt(idx);
            }
        }
        #endregion

        #region commandStart
        private RelayCommand commandStart;
        public ICommand CommandStart
        {
            get
            {
                if (commandStart == null)
                {
                    commandStart = new RelayCommand(param => this.Start(), param => this.CanStart());
                }
                return commandStart;
            }
        }

        private bool CanStart()
        {
            if (StroblhofwarteImage.Instance.IsValid == false) return false;
            if (PhotoState == enumPhotoState.INIT) return true;
            return false;
        }

        private void Start()
        {
            PhotoState = enumPhotoState.COMP;
            StepInfo = "Select C [" + StarDataRelay.Instance.CompName + "]";
            StarDataRelay.Instance.UserInfoVisibility = true;
            StarDataRelay.Instance.UserInfo = StepInfo;
            

        }
        #endregion

        #region commandTransform
        private RelayCommand commandTransform;
        public ICommand CommandTransform
        {
            get
            {
                if (commandTransform == null)
                {
                    commandTransform = new RelayCommand(param => this.Transform(), param => this.CanTransform());
                }
                return commandTransform;
            }
        }

        private bool CanTransform()
        {
            int filterCount = 0;
            List<string> filter = new List<string>();
            foreach(ApertureMeasurementEntry e in Measures)
            {
                if(!filter.Contains(e.Filter))
                {
                    filter.Add(e.Filter);
                }
            }
            if (filter.Count == 2) return true;
            return false;
        }

        private void Transform()
        {
            DialogTransformationView dlg = new DialogTransformationView();
            DialogTransformationViewModel model = dlg.DataContext as DialogTransformationViewModel;
            if (model != null)
                model.Measures = Measures;
            dlg.ShowDialog();
        }
        #endregion

        #region ctor

        public ApertureViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition;
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
            PhotoState = enumPhotoState.INIT;
            Z = 21.1;
            StarDataRelay.Instance.CheckStarChanged += Instance_CheckStarChanged;
            StarDataRelay.Instance.CompStarChanged += Instance_CompStarChanged;
            ContentId = "ApertureViewModel";
            _centroidSearchRadius = Properties.Settings.Default.MagnificationN / 8;

        }

        private void Instance_CompStarChanged(object? sender, EventArgs e)
        {
            ReferenceMag = StarDataRelay.Instance.CompMag;
        }

        private void Instance_CheckStarChanged(object? sender, EventArgs e)
        {
            CheckReferenceMag = StarDataRelay.Instance.CheckMag;
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            PhotoState = enumPhotoState.INIT;
            StarDataRelay.Instance.UserInfoVisibility = false;
            StarDataRelay.Instance.UserInfo = "";
            StepInfo = "<Start> to start measurement";
            try
            {
                InstrumentObject instr = AperturePhotometry.Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
                        (int)StroblhofwarteImage.Instance.GetSensorGain(),
                        (int)StroblhofwarteImage.Instance.GetSensorOffset(),
                        StroblhofwarteImage.Instance.GetBinning(),
                        StroblhofwarteImage.Instance.GetSensorSetTemp());
                ApertureSize = (int)instr.Aperture;
                AnnulusInnerRadius = (int)instr.InnerAnnulus;
                AnnulusOuterRadius = (int)instr.OuterAnnulus;
            }
            catch (Exception ex)
            {
                // could happen when no data inside the image. 
            }

        }

        private void Instance_NewCursorClickPosition(object? sender, EventArgs e)
        {
            try
            {
                AutodetectAperture();
                UpdateApertureMeasurement();
                if (MagMeasurement.Instance.CalibrationMode)
                {
                    OnPropertyChanged("ImageSource");
                    return;
                }
                if (PhotoState == enumPhotoState.COMP)
                {
                    StepInfo = "Select VAR [" + StroblhofwarteImage.Instance.GetObject() + "]";
                    StarDataRelay.Instance.UserInfo = StepInfo;
                    StarDataRelay.Instance.UserInfoVisibility = true;
                    PhotoState = enumPhotoState.VAR;
                }
                else if (PhotoState == enumPhotoState.VAR)
                {
                    StepInfo = "Select K [" + StarDataRelay.Instance.CheckName + "]";
                    StarDataRelay.Instance.UserInfo = StepInfo;
                    StarDataRelay.Instance.UserInfoVisibility = true;
                    PhotoState = enumPhotoState.CHECK;
                }
                else if (PhotoState == enumPhotoState.CHECK)
                {
                    StepInfo = "<Start> to start measurement";
                    StarDataRelay.Instance.UserInfoVisibility = false;
                    StarDataRelay.Instance.UserInfo = "";
                    PhotoState = enumPhotoState.INIT;
                }
                OnPropertyChanged("ImageSource");
                // If the reference data are prefilled, recalculate Z
                ReferenceMag = _referenceMag;
            }
            catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        private void AutodetectAperture()
        {
            try
            {
                Stroblhofwarte.AperturePhotometry.Centroid photometer = new Centroid(_centroidSearchRadius);
                Stroblhofwarte.AperturePhotometry.Radius radius = new Radius();
                _starCentroid = photometer.GetCentroid(StroblhofwarteImage.Instance.CursorClickPosition.X, StroblhofwarteImage.Instance.CursorClickPosition.Y);
            }
            catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        private void UpdateApertureMeasurement()
        {
            try
            {
                Stroblhofwarte.AperturePhotometry.ApertureMeasure measure = new ApertureMeasure();
                Stroblhofwarte.AperturePhotometry.Radius radius = new Radius();
                double inner = 0.0;
                double outer = 0.0;
                MeasurementResult meas = measure.Measure(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, (double)ApertureSize, AnnulusInnerRadius, AnnulusOuterRadius);
                if (MagMeasurement.Instance.CalibrationMode)
                {
                    Mag = measure.Magnitude(meas, 1.0); // Machine Mag here!
                    MagMeasurement.Instance.Update(Mag, _starCentroid.X, _starCentroid.Y, true);
                    return;
                }

                // Check if Filter match:
                if(Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToAAVSOFilter(StroblhofwarteImage.Instance.GetFilter()) != StarDataRelay.Instance.Filter)
                {
                    // Filter does not match!
                    Error = "AAVSO Data filter != Image filter!";
                    return;
                }
                Error = "";
                double M = measure.Magnitude(meas, Z);
                Mag = M;
                if (PhotoState == enumPhotoState.COMP)
                {
                    if (StarDataRelay.Instance.KValid == false || StarDataRelay.Instance.CValid == false)
                    {
                        Error = "No C and K star selected!";
                        return;
                    }
                    Error = "";
                    _currentMeas = new ApertureMeasurementEntry();
                    Measures.Add(_currentMeas);
                    _currentMeas.Filter = StarDataRelay.Instance.Filter;
                    _currentMeas.Name = StroblhofwarteImage.Instance.GetObject();
                    _currentMeas.CompMag = StarDataRelay.Instance.CompMag;
                    _currentMeas.KMag = StarDataRelay.Instance.CheckMag;
                    _currentMeas.CompMeasMag = Mag;
                    _currentMeas.CompMachineMag = measure.Magnitude(meas, 1.0);
                    _measComp = meas;
                }
                if (PhotoState == enumPhotoState.VAR)
                {
                    _currentMeas.Mag = Mag;
                    _currentMeas.MachineMag = measure.Magnitude(meas, 1.0);
                    _measVar = meas;
                }
                if (PhotoState == enumPhotoState.CHECK)
                {
                    _currentMeas.KMeasMag = Mag;
                    _currentMeas.KMachineMag = measure.Magnitude(meas, 1.0);
                    _measCheck = meas;
                }
                OnPropertyChanged("Measures");
                MagMeasurement.Instance.Update(Mag, _starCentroid.X, _starCentroid.Y, false);
            }
            catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        #endregion
    }

    public class ApertureMeasurementEntry : INotifyPropertyChanged
    {

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
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

        private double _mag;
        public double Mag
        {
            get { return _mag; }
            set
            {
                _mag = value;
                OnPropertyChanged("Mag");
            }
        }

        private double _magErr;
        public double MagErr
        {
            get { return _magErr; }
            set
            {
                _magErr = value;
                OnPropertyChanged("MagErr");
            }
        }

        private double _machinemag;
        public double MachineMag
        {
            get { return _machinemag; }
            set
            {
                _machinemag = value;
                OnPropertyChanged("MachineMag");
            }
        }

        private double _compMag;
        public double CompMag
        {
            get { return _compMag; }
            set
            {
                _compMag = value;
                OnPropertyChanged("CompMag");
            }
        }

        private double _compMeasMag;
        public double CompMeasMag
        {
            get { return _compMeasMag; }
            set
            {
                _compMeasMag = value;
                OnPropertyChanged("CompMeasMag");
            }
        }

        private double _compMachineMag;
        public double CompMachineMag
        {
            get { return _compMachineMag; }
            set
            {
                _compMachineMag = value;
                OnPropertyChanged("CompMachineMag");
            }
        }

        private double _compErr;
        public double CompErr
        {
            get { return _compErr; }
            set
            {
                _compErr = value;
                OnPropertyChanged("CompMeasMag");
            }
        }

        private double _kMag;
        public double KMag
        {
            get { return _kMag; }
            set
            {
                _kMag = value;
                OnPropertyChanged("KMag");
            }
        }

        private double _kMeasMag;
        public double KMeasMag
        {
            get { return _kMeasMag; }
            set
            {
                _kMeasMag = value;
                OnPropertyChanged("KMeasMag");
            }
        }

        private double _kMachineMag;
        public double KMachineMag
        {
            get { return _kMachineMag; }
            set
            {
                _kMachineMag = value;
                OnPropertyChanged("KMachineMag");
            }
        }

        private Guid _ident;
        public Guid Ident
        {
            get { return _ident; }
        }



        #region Ctor

        public ApertureMeasurementEntry()
        {
            _ident = Guid.NewGuid();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}