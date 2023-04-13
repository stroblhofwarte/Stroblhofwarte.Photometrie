using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using static Stroblhofwarte.FITS.CfitsioNative;

namespace Stroblhofwarte.FITS
{
    public class StroblhofwarteFITS : IFits
    {
        #region Properties

        private DataObjects.FitsImage _image = null;
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

        public StroblhofwarteFITS()
        { }

        public StroblhofwarteFITS(string filename)
        {
            try
            {
                IsValid = false;
                ErrorText = String.Empty;
                Uri uri = new Uri(filename);
                _image = FITS.Load(uri, false);
                if(_image != null) IsValid = true;
            } catch (Exception ex)
            {
                ErrorText = ex.ToString();
                IsValid = false;
            }
        }

        #endregion
        public Bitmap GetImage()
        {
            if(IsValid)
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
    }

    internal class HDUValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }
}
