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
    public class StroblhofwarteFITS : IFits
    {
        #region Properties

        private DataObjects.FitsImage _image = null;
        public bool IsValid { get; private set; }
        public string ErrorText { get; private set; }

        #endregion

        #region CTor

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
}
