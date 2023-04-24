using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.VSPAPI.Data
{
    public class VariableStar
    {
        #region Properties

        public string ChartId { get; set; }
        public Star Var { get; private set; }
        public List<Star> ReferenceStars { get; private set; }

        #endregion

        #region Ctor

        public VariableStar(Star var)
        {
            Var = var;
            ReferenceStars = new List<Star>();
        }

        public VariableStar()
        {
            Var = new Star();
            ReferenceStars = new List<Star>();
        }

        #endregion

        public void AddReferenceStar(Star star)
        {
            ReferenceStars.Add(star);
        }

        public override string ToString()
        {
            return Var.Name + ", " + Var.StarCoordinates.ToString() + ", " + ReferenceStars.Count() + " refStars"; ;

        }
    }
}
