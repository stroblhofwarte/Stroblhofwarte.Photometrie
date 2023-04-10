using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.Image;
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
                if (!StroblhofwarteImage.Instance.IsValid) return null;
                using (MemoryStream memory = new MemoryStream())
                {
                    StroblhofwarteImage.Instance.GetBitmap().Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
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

        private string _coordinateText = string.Empty;
        public String CoordinateText
        {
            set
            {
                _coordinateText = value;
                OnPropertyChanged("CoordinateText");
            }
            get
            {
                return _coordinateText;
            }
        }

        #endregion
        #region Ctor

        public ImageViewModel()
        {
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath) + "\\Ring Nebula(2)_2021-05-31_22-41-52_GAIN_0_G_-20.00_600.00s_0001.fits";
            StroblhofwarteImage.Instance.Load(strWorkPath);
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            OnPropertyChanged("ImageSource");
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
