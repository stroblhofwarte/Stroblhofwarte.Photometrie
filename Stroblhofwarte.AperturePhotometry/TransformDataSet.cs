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

namespace Stroblhofwarte.AperturePhotometry
{
    public class TransformDataSet
    {
        #region Singelton

        private static TransformDataSet _instance;

        public static TransformDataSet Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TransformDataSet();
                return _instance;
            }
        }

        #endregion

        #region Events

        public event EventHandler NewMeasurement;

        #endregion

        #region Properties
        public string Id { get; set; }
        public double Mag { get; set; }
        public double V { get; set; }
        public double BV { get; set; }
        public double UB { get; set; }
        public double VR { get; set; }
        public double RI { get; set; }

        #endregion

        public void Update(string id, double mag, double v, double bv, double ub, double vr, double ri)
        {
            Id = id;
            Mag = mag;
            V = v;
            BV = bv;
            UB = ub;
            VR = vr;
            RI = ri;
            if(NewMeasurement != null)
            {
                NewMeasurement(this, null);
            }
        }
    }
}
