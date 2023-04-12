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

        public bool MergeHeaderIntoImage(string filenameNewHeader, string filenameImage)
        {
            List<HDUValue> newHDU = new List<HDUValue>();
            IntPtr fptr = IntPtr.Zero; ;
            int status = 0;
            IOMODE iomode = IOMODE.READWRITE;

            int ret = fits_open_file(out fptr, filenameNewHeader, IOMODE.READONLY, out status);

            CfitsioNative.fits_get_hdrspace(fptr, out var numKeywords, out var numMoreKeywords, out status);
            CfitsioNative.CheckStatus("fits_get_hdrspace", status);
            for (int headerIdx = 1; headerIdx <= numKeywords; ++headerIdx)
            {
                CfitsioNative.fits_read_keyn(fptr, headerIdx, out var keyName, out var keyValue, out var keyComment);
                if (keyName == "WCSAXES") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CTYPE1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CTYPE2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "EQUINOX") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "LONPOLE") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "LATPOLE") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CRVAL1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CRVAL2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CRPIX1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CRPIX2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CUNIT1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CUNIT2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CD1_1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CD1_2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CD2_1") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "CD2_2") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "IMAGEW") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
                if (keyName == "IMAGEH") newHDU.Add(new HDUValue() { Name = keyName, Value = keyValue, Comment = keyComment });
            }
            fits_close_file(fptr, out status);

            ret = fits_open_file(out fptr, filenameNewHeader, IOMODE.READWRITE, out status);
            foreach(HDUValue hdu in newHDU)
            {
                fits_write_key_str(fptr, hdu.Name, hdu.Value, hdu.Comment, out status);
            }
            fits_close_file(fptr, out status);

            return true;
       
        }


    }

    internal class HDUValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }
}
