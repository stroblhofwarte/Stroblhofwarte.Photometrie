using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;
using System.Globalization;

namespace Stroblhofwarte.FITS.DataObjects
{
    public class FitsImage
    {
        #region Properties

        private ushort[] _rawImageData;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BitPix { get; private set; }
        public Bitmap BitmapData { get; private set; }
        public WorldCoordinateSystem WCS { get; private set; }
        public bool WCSValid { get; private set; }

        public string DATE_LOC
        {
            get
            {
                return _header.GetStringValue("DATE-LOC");
            }
        }


        private bool _isValid = false;
        private string _error = String.Empty;
        private FITSHeader _header;

        #endregion
        #region Ctor

        public FitsImage(int width, int height, int bitpix, FITSHeader header, ushort[] data)
        {
            Width = width;
            Height = height;
            BitPix = bitpix;
            _header = header;
            _rawImageData = data;
            WCSValid = false;
            bool flipped = false;
            if (header.CheckForWCS())
            {
                // WCS found
                WCS = header.WCS;
                WCSValid = header.WCSValid;
                flipped = WCS.Flipped;
            }

            // Could be a request to load only the header data:
            if (data == null) return;

            BitmapData = new Bitmap(width, height, PixelFormat.Format48bppRgb);
           
            int max = 0;
            int min = int.MaxValue;
            foreach(ushort v in data)
            {
                if (v > max) max = v;
                if (v < min) min = v;
            }
            double f = 8192.0/(double)(max - min);
            try
            {
                var rawData = BitmapData.LockBits(new Rectangle(0, 0, BitmapData.Width, BitmapData.Height),ImageLockMode.ReadWrite, BitmapData.PixelFormat);
                int ptr = 0;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        short pix = (short)((double)(data[ptr] - min) * f);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr*6, pix);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr*6 + 2 , pix);
                        System.Runtime.InteropServices.Marshal.WriteInt16(rawData.Scan0, ptr*6 + 4 , pix);
                        ptr++;
                    }
                BitmapData.UnlockBits(rawData);
                _isValid = true;
            } catch (Exception ex)
            {
                _error = ex.ToString();
            }
        }

        #endregion

        public ushort[] GetSubimage(Point center, int width, int hight)
        {
            ushort[] subimage = new ushort[width*hight];
            if (width > Width / 2) throw new Exception("Subimage: Width to big for the source image!");
            if (hight > Height / 2) throw new Exception("Subimage: Hight to big for the source image!");

            // Check if the center point is near the border and correct the center:
            if (center.X + width / 2 > Width) center.X = Width - (width / 2);
            if (center.X - width / 2 < 0) center.X = width / 2;
            if (center.Y + hight / 2 > Height) center.Y = Height - (hight / 2);
            if (center.Y - hight / 2 < 0) center.Y = hight / 2;

            int ptr = 0;
            for(int y = center.Y - hight / 2; y < center.Y + hight / 2; y++)
                for (int x = center.X - width / 2; x < center.X + width / 2; x++)
                {
                    subimage[ptr++] = _rawImageData[DataPtr(x, y)];
                }
            return subimage;
        }

        public Int32 DataPtr(int x, int y)
        {
            return y * Width + x;
        }

        public double GetExposureTime()
        {
            foreach(FITSHeaderCard card in _header.HeaderCards)
            {
                if(card.Key == "EXPOSURE")
                {
                    try
                    {
                        return Convert.ToDouble(card.Value, CultureInfo.InvariantCulture);
                    }
                    catch(Exception)
                    {
                        return 0.0;
                    }
                }
                if (card.Key == "EXPTIME")
                {
                    try
                    {
                        return Convert.ToDouble(card.Value, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        return 0.0;
                    }
                }
            }
            return 0.0;
        }
    }
}
