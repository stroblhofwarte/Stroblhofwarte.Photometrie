#region "copyright"

/*
    Copyright © 2016 - 2023 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

#endregion "copyright"

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Utility;

namespace Stroblhofwarte.FITS
{

    public class FITSHeader {

        public FITSHeader(int width, int height) {
            Add("SIMPLE", true, "C# FITS");
            Add("BITPIX", 16, "");
            Add("NAXIS", 2, "Dimensionality");
            Add("NAXIS1", width, "");
            Add("NAXIS2", height, "");
            Add("BZERO", 32768, "");
            Add("EXTEND", true, "Extensions are permitted");
            WCSValid = false;
        }

        private Dictionary<string, FITSHeaderCard> _headerCards = new Dictionary<string, FITSHeaderCard>();

        public ICollection<FITSHeaderCard> HeaderCards => _headerCards.Values;

        public WorldCoordinateSystem WCS { get; private set; }
        public bool WCSValid { get; private set; }
        public void Add(string keyword, string value, string comment) {
            if (!_headerCards.ContainsKey(keyword)) {
                _headerCards.Add(keyword, new FITSHeaderCard(keyword, value, comment));
            }
        }

        public void Add(string keyword, int value, string comment) {
            if (!_headerCards.ContainsKey(keyword)) {
                _headerCards.Add(keyword, new FITSHeaderCard(keyword, value, comment));
            }
        }

        public void Add(string keyword, double value, string comment) {
            if (!_headerCards.ContainsKey(keyword)) {
                _headerCards.Add(keyword, new FITSHeaderCard(keyword, value, comment));
            }
        }

        public void Add(string keyword, bool value, string comment) {
            if (!_headerCards.ContainsKey(keyword)) {
                _headerCards.Add(keyword, new FITSHeaderCard(keyword, value, comment));
            }
        }

        public void Add(string keyword, DateTime value, string comment) {
            if (!_headerCards.ContainsKey(keyword)) {
                _headerCards.Add(keyword, new FITSHeaderCard(keyword, value, comment));
            }
        }

        public void Write(Stream s) {
            /* Write header */
            foreach (var card in this.HeaderCards) {
                var b = card.Encode();
                s.Write(b, 0, FITS.HEADERCARDSIZE);
            }

            /* Close header http://archive.stsci.edu/fits/fits_standard/node64.html#SECTION001221000000000000000 */
            s.Write(Encoding.ASCII.GetBytes("END".PadRight(FITS.HEADERCARDSIZE)), 0, FITS.HEADERCARDSIZE);

            /* Write blank lines for the rest of the header block */
            for (var i = this.HeaderCards.Count + 1; i % (FITS.BLOCKSIZE / FITS.HEADERCARDSIZE) != 0; i++) {
                s.Write(Encoding.ASCII.GetBytes("".PadRight(FITS.HEADERCARDSIZE)), 0, FITS.HEADERCARDSIZE);
            }
        }

        public bool CheckForWCS()
        {
            WCSValid = false;
            double crval1 = GetDoubleValue("CRVAL1");
            double crval2 = GetDoubleValue("CRVAL2");
            double crpix1 = GetDoubleValue("CRPIX1");
            double crpix2 = GetDoubleValue("CRPIX2");
            double cd1_1 = GetDoubleValue("CD1_1");
            double cd1_2 = GetDoubleValue("CD1_2");
            double cd2_1 = GetDoubleValue("CD2_1");
            double cd2_2 = GetDoubleValue("CD2_2");
            double cdelta1 = GetDoubleValue("CDELT1");
            double cdelta2 = GetDoubleValue("CDELT2");
            double crota2 = GetDoubleValue("CROTA2");
           
            if (!double.IsNaN(crval1) &&
               !double.IsNaN(crval2) &&
               !double.IsNaN(crpix1) &&
               !double.IsNaN(crpix2) &&
               !double.IsNaN(cdelta1) &&
               !double.IsNaN(cdelta2) &&
               !double.IsNaN(crota2))
            {
                // WCD foud. Create WorldCoordinateSystem object
                WCS = new WorldCoordinateSystem(crval1, crval2, crpix1, crpix2, cdelta1, cdelta2, crota2);
                WCSValid = true;
                return true;
            }

            if (!double.IsNaN(crval1) &&
              !double.IsNaN(crval2) &&
              !double.IsNaN(crpix1) &&
              !double.IsNaN(crpix2) &&
              !double.IsNaN(cd1_1) &&
              !double.IsNaN(cd1_2) &&
              !double.IsNaN(cd2_1) &&
              !double.IsNaN(cd2_2))
            {
                // WCD foud. Create WorldCoordinateSystem object
                WCS = new WorldCoordinateSystem(crval1, crval2, crpix1, crpix2, cd1_1, cd1_2, cd2_1, cd2_2);
                WCSValid = true;
                return true;
            }
            return false;
        }
        private double GetDoubleValue(string keyword)
        {
            if (_headerCards.TryGetValue(keyword, out var card))
            {
                return ParseDouble(card.OriginalValue);
            }
            return double.NaN;
        }
        public void ExtractMetaData() {
          
            if (_headerCards.TryGetValue("IMAGETYP", out var card)) {
                string d = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("IMAGETYP", out card)) {
                string d = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("EXPOSURE", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("EXPTIME", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("DATE-LOC", out card)) {
                //metaData.Image.ExposureStart = DateTime.ParseExact($"{card.OriginalValue}", "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            }

            if (_headerCards.TryGetValue("DATE-OBS", out card)) {
                //metaData.Image.ExposureStart = DateTime.ParseExact($"{card.OriginalValue} UTC", "yyyy-MM-ddTHH:mm:ss.fff UTC", CultureInfo.InvariantCulture);
            }

            /* Camera */
            if (_headerCards.TryGetValue("XBINNING", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("YBINNING", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("GAIN", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("OFFSET", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("EGAIN", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("XPIXSZ", out card)) {
                double d = ParseDouble(card.OriginalValue);// / metaData.Camera.BinX;
            }

            if (_headerCards.TryGetValue("INSTRUME", out card)) {
                string d = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("SET-TEMP", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("CCD-TEMP", out card)) {
                double de = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("READOUTM", out card)) {
                string d = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("BAYERPAT", out card)) {
                // FITS Bayer Patterns seem to include single quotes and extra spaces
                var originalValue = card.OriginalValue.Trim('\'').Trim();
                //metaData.Camera.SensorType = metaData.StringToSensorType(originalValue);
            }

            if (_headerCards.TryGetValue("XBAYROFF", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("YBAYROFF", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("USBLIMIT", out card)) {
                int d = ParseInt(card.OriginalValue);
            }

            /* Telescope */
            if (_headerCards.TryGetValue("TELESCOP", out card)) {
                string d = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("FOCALLEN", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("FOCRATIO", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("PIERSIDE", out card)) {
                //metaData.Telescope.SideOfPier = ParsePierSide(card.OriginalValue);
            }

            if (_headerCards.ContainsKey("RA") && _headerCards.ContainsKey("DEC")) {
                string raHdrVal = _headerCards["RA"].OriginalValue;
                string decHdrVal = _headerCards["DEC"].OriginalValue;
                double ra;
                double dec;

                /*if (AstroUtil.IsHMS(raHdrVal)) {
                    ra = AstroUtil.HMSToDegrees(raHdrVal);
                } else {
                    ra = ParseDouble(raHdrVal);
                }

                if (AstroUtil.IsDMS(decHdrVal)) {
                    dec = AstroUtil.DMSToDegrees(decHdrVal);
                } else {
                    dec = ParseDouble(decHdrVal);
                }*/

                //metaData.Telescope.Coordinates = new Coordinates(Angle.ByDegree(ra), Angle.ByDegree(dec), Epoch.J2000);
            }

            if (_headerCards.TryGetValue("CENTALT", out card) || _headerCards.TryGetValue("OBJCTALT", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("CENTAZ", out card) || _headerCards.TryGetValue("OBJCTAZ", out card)) {
                double d = ParseDouble(card.OriginalValue);
            }

            /* Observer */
            if (_headerCards.TryGetValue("SITEELEV", out card)) {
                //metaData.Observer.Elevation = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("SITELAT", out card)) {
                string hdrVal = card.OriginalValue;
                double value;

                /*if (AstroUtil.IsDMS(hdrVal)) {
                    value = AstroUtil.DMSToDegrees(hdrVal);
                } else {
                    value = ParseDouble(hdrVal);
                }

                metaData.Observer.Latitude = value;*/
            }

            if (_headerCards.TryGetValue("SITELONG", out card)) {
                string hdrVal = card.OriginalValue;
                double value;

                /*if (AstroUtil.IsDMS(hdrVal)) {
                    value = AstroUtil.DMSToDegrees(hdrVal);
                } else {
                    value = ParseDouble(hdrVal);
                }

                metaData.Observer.Longitude = value;*/
            }

            /* Filter Wheel */
            if (_headerCards.TryGetValue("FWHEEL", out card)) {
                //metaData.FilterWheel.Name = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("FILTER", out card)) {
                //metaData.FilterWheel.Filter = card.OriginalValue;
            }

            /* Target */
            if (_headerCards.TryGetValue("OBJECT", out card)) {
                //metaData.Target.Name = card.OriginalValue;
            }

            if (_headerCards.ContainsKey("OBJCTRA") && _headerCards.ContainsKey("OBJCTDEC")) {
                string raHdrVal = _headerCards["OBJCTRA"].OriginalValue;
                string decHdrVal = _headerCards["OBJCTDEC"].OriginalValue;
                double ra;
                double dec;

                /*if (AstroUtil.IsHMS(raHdrVal)) {
                    ra = AstroUtil.HMSToDegrees(raHdrVal);
                } else {
                    ra = ParseDouble(raHdrVal);
                }

                if (AstroUtil.IsDMS(decHdrVal)) {
                    dec = AstroUtil.DMSToDegrees(decHdrVal);
                } else {
                    dec = ParseDouble(decHdrVal);
                }

                metaData.Target.Coordinates = new Coordinates(Angle.ByDegree(ra), Angle.ByDegree(dec), Epoch.J2000);*/
            }

            if (_headerCards.TryGetValue("OBJCTROT", out card)) {
                //metaData.Target.PositionAngle = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("AIRMASS", out card)) {
                //metaData.Telescope.Airmass = ParseDouble(card.OriginalValue);
            }

            /* Focuser */
            if (_headerCards.TryGetValue("FOCNAME", out card)) {
                //metaData.Focuser.Name = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("FOCPOS", out card)) {
                //metaData.Focuser.Position = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("FOCUSPOS", out card)) {
                //metaData.Focuser.Position = ParseInt(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("FOCUSSZ", out card)) {
                //metaData.Focuser.StepSize = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("FOCTEMP", out card)) {
                //metaData.Focuser.Temperature = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("FOCUSTEM", out card)) {
                //metaData.Focuser.Temperature = ParseDouble(card.OriginalValue);
            }

            /* Rotator */

            if (_headerCards.TryGetValue("ROTNAME", out card)) {
                //metaData.Rotator.Name = card.OriginalValue;
            }

            if (_headerCards.TryGetValue("ROTATOR", out card)) {
                //metaData.Rotator.MechanicalPosition = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("ROTATANG", out card)) {
                //metaData.Rotator.MechanicalPosition = ParseDouble(card.OriginalValue);
            }

            if (_headerCards.TryGetValue("ROTSTPSZ", out card)) {
                //metaData.Rotator.StepSize = ParseDouble(card.OriginalValue);
            }

            /* Weather Data */

            if (_headerCards.TryGetValue("CLOUDCVR", out card)) {
                //metaData.WeatherData.CloudCover = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("DEWPOINT", out card)) {
                //metaData.WeatherData.DewPoint = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("HUMIDITY", out card)) {
                //metaData.WeatherData.Humidity = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("PRESSURE", out card)) {
                //metaData.WeatherData.Pressure = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("SKYBRGHT", out card)) {
                //metaData.WeatherData.SkyBrightness = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("MPSAS", out card)) {
                //metaData.WeatherData.SkyQuality = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("SKYTEMP", out card)) {
               // metaData.WeatherData.SkyTemperature = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("STARFWHM", out card)) {
                //metaData.WeatherData.StarFWHM = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("AMBTEMP", out card)) {
                //metaData.WeatherData.Temperature = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("WINDDIR", out card)) {
                //metaData.WeatherData.WindDirection = ParseDouble(card.OriginalValue);
            }
            if (_headerCards.TryGetValue("WINDGUST", out card)) {
                //metaData.WeatherData.WindGust = ParseDouble(card.OriginalValue) / 3.6;
            }
            if (_headerCards.TryGetValue("WINDSPD", out card)) {
                //metaData.WeatherData.WindSpeed = ParseDouble(card.OriginalValue) / 3.6;
            }

            /* WCS */
            if (_headerCards.TryGetValue("CTYPE1", out var ctype1Card) && _headerCards.TryGetValue("CTYPE2", out var ctype2Card)) {
                if (ctype1Card.OriginalValue == "RA---TAN" && ctype2Card.OriginalValue == "DEC--TAN") {
                    if (_headerCards.TryGetValue("CRPIX1", out var crpix1Card)
                        && _headerCards.TryGetValue("CRPIX2", out var crpix2Card)
                        && _headerCards.TryGetValue("CRVAL1", out var crval1Card)
                        && _headerCards.TryGetValue("CRVAL2", out var crval2Card)) {
                        var crPix1 = ParseDouble(crpix1Card.OriginalValue);
                        var crPix2 = ParseDouble(crpix2Card.OriginalValue);
                        var crVal1 = ParseDouble(crval1Card.OriginalValue);
                        var crVal2 = ParseDouble(crval2Card.OriginalValue);

                        if (_headerCards.TryGetValue("CD1_1", out var cd1_1Card)
                            && _headerCards.TryGetValue("CD1_2", out var cd1_2Card)
                            && _headerCards.TryGetValue("CD2_1", out var cd2_1Card)
                            && _headerCards.TryGetValue("CD2_2", out var cd2_2Card)
                        ) {
                            // CDn_m notation
                            var cd1_1 = ParseDouble(cd1_1Card.OriginalValue);
                            var cd1_2 = ParseDouble(cd1_2Card.OriginalValue);
                            var cd2_1 = ParseDouble(cd2_1Card.OriginalValue);
                            var cd2_2 = ParseDouble(cd2_2Card.OriginalValue);

                            //var wcs = new WorldCoordinateSystem(crVal1, crVal2, crPix1, crPix2, cd1_1, cd1_2, cd2_1, cd2_2);
                           // metaData.WorldCoordinateSystem = wcs;
                        } else if (_headerCards.TryGetValue("CDELT1", out var CDELT1Card)
                            && _headerCards.TryGetValue("CDELT2", out var CDELT2Card)
                            && _headerCards.TryGetValue("CROTA2", out var CROTA2Card)
                        ) {
                            // Older CROTA2 notation
                            var cdelt1 = ParseDouble(CDELT1Card.OriginalValue);
                            var cdelt2 = ParseDouble(CDELT2Card.OriginalValue);
                            var crota2 = ParseDouble(CROTA2Card.OriginalValue);

                            //var wcs = new WorldCoordinateSystem(crVal1, crVal2, crPix1, crPix2, cdelt1, cdelt2, crota2);
                            //metaData.WorldCoordinateSystem = wcs;
                        } else {
                            Logger.Debug("FITS WCS - No CROTA2 or CDn_m keywords found");
                        }
                    } else {
                        Logger.Debug("FITS WCS - No CRPIX and CRVAL keywords found");
                    }
                } else {
                    Logger.Debug($"FITS WCS - Incompatible projection found {ctype1Card.OriginalValue} {ctype2Card.OriginalValue}");
                }
            }

            return;// metaData;
        }

        /*private List<IGenericMetaDataHeader> GetAllFITSKeywords() {
            var l = new List<IGenericMetaDataHeader>();            
            if (_headerCards.Count == 0) { return l; }

            foreach (var elem in _headerCards) {
                if (elem.Key == null) { continue; }

                var key = elem.Key;
                if (key == null) { continue; }

                if(elem.Value.Value.StartsWith("'")) {
                    var value = elem.Value.OriginalValue.Trim();
                    l.Add(new StringMetaDataHeader(key, value, elem.Value.Comment));
                } else if (elem.Value.OriginalValue == "T" || elem.Value.OriginalValue == "F") {
                    var value = elem.Value.OriginalValue.Trim() == "T" ? true : false;
                    l.Add(new BoolMetaDataHeader(key, value, elem.Value.Comment));
                } else if (elem.Value.OriginalValue.Contains(".") && double.TryParse(elem.Value.OriginalValue, out var number)) {                    
                    l.Add(new DoubleMetaDataHeader(key, number, elem.Value.Comment));
                } else if (int.TryParse(elem.Value.OriginalValue, out var integer)) {
                    l.Add(new IntMetaDataHeader(key, integer, elem.Value.Comment));
                }
            }
            return l;
        }
        */
        private double ParseDouble(string value) {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var dbl)) {
                return dbl;
            } else {
                return double.NaN;
            }
        }

        private int ParseInt(string value) {
            if (value.EndsWith(".0")) {
                value = value.Replace(".0", "");
            }
            if (int.TryParse(value, out var intVal)) {
                return intVal;
            } else {
                return 0;
            }
        }

        /*private PierSide ParsePierSide(string value) {
            var strVal = value.Trim();

            if (strVal.StartsWith("west", true, CultureInfo.InvariantCulture)) {
                return PierSide.pierWest;
            } else if (strVal.StartsWith("east", true, CultureInfo.InvariantCulture)) {
                return PierSide.pierEast;
            } else {
                return PierSide.pierUnknown;
            }
        }*/

       
    }
}