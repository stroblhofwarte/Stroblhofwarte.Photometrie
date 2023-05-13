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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.AperturePhotometry.StandardFields
{
    public class StandardStar
    {
        public string AUID { get; set; }
        public string Id { get; set; }
        public double Ra { get; set; }
        public double DEC { get; set; }
        public double V { get; set; }
        public double BV { get; set; }
        public double UB { get; set; }
        public double VR { get; set; }
        public double RI { get; set; }
        public double Verr { get; set; }
        public double BVerr { get; set; }
        public double UBerr { get; set; }
        public double VRerr { get; set; }
        public double RIerr { get; set; }
        public double X { get; set; }
        public double Y { get; set; }


    }
}
