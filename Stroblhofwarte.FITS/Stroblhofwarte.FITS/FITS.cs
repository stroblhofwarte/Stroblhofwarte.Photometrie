#region "copyright"

/*
    Copyright © 2016 - 2023 Stefan Berg <isbeorn86+NINA@googlemail.com> and the N.I.N.A. contributors

    This file is part of N.I.N.A. - Nighttime Imaging 'N' Astronomy.

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/.

    CHANGELOG:
    ===================================
    Author: Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de, 10.4.2023: Some minor changes to meet my req. for the photometrie tool

*/

#endregion "copyright"

using NINA.Image.FileFormat.FITS.DataConverter;
using nom.tam.fits;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.FITS.Utility;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Stroblhofwarte.FITS
{

    /// <summary>
    /// Specification:
    /// https://fits.gsfc.nasa.gov/fits_standard.html
    /// http://archive.stsci.edu/fits/fits_standard/fits_standard.html
    /// </summary>
    public class FITS
    {

        [SecurityCritical]
        public static DataObjects.FitsImage Load(Uri filePath, bool isBayered) 
        {
            string filename = filePath.LocalPath;
            Fits f = new Fits(filename);
            ImageHDU hdu = (ImageHDU)f.ReadHDU();
            Array[] arr = (Array[])hdu.Data.DataArray;

            var dimensions = hdu.Header.GetIntValue("NAXIS");
            if (dimensions > 2)
            {
                //Debayered Images are not supported. Take the first dimension instead to get at least a monochrome image
                arr = (Array[])arr[0];
            }

            var width = hdu.Header.GetIntValue("NAXIS1");
            var height = hdu.Header.GetIntValue("NAXIS2");
            var bitPix = hdu.Header.GetIntValue("BITPIX");

            var converter = GetConverter(bitPix);
            ushort[] pixels = converter.Convert(arr, width, height);

            //Translate nom.tam.fits into N.I.N.A. FITSHeader
            FITSHeader header = new FITSHeader(width, height);
            var iterator = hdu.Header.GetCursor();
            while (iterator.MoveNext())
            {
                HeaderCard card = (HeaderCard)((DictionaryEntry)iterator.Current).Value;

                if (string.IsNullOrEmpty(card.Value) || card.Key.Equals("COMMENT") || card.Key.Equals("HISTORY"))
                {
                    continue;
                }

                if (card.IsStringValue)
                {
                    header.Add(card.Key, card.Value, card.Comment);
                }
                else
                {
                    if (card.Value.Equals("T"))
                    {
                        header.Add(card.Key, true, card.Comment);
                    }
                    else if (card.Value.Equals("F"))
                    {
                        header.Add(card.Key, false, card.Comment);
                    }
                    else if (card.Value.Contains("."))
                    {
                        if (double.TryParse(card.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                        {
                            header.Add(card.Key, value, card.Comment);
                        }
                    }
                    else
                    {
                        if (int.TryParse(card.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                            header.Add(card.Key, value, card.Comment);
                    }
                }
            }

            DataObjects.FitsImage img = new DataObjects.FitsImage(width, height, bitPix, header, pixels);
            return img;
        }

        [SecurityCritical]
        public static DataObjects.FitsImage LoadHeaderOnly(Uri filePath)
        {
            string filename = filePath.LocalPath;
            // CSharpFits: Use depricated calls to get a proxy. We use the filename interface only.
            Fits f = new Fits(filename);
            ImageHDU hdu = (ImageHDU)f.ReadHDU();
            Array[] arr = (Array[])hdu.Data.DataArray;

            var dimensions = hdu.Header.GetIntValue("NAXIS");
            if (dimensions > 2)
            {
                //Debayered Images are not supported. Take the first dimension instead to get at least a monochrome image
                arr = (Array[])arr[0];
            }

            var width = hdu.Header.GetIntValue("NAXIS1");
            var height = hdu.Header.GetIntValue("NAXIS2");
            var bitPix = hdu.Header.GetIntValue("BITPIX");

            //Translate nom.tam.fits into N.I.N.A. FITSHeader
            FITSHeader header = new FITSHeader(width, height);
            var iterator = hdu.Header.GetCursor();
            while (iterator.MoveNext())
            {
                HeaderCard card = (HeaderCard)((DictionaryEntry)iterator.Current).Value;

                if (string.IsNullOrEmpty(card.Value) || card.Key.Equals("COMMENT") || card.Key.Equals("HISTORY"))
                {
                    continue;
                }

                if (card.IsStringValue)
                {
                    header.Add(card.Key, card.Value, card.Comment);
                }
                else
                {
                    if (card.Value.Equals("T"))
                    {
                        header.Add(card.Key, true, card.Comment);
                    }
                    else if (card.Value.Equals("F"))
                    {
                        header.Add(card.Key, false, card.Comment);
                    }
                    else if (card.Value.Contains("."))
                    {
                        if (double.TryParse(card.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                        {
                            header.Add(card.Key, value, card.Comment);
                        }
                    }
                    else
                    {
                        if (int.TryParse(card.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                            header.Add(card.Key, value, card.Comment);
                    }
                }
            }

            DataObjects.FitsImage img = new DataObjects.FitsImage(width, height, bitPix, header, null);
            return img;

        }

        private static IDataConverter GetConverter(int bitPix)
        {
            switch (bitPix)
            {
                case BITPIX_BYTE:
                    return new ByteConverter();

                case BITPIX_SHORT:
                    return new ShortConverter();

                case BITPIX_INT:
                    return new IntConverter();

                case BITPIX_LONG:
                    return new LongConverter();

                case BITPIX_FLOAT:
                    return new FloatConverter();

                case BITPIX_DOUBLE:
                    return new DoubleConverter();

                default:
                    throw new InvalidDataException($"Invalid BITPIX in FITS header {bitPix}");
            }
        }

        /* Header card size Specification: http://archive.stsci.edu/fits/fits_standard/node29.html#SECTION00912100000000000000 */
        public const int HEADERCARDSIZE = 80;
        /* Blocksize specification: http://archive.stsci.edu/fits/fits_standard/node13.html#SECTION00810000000000000000 */
        public const int BLOCKSIZE = 2880;
        public const int BITPIX_BYTE = 8;
        public const int BITPIX_SHORT = 16;
        public const int BITPIX_INT = 32;
        public const int BITPIX_LONG = 64;
        public const int BITPIX_FLOAT = -32;
        public const int BITPIX_DOUBLE = -64;
    }
}