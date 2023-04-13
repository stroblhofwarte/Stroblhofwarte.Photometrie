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
