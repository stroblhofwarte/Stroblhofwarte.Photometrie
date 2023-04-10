using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Drawing.Imaging;

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
            if (header.CheckForWCS())
            {
                // WCS found
                WCS = header.WCS;
                WCSValid = header.WCSValid;
            }

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
                BitmapData.Save("filename.jpg", ImageFormat.Jpeg);
            } catch (Exception ex)
            {
                _error = ex.ToString();
            }
        }

        #endregion
    }
}
