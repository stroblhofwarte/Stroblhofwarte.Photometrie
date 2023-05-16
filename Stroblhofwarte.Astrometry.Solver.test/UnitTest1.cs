#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

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
            IAstrometrySolver solver = new AstrometryNet.AstrometryNet("192.168.42.90/api/", "");
            solver.Solve("Question Mark Galaxy(1)_2021-05-30_23-09-02_GAIN_0_R_-20.00_120.00s_0001.fits");
        }
    }
}