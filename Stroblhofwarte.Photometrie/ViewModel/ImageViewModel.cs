using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ImageViewModel : DockWindowViewModel
    {
        #region Properties

        private StroblhofwarteFITS _fitsImage = null;

        public BitmapImage ImageSource
        {
            get
            {
                if (_fitsImage == null) return null;
                using (MemoryStream memory = new MemoryStream())
                {
                    _fitsImage.GetImage().Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
            }
        }

        #endregion
        #region Ctor

        public ImageViewModel()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath) + "\\Ring Nebula(2)_2021-05-31_22-41-52_GAIN_0_G_-20.00_600.00s_0001.fits";
            _fitsImage = new StroblhofwarteFITS(strWorkPath);
        }

        #endregion

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
