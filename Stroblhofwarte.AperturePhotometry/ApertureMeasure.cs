using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.AperturePhotometry
{
    public  class ApertureMeasure
    {
        #region Properties

        private int _searchRadius;

        #endregion

        #region Ctor

        #endregion

        public MeasurementResult Measure(int userx, int usery, int searchRadius, double apertureR, double innerAnulusR, double outerAnnulusR)
        {
            _searchRadius = searchRadius;

            double apertureInnerR = apertureR;
            double apertureOuterR = apertureR + 2;

            // Get orginal data array:
            // This function will throw an exception when the subimage is outside the image.
            ushort[] pix = StroblhofwarteImage.Instance.GetSubimagRaw(new Point(userx, usery), (searchRadius * 2) + 4, (searchRadius * 2) + 4);

            // Check if the aperture is inside the image
            int First = 2;
            int Last = (searchRadius * 2) + 2;

            int middle = searchRadius + 2;
            // Get the average and threshold values

            double apertureInnerRsqr = (apertureInnerR * apertureInnerR);
            double apertureOuterRsqr = (apertureOuterR * apertureOuterR);
            double innerAnulusRsqr = (innerAnulusR * innerAnulusR);
            double outerAnnulusRsqr = (outerAnnulusR * outerAnnulusR);

            double apertureSum = 0.0;
            double annulusSum = 0.0;
            double weight = 0.0;
            double starPixels = 0.0;
            double skyPixels = 0.0;
            List<ushort> annulusValues = new List<ushort>();

            for (int x = First; x <= Last; x++)
                for (int y = First; y <= Last; y++)
                {
                    int Xradius = x - middle;
                    int Yradius = y - middle;
                    int CurrentRadiusSquared = (Xradius * Xradius) + (Yradius * Yradius);
                    if (CurrentRadiusSquared < apertureOuterRsqr)
                    {
                        weight = 0.5;
                        if (CurrentRadiusSquared < apertureInnerRsqr)
                        {
                            weight = 1.0;
                        }
                        apertureSum = apertureSum + (pix[DataPtr(x, y)] * weight);
                        starPixels = starPixels + weight;
                    }
                    if (CurrentRadiusSquared >= innerAnulusRsqr && CurrentRadiusSquared <= outerAnnulusRsqr)
                    {
                        annulusValues.Add(pix[DataPtr(x, y)]);
                    }
                }
            // Throw away the 20% of brightest pixels of the annulus values:
            annulusValues.Sort();
            double cutEdge = annulusValues.Count * 0.8;
            for (int a = 0; a < annulusValues.Count; a++)
            {
                if (a < cutEdge)
                {
                    annulusSum = annulusSum + annulusValues[a];
                    skyPixels = skyPixels + 1.0;
                }
            }
            MeasurementResult result = new MeasurementResult() { StarADU = apertureSum, SkyADU = annulusSum, StarPixels = starPixels, SkyPixels = skyPixels };
            result.ExposureTime = StroblhofwarteImage.Instance.GetExposureTime();

            return result;
        }

        public double Magnitude(MeasurementResult meas, double Z)
        {
            if (meas.ExposureTime <= 0.0) return -999.0;
            double StarADU = meas.StarPixels * ((meas.StarADU / meas.StarPixels) - (meas.SkyADU / meas.SkyPixels));
            double SoftApertureMagnitude = Z - 2.5 * Math.Log10(StarADU / meas.ExposureTime);
            return SoftApertureMagnitude;
        }

        private int DataPtr(int x, int y)
        {
            return (y * ((_searchRadius * 2) + 4)) + x;
        }

        public double GetZ(MeasurementResult machineMag, int referenceMag /* mag*10 */)
        {
            double Zmax = 100;
            double Zmin = 0.0;

            double goal = (double)referenceMag / 10.0;

            while (true)
            {
                double Z = ((Zmax - Zmin) / 2.0) + Zmin;
                double mag = Magnitude(machineMag, Z);
                double diff = mag - goal;
                if(Math.Abs(diff) < 0.01)
                {
                    return Z;
                }
                if (diff > 0)
                    Zmax = Z;
                if (diff < 0)
                    Zmin = Z;
            }
        }
    }

    public class MeasurementResult
    {
        public double StarADU { get; set; }
        public double SkyADU { get; set; }
        public double StarPixels { get; set; }
        public double SkyPixels { get; set; }
        public double ExposureTime { get; set; }
    }
}
