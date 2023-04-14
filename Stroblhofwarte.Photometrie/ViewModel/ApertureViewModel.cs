using Stroblhofwarte.AperturePhotometry;
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
using System.Windows.Media.Imaging;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ApertureViewModel : DockWindowViewModel
    {
        #region Properties

        private int _centroidSearchRadius = 100;

        private Point _starCentroid = new Point(0, 0);
        private double _apertureR = 0.0;
        private double _annulusInnerR = 0.0;
        private double _annulusOuterR = 0.0;

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
                        (int)(magN/2 - _apertureR),
                        (int)(magN/2 - _apertureR),
                        (int)(2 * _apertureR),
                        (int)(2 * _apertureR));
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f),
                        (int)(magN / 2 - _annulusInnerR),
                        (int)(magN / 2 - _annulusInnerR),
                        (int)(2 * _annulusInnerR),
                        (int)(2 * _annulusInnerR));
                    g.DrawEllipse(new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 1.0f),
                        (int)(magN / 2 - _annulusOuterR),
                        (int)(magN / 2 - _annulusOuterR),
                        (int)(2 * _annulusOuterR),
                        (int)(2 * _annulusOuterR));
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

        #endregion

        #region ctor

        public ApertureViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition;
            Z = 21.1;
        }

        private void Instance_NewCursorClickPosition(object? sender, EventArgs e)
        {
            try
            {
                Stroblhofwarte.AperturePhotometry.Centroid photometer = new Centroid(_centroidSearchRadius);
                Stroblhofwarte.AperturePhotometry.ApertureMeasure measure = new ApertureMeasure();
                _starCentroid = photometer.GetCentroid(StroblhofwarteImage.Instance.CursorClickPosition.X, StroblhofwarteImage.Instance.CursorClickPosition.Y);

                double apertureR = 0.0;
                double outerAnnulusR = 0.0;
                double innerAnnulusR = 0.0;
                Stroblhofwarte.AperturePhotometry.Radius r = new Radius(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, out apertureR, out innerAnnulusR, out outerAnnulusR);
                _apertureR = apertureR;
                _annulusInnerR = innerAnnulusR;
                _annulusOuterR = outerAnnulusR;
                MeasurementResult meas = measure.Measure(_starCentroid.X, _starCentroid.Y, _centroidSearchRadius, _apertureR, _annulusInnerR, _annulusOuterR);
                double M = measure.Magnitude(meas, Z);
                Mag = M;
                OnPropertyChanged("ImageSource");
            } catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        #endregion
    }
}