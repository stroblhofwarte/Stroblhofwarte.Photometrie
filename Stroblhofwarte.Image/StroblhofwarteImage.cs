using Stroblhofwarte.FITS;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.Image.Interface;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

        private System.Drawing.Point _cursorClickPosition;
        public System.Drawing.Point CursorClickPosition
        {
            get { return _cursorClickPosition; }
            set
            {
                _cursorClickPosition = value;
                if (NewCursorClickPosition != null)
                {
                    NewCursorClickPosition(this, null);
                }
            }
        }

        private System.Drawing.Point _cursorPosition;
        public System.Drawing.Point CursorPosition
        {
            get { return _cursorPosition; }
            set
            {
                _cursorPosition = value;
                if (NewCursorPosition != null)
                {
                    NewCursorPosition(this, null);
                }
            }
        }

        private List<Annotation> _annotations = new List<Annotation>();
        private IFits _imageData = null;

        public int Width
        {
            get {
                if (_imageData == null) return 0;
                    return _imageData.Width(); 
            }
        }

        public int Height
        {
            get
            {
                if (_imageData == null) return 0;
                return _imageData.Height();
            }
        }
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
        public event EventHandler NewCursorPosition;
        public event EventHandler NewCursorClickPosition;

        #endregion

        #region Ctor

        public StroblhofwarteImage()
        {
            CursorPosition = new System.Drawing.Point(0, 0);
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

        public Bitmap GetSubimage(System.Drawing.Point center, int width, int height)
        {
            ushort[] fitsData = _imageData.GetRawImage().GetSubimage(center, width, height);
            try
            {
                int max = 0;
                int min = int.MaxValue;
                foreach (ushort v in fitsData)
                {
                    if (v > max) max = v;
                    if (v < min) min = v;
                }
                double f = 8192.0 / (double)(max - min);
                Bitmap BitmapData = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
                var rawData = BitmapData.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, BitmapData.PixelFormat);
                int ptr = 0;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        short pix = (short)((double)(fitsData[ptr] - min) * f);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr * 6, pix);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr * 6 + 2, pix);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr * 6 + 4, pix);
                        ptr++;
                    }
                BitmapData.UnlockBits(rawData);
                return BitmapData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ushort[] GetSubimagRaw(System.Drawing.Point center, int width, int height)
        {
            return _imageData.GetRawImage().GetSubimage(center, width, height);
        }

        public int XYtoPtr(int x, int y)
        {
            return _imageData.GetRawImage().DataPtr(x, y);
        }

        public double GetExposureTime()
        {
            return _imageData.GetExposureTime();
        }
    }

    public class Annotation
    {
        public string Name { get; set;}
        public Coordinates Coor { get; set; }

    }
}