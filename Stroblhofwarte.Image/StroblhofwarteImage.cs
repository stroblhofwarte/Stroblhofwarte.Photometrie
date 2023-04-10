using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.Image.Interface;
using System.ComponentModel;
using System.Drawing;

namespace Stroblhofwarte.Image
{
    public class StroblhofwarteImage : IStroblhofwarteImage
    {
        #region Singelton
        private static StroblhofwarteImage _instance = null;

        public static StroblhofwarteImage Instance {
            get
            {
                if (_instance == null)
                    _instance = new StroblhofwarteImage();
                return _instance;
            }
        }

        #endregion

        #region Properties

        private IFits _imageData = null;

        public bool IsValid
        {
            get
            {
                if (_imageData == null) return false;
                return _imageData.IsValid;
            }
        }

        public WorldCoordinateSystem WCS
        {
            get
            {
                if (_imageData == null) return null;
                return _imageData.WCS;
            }
        }

        #endregion

        #region Events

        public event EventHandler NewImageLoaded;

        #endregion

        #region Ctor

        public StroblhofwarteImage()
        {
            
        }

        #endregion

        public bool Load(string filename)
        {
            _imageData = new StroblhofwarteFITS(filename);
            if(NewImageLoaded != null)
            {
                NewImageLoaded(this, null);
            }
            return _imageData.IsValid;
        }

        public Bitmap GetBitmap()
        {
            if (_imageData == null) return null;
            if (!_imageData.IsValid) return null;
            return _imageData.GetImage();
        }
    }
}