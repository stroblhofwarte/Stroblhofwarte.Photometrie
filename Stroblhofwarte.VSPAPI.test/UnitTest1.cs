using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.VSPAPI.Data;

namespace Stroblhofwarte.VSPAPI.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void VariableStarObjectTest()
        {
            VariableStar var = new VariableStar(new Star("21:42:42.80", "43:35:09.9", "SS Cyg", "000-BCP-220", "60.0", "14.5"));

            Mag mag = new Mag("V", "5.11", "0.0");
            Star refStar = new Star("21:40:11.11", "43:16:25.8", "51", "000-BCP-115", "", "");
            refStar.AddMagnitude(mag);
            var.AddReferenceStar(refStar);
        }

        [TestMethod]
        public void GetStarFromAAVSO()
        {
            Task.Run(async () =>
            {
                StroblhofwarteVSPAPI api = new StroblhofwarteVSPAPI();
                Task<VariableStar> task = api.GetAAVSOData("TV Boo", "60.0", "14.5");
                VariableStar star = task.Result;
            }).GetAwaiter().GetResult();
        }
    }
}

