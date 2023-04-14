using Stroblhofwarte.AperturePhotometry;
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
    public class ApertureViewModel : DockWindowViewModel
    {
        #region Properties

        private Point _starCentroid = new Point(0, 0);

        public BitmapImage ImageSource
        {
            get
            {
                if (!StroblhofwarteImage.Instance.IsValid) return null;

                using (MemoryStream memory = new MemoryStream())
                {
                    int magN = Properties.Settings.Default.MagnificationN;
                    StroblhofwarteImage.Instance.GetSubimage(_starCentroid, magN, magN).Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
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

        public ApertureViewModel()
        {
            StroblhofwarteImage.Instance.NewCursorClickPosition += Instance_NewCursorClickPosition; ;
        }

        private void Instance_NewCursorClickPosition(object? sender, EventArgs e)
        {
            try
            {
                Stroblhofwarte.AperturePhotometry.Centroid photometer = new Centroid(100);
                _starCentroid = photometer.GetCentroid(StroblhofwarteImage.Instance.CursorClickPosition.X, StroblhofwarteImage.Instance.CursorClickPosition.Y);
                OnPropertyChanged("ImageSource");
            } catch (Exception ex)
            {
                // Centroid is outside the search region. 
            }
        }

        #endregion
    }
}