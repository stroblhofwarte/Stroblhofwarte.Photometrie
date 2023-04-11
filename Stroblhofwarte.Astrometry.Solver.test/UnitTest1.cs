using Stroblhofwarte.Astrometry.Solver.AstrometryNet;
using Stroblhofwarte.Astrometry.Solver.Interface;

namespace Stroblhofwarte.Astrometry.Solver.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            IAstrometrySolver solver = new AstrometryNet.AstrometryNet("http://nova.astrometry.net/api/", "vnxyefaoypruzwuj");
            solver.Solve("Question Mark Galaxy(1)_2021-05-30_23-09-02_GAIN_0_R_-20.00_120.00s_0001.fits");
        }
    }
}