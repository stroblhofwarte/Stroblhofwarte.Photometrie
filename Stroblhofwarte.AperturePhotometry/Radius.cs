#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.AperturePhotometry
{
    public class Radius
    {
        #region Properties

        private double _annulusCut = 0.8; // We want to have 80% of the light

        private int _searchRadius;

        #endregion

        #region Ctor
        #endregion
        public void CalculateRadius(int userx, int usery, int searchRadius, out double apertureR, out double outerAnnulusR, out double innerAnulusR)
        {
            Dictionary<double, double> machineMags = new Dictionary<double, double>();
            apertureR = 0.0;
            outerAnnulusR = 0.0;
            innerAnulusR = 0.0;
            ApertureMeasure measure = new ApertureMeasure();
            _searchRadius = searchRadius;
            double startRadius = 4;
            for(double r = startRadius; r < (double)searchRadius; r=r+1.0)
            {
                double innerR = 0.0;
                double outerR = 0.0;
                CalculateCircles(r, out innerR, out outerR);
                if (outerR >= searchRadius)
                    break;
                MeasurementResult result = measure.Measure(userx, usery, searchRadius, r, innerR, outerR);
                double mag = result.StarADU - result.SkyADU;
                machineMags.Add(r, mag);
            }
            double max = 0;
            double maxR = 0;
            foreach(KeyValuePair<double, double> pair in machineMags)
            {
                if(pair.Value > max)
                {
                    max = pair.Value;
                    maxR = pair.Key;
                }
            }
            apertureR = maxR;
            CalculateCircles(apertureR, out outerAnnulusR, out innerAnulusR);
        }

      

        private int DataPtr(int x, int y)
        {
            return (y * ((_searchRadius * 2) + 4)) + x;
        }

        public void CalculateCircles(double apertureR, out double innerAnnulusR, out double outerAnnulusR)
        {
            innerAnnulusR = apertureR + apertureR / 2;
            outerAnnulusR = innerAnnulusR + apertureR;
        }
    }
}
