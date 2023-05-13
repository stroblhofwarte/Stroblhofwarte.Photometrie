#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"


namespace Stroblhofwarte.FITS.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            Uri uri = new Uri(strWorkPath + "\\Ring Nebula(2)_2021-05-31_22-41-52_GAIN_0_G_-20.00_600.00s_0001.fits");
            FITS.Load(uri, true);
        }
    }
}