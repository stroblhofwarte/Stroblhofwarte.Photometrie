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
        private int _width;
        private int _height;
        private int _bitPix;
        private Bitmap _bitmap;
        private bool _isValid = false;
        private string _error = String.Empty;
        private FITSHeader _header;

        #endregion
        #region Ctor

        public FitsImage(int width, int height, int bitpix, FITSHeader header, ushort[] data)
        {
            _width = width;
            _height = height;
            _bitPix = bitpix;
            _header = header;
            _rawImageData = data;
            _bitmap = new Bitmap(width, height, PixelFormat.Format48bppRgb);
           
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
                var rawData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),ImageLockMode.ReadWrite, _bitmap.PixelFormat);
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
                _bitmap.UnlockBits(rawData);
                _isValid = true;
                _bitmap.Save("filename.jpg", ImageFormat.Jpeg);
            } catch (Exception ex)
            {
                _error = ex.ToString();
            }
        }

        #endregion
    }
}
