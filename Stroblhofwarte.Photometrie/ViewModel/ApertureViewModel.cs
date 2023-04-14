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

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public enum enumPhotoState
    {
        VAR,
        COMP,
        CHECK
    }
    public class ApertureViewModel : DockWindowViewModel
    {
        #region Properties
        private MeasurementResult _measVar;
        private MeasurementResult _measComp;
        private MeasurementResult _measCheck;


        private enumPhotoState _photoState;
        public enumPhotoState PhotoState
        {
            get { return _photoState; }
            set
            {
                _photoState = value;
                if(_photoState == enumPhotoState.VAR)
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

        private int _centroidSearchRadius = 100;

        private Point _starCentroid = new Point(0, 0);

        public BitmapImage ImageSource
        {
            get
            {
                if (!StroblhofwarteImage.Instance.IsValid) return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    int magN = Properties.Settings.Default.MagnificationN;
                    Bitmap bitmap = StroblhofwarteImage.Instance.GetSubimage(_starCentroid, magN, magN);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Green, 1.0f),
                        (int)(magN/2 - ApertureSize),
                        (int)(magN/2 - ApertureSize),
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

        private bool _apertureEdit;
        public bool ApertureEdit

        {
            get { return _apertureEdit; }
            set
            {
                _apertureEdit = value;
                OnPropertyChanged("ApertureEdit");
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

        private int _referenceMag;
        public int ReferenceMag
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
                VarError = CheckMag - (double)(CheckReferenceMag / 10.0);
                OnPropertyChanged("ReferenceMag");
            }
        }

        private int _checkReferenceMag;
        public int CheckReferenceMag
        {
            get { return _checkReferenceMag; }
            set
            {
                _checkReferenceMag = value;
                VarError = CheckMag - (double)(CheckReferenceMag / 10.0);
                OnPropertyChanged("CheckReferenceMag");
            }
        }


        private bool _autoCentroid;
        public bool AutoCentroid

        {
            get { return _autoCentroid; }
            set
            {
                _autoCentroid = value;
                ApertureEdit = !value;
                OnPropertyChanged("AutoCentroid");
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

        private string _magString;
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
                UpdateApertureMeasurement();
                if(PhotoState == enumPhotoState.VAR)
                {
                    VarMag = Mag;
                }
                if (PhotoState == enumPhotoState.COMP)
                {
                    CompMag = Mag;
                }
                if (PhotoState == enumPhotoState.CHECK)
                {
                    CheckMag = Mag;
                }
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
                OnPropertyChanged("AnnulusOuterRadius");
            }
        }

        #endregion
        #region commandResetPhoto
        private RelayCommand commandResetPhoto;
        public ICommand CommandResetPhoto
        {
            get
            {
                if (commandResetPhoto == null)
                {
                    commandResetPhoto = new RelayCommand(param => this.ResetPhoto(), param => this.CanResetPhoto());
                }
                return commandResetPhoto;
            }
        }

        private bool CanResetPhoto()
        {
            if (PhotoState == enumPhotoState.CHECK) return true;
            return false;
        }

        private void ResetPhoto()
        {
            PhotoState = enumPhotoState.VAR;
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
            if (!AutoCentroid && ApertureSize < 30) return true;
            return false;
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
            if (!AutoCentroid && ApertureSize > 3) return true;
            return false;
        }

        private void ApertureMinus()
        {
            ApertureSize--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region CommandCentroidXPlus
        private RelayCommand commandCentroidXPlus;
        public ICommand CommandCentroidXPlus
        {
            get
            {
                if (commandCentroidXPlus == null)
                {
                    commandCentroidXPlus = new RelayCommand(param => this.CentroidXPlus(), param => this.CanCentroidXPlus());
                }
                return commandCentroidXPlus;
            }
        }

        private bool CanCentroidXPlus()
        {
            if (!AutoCentroid) return true;
            return false;
        }

        private void CentroidXPlus()
        {
            _starCentroid.X++;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandCentroidYPlus
        private RelayCommand commandCentroidYPlus;
        public ICommand CommandCentroidYPlus
        {
            get
            {
                if (commandCentroidYPlus == null)
                {
                    commandCentroidYPlus = new RelayCommand(param => this.CentroidYPlus(), param => this.CanCentroidYPlus());
                }
                return commandCentroidYPlus;
            }
        }

        private bool CanCentroidYPlus()
        {
            if (!AutoCentroid) return true;
            return false;
        }

        private void CentroidYPlus()
        {
            _starCentroid.Y++;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region CommandCentroidXMinus
        private RelayCommand commandCentroidXMinus;
        public ICommand CommandCentroidXMinus
        {
            get
            {
                if (commandCentroidXMinus == null)
                {
                    commandCentroidXMinus = new RelayCommand(param => this.CentroidXMinus(), param => this.CanCentroidXMinus());
                }
                return commandCentroidXMinus;
            }
        }

        private bool CanCentroidXMinus()
        {
            if (!AutoCentroid) return true;
            return false;
        }

        private void CentroidXMinus()
        {
            _starCentroid.X--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion

        #region commandCentroidYMinus
        private RelayCommand commandCentroidYMinus;
        public ICommand CommandCentroidYMinus
        {
            get
            {
                if (commandCentroidYMinus == null)
                {
                    commandCentroidYMinus = new RelayCommand(param => this.CentroidYMinus(), param => this.CanCentroidYMinus());
                }
                return commandCentroidYMinus;
            }
        }

        private bool CanCentroidYMinus()
        {
            if (!AutoCentroid) return true;
            return false;
        }

        private void CentroidYMinus()
        {
            _starCentroid.Y--;
            UpdateApertureMeasurement();
            OnPropertyChanged("ImageSource");
        }
        #endregion
        #region ctor

        public ApertureViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition;
            AutoCentroid = true;
            PhotoState = enumPhotoState.VAR;
            Z = 21.1;
        }

        private void Instance_NewCursorClickPosition(object? sender, EventArgs e)
        {
            try
            {
                if (AutoCentroid)
                {
                    AutodetectAperture();
                }
                else
                {
                    _starCentroid = StroblhofwarteImage.Instance.CursorClickPosition;
                    ManualAperture();
                }
                if (PhotoState == enumPhotoState.VAR)
                {
                    PhotoState = enumPhotoState.COMP;
                }
                else if (PhotoState == enumPhotoState.COMP)
                {
                    PhotoState = enumPhotoState.CHECK;
                }
                else
                {

                }
                OnPropertyChanged("ImageSource");
            } catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        private void ManualAperture()
        {
            try
            {
                Stroblhofwarte.AperturePhotometry.Radius radius = new Radius();
              
                double apertureR = 0.0;
                double outerAnnulusR = 0.0;
                double innerAnnulusR = 0.0;
                radius.CalculateRadius(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, out apertureR, out innerAnnulusR, out outerAnnulusR);
                AnnulusInnerRadius = innerAnnulusR;
                AnnulusOuterRadius = outerAnnulusR;
                ApertureSize = (int)apertureR;
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

                double apertureR = 0.0;
                double outerAnnulusR = 0.0;
                double innerAnnulusR = 0.0;
                radius.CalculateRadius(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, out apertureR, out innerAnnulusR, out outerAnnulusR);
                AnnulusInnerRadius = innerAnnulusR;
                AnnulusOuterRadius = outerAnnulusR;
                ApertureSize = (int)apertureR;
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
                radius.CalculateCircles((double)ApertureSize, out inner, out outer);
                AnnulusOuterRadius = outer;
                AnnulusInnerRadius = inner;
                MeasurementResult meas = measure.Measure(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, (double)ApertureSize, AnnulusInnerRadius, AnnulusOuterRadius);
                if (PhotoState == enumPhotoState.VAR) _measVar = meas;
                if (PhotoState == enumPhotoState.COMP) _measComp = meas;
                if (PhotoState == enumPhotoState.CHECK) _measCheck = meas;
                double M = measure.Magnitude(meas, Z);
                Mag = M;
            }
            catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        #endregion
    }
}