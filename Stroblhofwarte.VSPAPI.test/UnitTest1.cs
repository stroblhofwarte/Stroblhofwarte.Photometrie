#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

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

