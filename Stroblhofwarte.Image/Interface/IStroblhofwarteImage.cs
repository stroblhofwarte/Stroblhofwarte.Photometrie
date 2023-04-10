using Stroblhofwarte.FITS.DataObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Stroblhofwarte.Image.Interface
{
    public interface IStroblhofwarteImage
    {
        public bool IsValid { get; }
        public WorldCoordinateSystem WCS { get; }
        #region Events

        event EventHandler NewImageLoaded;

        #endregion

        public Bitmap GetBitmap();
        
    }
}
