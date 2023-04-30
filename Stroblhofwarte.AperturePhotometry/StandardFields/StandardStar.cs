using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.AperturePhotometry.StandardFields
{
    public class StandardStar
    {
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
