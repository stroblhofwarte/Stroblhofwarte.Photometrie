using Stroblhofwarte.FITS.DataObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.FITS.Interface
{
    public interface IFits
    {
        bool IsValid { get; }
        string ErrorText { get; }
        WorldCoordinateSystem WCS { get; }

        Bitmap GetImage();
        int Width();
        int Height();

    }
}
