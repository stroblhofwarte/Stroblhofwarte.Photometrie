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
    public class MagMeasurement
    {

        #region Singelton

        private static MagMeasurement _instance;

        public static MagMeasurement Instance
        {
            get
            {
                if(_instance == null )
                    _instance = new MagMeasurement();
                return _instance;
            }
        }

        #endregion

        #region Events

        public event EventHandler NewMeasurement;

        #endregion

        #region Properties

        private double _mag;
        public double Mag
        {
            get { return _mag; }
        }

        private bool _isMachine;
        public bool IsMachine
        {
            get { return _isMachine; }
        }
        public bool CalibrationMode { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        #endregion

        #region Ctor
        public MagMeasurement()
        {

        }

        #endregion

        public void Update(double mag, int x, int y, bool isMachine)
        {
            _mag = mag;
            _isMachine = isMachine;
            X = x;
            Y = y;
            if (NewMeasurement != null)
            {
                MeasurementEventArgs args = new MeasurementEventArgs() { Mag = _mag, IsMachine = _isMachine, X = x, Y = y };
                NewMeasurement(this, args);
            }
        }
    }

    public class MeasurementEventArgs : EventArgs
    {
        public double Mag { get; set; }
        public bool IsMachine { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}

