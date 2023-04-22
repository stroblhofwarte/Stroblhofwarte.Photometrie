using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;

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
                    bitmapimage.DecodePixelWidth = StroblhofwarteImage.Instance.Width;
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

        private double _annotationScale = 3.0;
        public double AnnotationScale
        {
            set
            {
                _annotationScale = value;
                OnPropertyChanged("AnnotationScale");
            }
            get
            {
                return _annotationScale;
            }
        }

        private bool _annotation = false;
        public bool Annotation
        {
            set
            {
                _annotation = value;
                OnPropertyChanged("Annotation");
            }
            get
            {
                return _annotation;
            }
        }

        #endregion

        #region Commands

        private RelayCommand annotateCommand;
        public ICommand AnnotateCommand
        {
            get
            {
                if (annotateCommand == null)
                {
                    annotateCommand = new RelayCommand(param => this.Annotate(), param => this.CanAnnotate());
                }
                return annotateCommand;
            }
        }

        private bool CanAnnotate()
        {
            if (StroblhofwarteImage.Instance.IsValid) return true;
            return false;
        }

        private async void Annotate()
        {
           if(StroblhofwarteImage.Instance.Annotate)
            {
                StroblhofwarteImage.Instance.Annotate = false;
                Annotation = false;
            }
           else
            {
                StroblhofwarteImage.Instance.AnnotateScale = AnnotationScale;
                StroblhofwarteImage.Instance.Annotate = true;
                Annotation = true;
            }
            OnPropertyChanged("ImageSource");
        }

        #endregion
        #region Ctor

        public ImageViewModel()
        {
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
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
