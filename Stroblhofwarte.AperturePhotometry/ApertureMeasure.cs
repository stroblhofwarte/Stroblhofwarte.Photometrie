#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.Globalization;
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
            _searchRadius = (int)outerAnnulusR;

            double apertureInnerR = apertureR;
            double apertureOuterR = apertureR + 2;

            // Get orginal data array:
            // This function will throw an exception when the subimage is outside the image.
            ushort[] pix = StroblhofwarteImage.Instance.GetSubimagRaw(new Point(userx, usery), (_searchRadius * 2) + 4, (_searchRadius * 2) + 4);

            // Check if the aperture is inside the image
            int First = 2;
            int Last = (_searchRadius * 2) + 2;

            int middle = _searchRadius + 2;
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

            ScratchPad.ScratchPad.Instance.Add(@"R_{Aperture}:=" + apertureR.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add(@"R_{inner Annulus}:=" + innerAnulusR.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add(@"R_{outer Annulus}:=" + outerAnnulusR.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add("Star_{ADU}:=" + result.StarADU.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add("Sky_{ADU}:=" + result.SkyADU.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add("Star_{pixel}:=" + result.StarPixels.ToString("0.####", CultureInfo.InvariantCulture));
            ScratchPad.ScratchPad.Instance.Add("Sky_{pixel}:=" + result.SkyPixels.ToString("0.####", CultureInfo.InvariantCulture));


            return result;
        }

        public double Magnitude(MeasurementResult meas, double Z)
        {
            if (meas == null) return -999.0;
            if (meas.ExposureTime <= 0.0) return -999.0;
            double StarADU = meas.StarPixels * ((meas.StarADU / meas.StarPixels) - (meas.SkyADU / meas.SkyPixels));
            meas.ADU = StarADU;
            double SoftApertureMagnitude = Z - 2.5 * Math.Log10(StarADU / meas.ExposureTime);
            return SoftApertureMagnitude;
        }

        public double Uncertenty(MeasurementResult meas)
        {
            ScratchPad.ScratchPad.Instance.Add("Uncertainty (CCD  Equation):");
            double SoftApertureSigma = 0.0;
            if (meas == null) return 999.0;
            InstrumentObject instr = Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
                (int)StroblhofwarteImage.Instance.GetSensorGain(),
                (int)StroblhofwarteImage.Instance.GetSensorOffset(),
                StroblhofwarteImage.Instance.GetBinning(),
                StroblhofwarteImage.Instance.GetSensorSetTemp());
            if (instr == null) return 999.0;
            double Nsky = instr.Gain_e_ADU * (meas.SkyADU / meas.SkyPixels);
            double Ndark = instr.DarkCurrent;
            double Nron2 = instr.ReadOutNoise * instr.ReadOutNoise;
            double PixRatio = 1 + (meas.StarPixels / meas.SkyPixels);
            double Gsig2 = (0.289 * instr.Gain_e_ADU) * (0.289 * instr.Gain_e_ADU);

            ScratchPad.ScratchPad.Instance.Add(@"N_{sky}:=" + Nsky.ToString("0.######", CultureInfo.InvariantCulture) + " e^{-}");
            ScratchPad.ScratchPad.Instance.Add(@"N_{dark}:=" + Ndark.ToString("0.######", CultureInfo.InvariantCulture) + " e^{-}/s/pix");
            ScratchPad.ScratchPad.Instance.Add(@"N_{readout}:=" + instr.ReadOutNoise.ToString("0.######", CultureInfo.InvariantCulture) + " e^{-} rms");
            ScratchPad.ScratchPad.Instance.Add(@"\sigma:= (0.289 \cdot Gain) = " + (0.289 * instr.Gain_e_ADU).ToString("0.######", CultureInfo.InvariantCulture) + " e^{-} ADU");

            double NStarMinusSkyElectrons = instr.Gain_e_ADU * (meas.StarPixels * ((meas.StarADU / meas.StarPixels) - (meas.SkyADU / meas.SkyPixels)));
            ScratchPad.ScratchPad.Instance.Add(@"Star = Gain ( Star_{pixl} \left( \frac{Star_{ADU}}{Star_{pixl}} - \frac{Sky_{ADU}}{Sky_{pixl}} \right)) = " + NStarMinusSkyElectrons.ToString("0.######", CultureInfo.InvariantCulture) + " e^{-}");

            double NoiseRMSElectrons = Math.Sqrt(NStarMinusSkyElectrons + meas.StarPixels * PixRatio * (Nsky + Ndark + Nron2 + Gsig2));
            ScratchPad.ScratchPad.Instance.Add(@"Nois_{RMS e^{-}} = \sqrt{Star + Star_{pixl} \cdot (1+\frac{Star_{pixl}}{Sky_{pixl}}) \cdot (N_{sky} + N_{dark} + N_{readout}^2 + \sigma)} = " + NoiseRMSElectrons.ToString("0.######", CultureInfo.InvariantCulture) + " e^{-}");

            if (NoiseRMSElectrons > 0)
            {
                double Snr = NStarMinusSkyElectrons / NoiseRMSElectrons;
                ScratchPad.ScratchPad.Instance.Add(@"SNR:=\frac{" + NStarMinusSkyElectrons.ToString("0.######", CultureInfo.InvariantCulture) + "}{" + NoiseRMSElectrons.ToString("0.######", CultureInfo.InvariantCulture) + "} = " + Snr.ToString("0.######", CultureInfo.InvariantCulture));
                if(Snr > 1.11)
                {
                    SoftApertureSigma = 2.5 * Math.Log10(1 / (1 - 1 / Snr)) / 2.302585;
                    ScratchPad.ScratchPad.Instance.Add(@"\varsigma:=\frac{2.5 Log_{10}(\frac{1}{1 - \frac{1}{SNR}})}{2.302585} = " + SoftApertureSigma.ToString("0.######", CultureInfo.InvariantCulture));
                }
            }
            return SoftApertureSigma;
        }

        private int DataPtr(int x, int y)
        {
            return (y * ((_searchRadius * 2) + 4)) + x;
        }

        public double GetZ(MeasurementResult machineMag, double referenceMag)
        {
            double Zmax = 100;
            double Zmin = 0.0;

            double goal = (double)referenceMag;

            while (true)
            {
                double Z = ((Zmax - Zmin) / 2.0) + Zmin;
                double mag = Magnitude(machineMag, Z);
                if (double.IsNaN(mag)) return -999.0;
                if (mag == -999.0) return 0.0;
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
        public double ADU { get; set; }
    }
}
