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
            FITS.LoadInternal(uri, true);
        }
    }
}