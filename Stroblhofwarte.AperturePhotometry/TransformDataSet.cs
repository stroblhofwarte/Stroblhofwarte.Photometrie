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
