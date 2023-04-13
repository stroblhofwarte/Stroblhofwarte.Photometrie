using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class MagnificationViewModel : DockWindowViewModel
    {
        #region Properties

        public BitmapImage ImageSource
        {
            get
            {
                if (!StroblhofwarteImage.Instance.IsValid) return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    int magN = Properties.Settings.Default.MagnificationN;
                    StroblhofwarteImage.Instance.GetSubimage(StroblhofwarteImage.Instance.CursorPosition, magN, magN).Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
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

        #endregion

        #region ctor

        public MagnificationViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorPosition += Instance_NewCursorPosition;
        }

        private void Instance_NewCursorPosition(object? sender, EventArgs e)
        {
            OnPropertyChanged("ImageSource");
        }
        #endregion
    }
}
