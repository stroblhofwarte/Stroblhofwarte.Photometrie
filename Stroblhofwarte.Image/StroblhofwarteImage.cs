using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.Image.Interface;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Media;

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

        private List<Annotation> _annotations = new List<Annotation>();
        private IFits _imageData = null;

        public bool Annotate { set; get; }
        public double AnnotateScale { set; get; }
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

        public void ClearAnnotation()
        {
            _annotations.Clear();
        }

        public Bitmap GetBitmap()
        {
            if (_imageData == null) return null;
            if (!_imageData.IsValid) return null;
            if (Annotate && WCS != null)
            {
                Bitmap bmp =(Bitmap) _imageData.GetImage().Clone();
                Graphics g = Graphics.FromImage(bmp);
                foreach (Annotation a in _annotations)
                {
                    System.Windows.Point p = WCS.GetCoordinates(a.Coor);
                    g.DrawLine(new System.Drawing.Pen(System.Drawing.Brushes.Yellow, 10.0f),
                        (int)p.X + (int)(10* AnnotateScale),
                        (int)p.Y + (int)(10* AnnotateScale),
                        (int)p.X+(int)(20 * AnnotateScale),
                        (int)p.Y+(int)(20 * AnnotateScale));
                    g.DrawString(a.Name, new Font("Segoe UI", (float)(24.0 * AnnotateScale)), System.Drawing.Brushes.Yellow, 
                        new System.Drawing.Point((int)p.X + (int)(30 * AnnotateScale),
                        (int)p.Y + (int)(30 * AnnotateScale)));
                }   
                return bmp;
            }
            return _imageData.GetImage();
        }

        public void AddAnnotation(string name, Coordinates c)
        {
            _annotations.Add(new Annotation() { Name = name, Coor = c }); 
        }
    }

    public class Annotation
    {
        public string Name { get; set;}
        public Coordinates Coor { get; set; }

    }
}