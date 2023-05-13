#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

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
        Stroblhofwarte.FITS.DataObjects.FitsImage GetRawImage();
        double GetExposureTime();
        string GetInstrument();
        string GetTelescope();
        double GetFocalLength();
        double GetFocalRatio();
        double GetSensorSetTemp();
        double GetSensorTemp();
        double GetSensorGain();
        double GetSensorOffset();
        string GetFilter();
        string GetObject();
        string GetBinningId();
        double GetAirmass();

        DateTime GetObservationTimeUTC();
        DateTime GetObservationTime();
    }
}
