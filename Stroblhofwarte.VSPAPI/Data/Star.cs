using Stroblhofwarte.FITS.DataObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stroblhofwarte.VSPAPI.Data
{
    public class Star
    {
        #region Properties

        public Coordinates StarCoordinates { get; private set; }
        public string Name { get; private set; }
        public double Fov { get; private set; }
        public string Auid { get; private set; }
        public string Comment { get; set; }
        public double Maglimit { get; private set; }

        public List<Mag> Magnitudes { get; private set; }

        public bool IsReference
        {
            get
            {
                if (Magnitudes.Count == 0) return false;
                return true;
            }
        }

        #endregion

        #region Ctor

        public Star()
        {
            Magnitudes = new List<Mag>();
            StarCoordinates = new Coordinates(0, 0, Epoch.B1950, Coordinates.RAType.Hours);
            Name = string.Empty;
            Auid = string.Empty;
            Fov = double.NaN;
            Maglimit = double.NaN;
        }
        public Star(string ra, string dec, string name, string auid, string fov, string maglimit)
        {
            Magnitudes = new List<Mag>();
            double dRa = AstroUtil.DMSToDegrees(ra);
            double dDec = AstroUtil.DMSToDegrees(dec);

            StarCoordinates = new Coordinates(dRa, dDec, Epoch.J2000, Coordinates.RAType.Hours);
            Name = name;
            Auid = auid;
            try
            {
                fov = fov.Replace(",", ".");
                Fov = Convert.ToDouble(fov, CultureInfo.InvariantCulture);
            } catch (Exception ex)
            {
                Fov = double.NaN;
            }
            try
            {
                maglimit = maglimit.Replace(",", ".");
                Maglimit = Convert.ToDouble(maglimit, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Maglimit = double.NaN;
            }
        }

        #endregion

        public void AddMagnitude(Mag mag)
        {
            Magnitudes.Add(mag);
        }

        public override string ToString()
        {
            return Name + ", " + StarCoordinates.ToString() + ", " + Magnitudes.Count() + " bands";

        }
    }
}
