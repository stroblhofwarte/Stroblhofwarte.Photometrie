using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 
 * Aperture Photometry algorithm from
 *  Richard Berry
 * 
 * 
 */

namespace Stroblhofwarte.AperturePhotometry
{
    public class Centroid
    {
        #region Properties

        private double _thresholdFactor = 1.01;

        private int _searchRadius;
        public int SearchRadius
        {
            get { return _searchRadius; }
            set { _searchRadius = value; }
        }

        #endregion

        #region Ctor

        public Centroid(int searchRadius)
        {
            _searchRadius = searchRadius;
        }

        #endregion

        public Point GetCentroid(int userx, int usery)
        {
            int width = StroblhofwarteImage.Instance.Width;
            int height = StroblhofwarteImage.Instance.Height;
            // Get orginal data array:
            // This function will throw an exception when the subimage is outside the image.
            ushort[] pix = StroblhofwarteImage.Instance.GetSubimagRaw(new Point(userx, usery), (_searchRadius * 2) + 4, (_searchRadius * 2) + 4);

            // Check if the aperture is inside the image
            int First = 2;
            int Last = (_searchRadius * 2) + 2;

            int middle = _searchRadius + 2;
            // Get the average and threshold values

            int ApertureRadiusSquared = (_searchRadius * _searchRadius);
            int StarSum = 0;
            int starpixels = 0;

            for (int x = First; x <= Last; x++)
                for (int y = First; y <= Last; y++)
                {
                    int Xradius = x - middle;
                    int Yradius = y - middle;
                    int CurrentRadiusSquared = (Xradius * Xradius) + (Yradius * Yradius);
                    if (CurrentRadiusSquared <= ApertureRadiusSquared)
                    {
                        StarSum = StarSum + pix[DataPtr(x, y)];
                        starpixels = starpixels + 1;
                    }
                }
            double Background = (StarSum / starpixels);
            double threshold = (_thresholdFactor * Background);

            // Check if a pixel is above threshold and contains to a cluster of pixels:
            int XMoment = 0;
            int YMoment = 0;
            int Xsum = 0;
            int Ysum = 0;

            for (int y = First; y <= Last; y++)
                for (int x = First; x <= Last; x++)
                {
                    int Xradius = x - middle;
                    int Yradius = y - middle;
                    int CurrentRadiusSquared = (Xradius * Xradius) + (Yradius * Yradius);
                    if (CurrentRadiusSquared <= ApertureRadiusSquared)
                    {
                        int Neighbors = 0;
                        if ((double)pix[DataPtr(x, y - 1)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x, y + 1)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x - 1, y - 1)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x - 1, y)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x - 1, y + 1)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x + 1, y - 1)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x + 1, y)] > threshold) Neighbors++;
                        if ((double)pix[DataPtr(x + 1, y + 1)] > threshold) Neighbors++;
                        if (Neighbors > 3)
                        {
                            double Foreground = ((double)pix[DataPtr(x, y)] - Background);
                            XMoment = XMoment + (int)Foreground * x;
                            Xsum = Xsum + (int)Foreground;
                            YMoment = YMoment + (int)Foreground * y;
                            Ysum = Ysum + (int)Foreground;
                        }
                    }
                }
            if (Xsum > 0 && Ysum > 0)
            {
                double resultX = (double)((double)XMoment / (double)Xsum);
                double resultY = (double)((double)YMoment / (double)Ysum);
                
                Point p = new Point((userx - (_searchRadius + 2)) + (int)resultX, (usery - (_searchRadius +2)) + (int)resultY);
                return p;
            }
            throw new Exception("No valid centroid position found");
        }

        private int DataPtr(int x, int y)
        {
            return (y * ((_searchRadius * 2) + 4)) + x;
        }
    }
}
