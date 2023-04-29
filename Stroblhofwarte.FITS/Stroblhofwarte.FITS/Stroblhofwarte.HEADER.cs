using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.FITS
{
    public class StroblhofwarteHEADER : IFits
    {
        #region Properties

        private DataObjects.FitsImage _image = null;
        public DataObjects.FitsImage RawImage
        {
            get
            {
                return _image;
            }
        }

        public bool IsValid { get; private set; }
        public string ErrorText { get; private set; }

        public WorldCoordinateSystem WCS
        {
            get
            {
                if (_image == null) return null;
                if (!_image.WCSValid) return null;
                return _image.WCS;
            }
        }


        #endregion

        #region CTor

        public StroblhofwarteHEADER(string filename)
        {
            try
            {
                IsValid = false;
                ErrorText = String.Empty;
                Uri uri = new Uri(filename);
                _image = FITS.LoadHeaderOnly(uri);
                if (_image != null) IsValid = true;
            }
            catch (Exception ex)
            {
                ErrorText = ex.ToString();
                IsValid = false;
            }
        }

        #endregion
        public Bitmap GetImage()
        {
            if (IsValid)
            {
                return _image.BitmapData;
            }
            return new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
        }

        public int Width()
        {
            if (IsValid)
            {
                return _image.Width;
            }
            return 1;
        }

        public int Height()
        {
            if (IsValid)
            {
                return _image.Height;
            }
            return 1;
        }

        public FitsImage GetRawImage()
        {
            throw new NotImplementedException();
        }

        public double GetExposureTime()
        {
            if (_image == null) return 0.0;
            return _image.GetExposureTime();
        }

        public string GetInstrument()
        {
            if (_image == null) return string.Empty;
            return _image.GetInstrument().Trim();
        }

        public string GetTelescope()
        {
            if (_image == null) return string.Empty;
            return _image.GetTelescope().Trim();
        }

        public double GetFocalLength()
        {
            if (_image == null) return 0.0;
            return _image.GetFocalLength();
        }

        public double GetFocalRatio()
        {
            if (_image == null) return 0.0;
            return _image.GetFocalRatio();
        }

        public string GetFilter()
        {
            if (_image == null) return string.Empty;
            return _image.GetFilter().Trim();
        }

        public string GetObject()
        {
            if (_image == null) return string.Empty;
            return _image.GetObject().Trim();
        }

        public DateTime GetObservationTimeUTC()
        {
            if (_image == null) return DateTime.MinValue;
            return _image.GetObservationTimeUTC();
        }

        public DateTime GetObservationTime()
        {
            if (_image == null) return DateTime.MinValue;
            return _image.GetObservationTime();
        }

        public double GetSensorSetTemp()
        {
            if (_image == null) return double.MaxValue;
            return _image.GetSensorSetTemp();
        }

        public double GetSensorTemp()
        {
            if (_image == null) return double.MaxValue;
            return _image.GetSensorTemp();
        }

        public double GetSensorGain()
        {
            if (_image == null) return double.MinValue;
            return _image.GetSensorGain();
        }

        public double GetSensorOffset()
        {
            if (_image == null) return double.MinValue;
            return _image.GetSensorOffset();
        }
        public string GetBinningId()
        {
            if (_image == null) return string.Empty;
            return _image.GetBinningId();
        }
    }
}
