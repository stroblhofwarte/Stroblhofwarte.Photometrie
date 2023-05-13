#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Stroblhofwarte.VSPAPI.Data
{
    public class Mag
    {
        #region Properties

        public string Band { get; private set; }
        public double Magnitude { get; private set; }
        public double Error { get; private set; }

        #endregion

        #region Ctor

        public Mag(string band, string mag, string error)
        {
            Band = band;
            try
            {
                mag = mag.Replace(",", ".");
                Magnitude = Convert.ToDouble(mag, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Magnitude = double.NaN;
            }
            try
            {
                error = error.Replace(",", ".");
                Error = Convert.ToDouble(error, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Error = double.NaN;
            }
        }

        #endregion

        public override string ToString()
        {
            return Band + ", " + Magnitude.ToString();

        }
    }
}
