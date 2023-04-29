﻿using MahApps.Metro.Controls.Dialogs;
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
        private string PERMADB = "mymeasurements.db";

        #region Properties
        private MeasurementResult _measVar;
        private MeasurementResult _measComp;
        private MeasurementResult _measCheck;
        private bool _apertureChanged = false;

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

        private int _centroidSearchRadius = 30;

        private Point _starCentroid = new Point(0, 0);

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
                        Pen p = new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f);
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
                _apertureChanged = true;
                if (AnnulusInnerRadius <= value) AnnulusInnerRadius = value + 1;
                UpdateApertureMeasurement();
                PhotoState = enumPhotoState.VAR;
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
                InstrumentObject instr = Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
                (int)StroblhofwarteImage.Instance.GetSensorGain(),
                (int)StroblhofwarteImage.Instance.GetSensorOffset(),
                StroblhofwarteImage.Instance.GetSensorSetTemp());
                instr.Aperture = _apertureSize;
                instr.InnerAnnulus = _annulusInnerRadius;
                instr.OuterAnnulus = _annulusOuterRadius;
                Instruments.Instance.Save();
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
            if (PhotoState == enumPhotoState.CHECK) return true;
            return false;
        }

        private void Report()
        {
            AAVSOList newElement;
            AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                StroblhofwarteImage.Instance.GetJD().ToString(CultureInfo.InvariantCulture),
                VarMag.ToString("0.##", CultureInfo.InvariantCulture), 
                VarError.ToString("0.####", CultureInfo.InvariantCulture),
                Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToAAVSOFilter(StroblhofwarteImage.Instance.GetFilter()),
                "NO",
                "STD",
                StarDataRelay.Instance.CompName,
                StarDataRelay.Instance.CompMag.ToString(CultureInfo.InvariantCulture),
                StarDataRelay.Instance.CheckName,
                StarDataRelay.Instance.CheckMag.ToString(CultureInfo.InvariantCulture),
                "na",
                "na",
                StarDataRelay.Instance.ChartId,
                "na", out newElement);
            PhotoState = enumPhotoState.VAR;

            // Store reported measurments also in the permanent db:
            AAVSOExtendedFileFormat permanentDb = new AAVSOExtendedFileFormat(PERMADB);
            permanentDb.Add(newElement);

        }
        #endregion
        #region ctor

        public ApertureViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition;
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
            PhotoState = enumPhotoState.VAR;
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
            PhotoState = enumPhotoState.VAR;
            try
            {
                InstrumentObject instr = Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
                        (int)StroblhofwarteImage.Instance.GetSensorGain(),
                        (int)StroblhofwarteImage.Instance.GetSensorOffset(),
                        StroblhofwarteImage.Instance.GetSensorSetTemp());
                ApertureSize = (int)instr.Aperture;
                AnnulusInnerRadius = (int)instr.InnerAnnulus;
                AnnulusOuterRadius = (int)instr.OuterAnnulus;
            } catch (Exception ex)
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
                // If the reference data are prefilled, recalculate Z
                ReferenceMag = _referenceMag;
            } catch (Exception ex)
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