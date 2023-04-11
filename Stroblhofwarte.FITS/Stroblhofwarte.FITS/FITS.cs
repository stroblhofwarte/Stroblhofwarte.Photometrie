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

using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.FITS.Utility;
using System;
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
        public static DataObjects.FitsImage Load(Uri filePath, bool isBayered) {
            IntPtr fitsPtr = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;
            try {
                var bytes = File.ReadAllBytes(filePath.LocalPath);

                buffer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, buffer, bytes.Length);

                UIntPtr size = new UIntPtr((uint)bytes.Length);
                UIntPtr deltaSize = UIntPtr.Zero;

                CfitsioNative.fits_open_memory(out fitsPtr, string.Empty, CfitsioNative.IOMODE.READONLY, ref buffer, ref size, ref deltaSize, IntPtr.Zero, out var status);
                CfitsioNative.CheckStatus("fits_open_memory", status);

                var dimensions = CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS");
                if (dimensions > 2) {
                    Logger.Warning("Reading debayered FITS images not supported. Reading the first 2 axes to get a monochrome image");
                }

                var width = (int)CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS1");
                var height = (int)CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS2");
                var bitPix = (CfitsioNative.BITPIX)(int)CfitsioNative.fits_read_key_long(fitsPtr, "BITPIX");
                int intBitPix = (int)CfitsioNative.fits_read_key_long(fitsPtr, "BITPIX");
                var pixels = CfitsioNative.read_ushort_pixels(fitsPtr, bitPix, 2, width * height);

                //Translate CFITSio into N.I.N.A. FITSHeader
                FITSHeader header = new FITSHeader(width, height);
                CfitsioNative.fits_get_hdrspace(fitsPtr, out var numKeywords, out var numMoreKeywords, out status);
                CfitsioNative.CheckStatus("fits_get_hdrspace", status);
                for (int headerIdx = 1; headerIdx <= numKeywords; ++headerIdx) {
                    CfitsioNative.fits_read_keyn(fitsPtr, headerIdx, out var keyName, out var keyValue, out var keyComment);

                    if (string.IsNullOrEmpty(keyValue) || keyName.Equals("COMMENT") || keyName.Equals("HISTORY")) {
                        continue;
                    }

                    if (keyValue.Equals("T")) {
                        header.Add(keyName, true, keyComment);
                    } else if (keyValue.Equals("F")) {
                        header.Add(keyName, false, keyComment);
                    } else if (keyValue.StartsWith("'")) {
                        // Treat as a string
                        keyValue = $"{keyValue.TrimStart('\'').TrimEnd('\'', ' ').Replace(@"''", @"'")}";
                        header.Add(keyName, keyValue, keyComment);

                    } else if (keyValue.Contains(".")) {
                        if (double.TryParse(keyValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) {
                            header.Add(keyName, value, keyComment);
                        }
                    } else {
                        if (int.TryParse(keyValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)) {
                            header.Add(keyName, value, keyComment);
                        } else {
                            // Treat as a string
                            keyValue = $"{keyValue.TrimStart('\'').TrimEnd('\'', ' ').Replace(@"''", @"'")}";
                            header.Add(keyName, keyValue, keyComment);
                        }
                    }
                }
                DataObjects.FitsImage img = new DataObjects.FitsImage(width, height, intBitPix, header, pixels);
                return img;

            } catch (AccessViolationException ex) {
                Logger.Error($"{nameof(FITS)} - Access Violation Exception occurred during cfitsio load!", ex);
                // Finally blocks are not executed after corrupted state exception
                if (fitsPtr != IntPtr.Zero) {
                    try {
                        CfitsioNative.fits_close_file(fitsPtr, out var status);
                    } catch(Exception) { }                    
                }
                if (buffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(buffer);
                }
                throw new Exception($"Unable to load FITS file from {filePath.LocalPath}");
            } finally {
                if (fitsPtr != IntPtr.Zero) {
                    CfitsioNative.fits_close_file(fitsPtr, out var status);
                }
                if (buffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        [SecurityCritical]
        public static DataObjects.FitsImage LoadHeaderOnly(Uri filePath)
        {
            IntPtr fitsPtr = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;
            try
            {
                var bytes = File.ReadAllBytes(filePath.LocalPath);

                buffer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, buffer, bytes.Length);

                UIntPtr size = new UIntPtr((uint)bytes.Length);
                UIntPtr deltaSize = UIntPtr.Zero;

                CfitsioNative.fits_open_memory(out fitsPtr, string.Empty, CfitsioNative.IOMODE.READONLY, ref buffer, ref size, ref deltaSize, IntPtr.Zero, out var status);
                CfitsioNative.CheckStatus("fits_open_memory", status);

                var dimensions = CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS");
                if (dimensions > 2)
                {
                    Logger.Warning("Reading debayered FITS images not supported. Reading the first 2 axes to get a monochrome image");
                }

                var width = (int)CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS1");
                var height = (int)CfitsioNative.fits_read_key_long(fitsPtr, "NAXIS2");
                var bitPix = (CfitsioNative.BITPIX)(int)CfitsioNative.fits_read_key_long(fitsPtr, "BITPIX");
                int intBitPix = (int)CfitsioNative.fits_read_key_long(fitsPtr, "BITPIX");
               
                //Translate CFITSio into N.I.N.A. FITSHeader
                FITSHeader header = new FITSHeader(width, height);
                CfitsioNative.fits_get_hdrspace(fitsPtr, out var numKeywords, out var numMoreKeywords, out status);
                CfitsioNative.CheckStatus("fits_get_hdrspace", status);
                for (int headerIdx = 1; headerIdx <= numKeywords; ++headerIdx)
                {
                    CfitsioNative.fits_read_keyn(fitsPtr, headerIdx, out var keyName, out var keyValue, out var keyComment);

                    if (string.IsNullOrEmpty(keyValue) || keyName.Equals("COMMENT") || keyName.Equals("HISTORY"))
                    {
                        continue;
                    }

                    if (keyValue.Equals("T"))
                    {
                        header.Add(keyName, true, keyComment);
                    }
                    else if (keyValue.Equals("F"))
                    {
                        header.Add(keyName, false, keyComment);
                    }
                    else if (keyValue.StartsWith("'"))
                    {
                        // Treat as a string
                        keyValue = $"{keyValue.TrimStart('\'').TrimEnd('\'', ' ').Replace(@"''", @"'")}";
                        header.Add(keyName, keyValue, keyComment);

                    }
                    else if (keyValue.Contains("."))
                    {
                        if (double.TryParse(keyValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                        {
                            header.Add(keyName, value, keyComment);
                        }
                    }
                    else
                    {
                        if (int.TryParse(keyValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                        {
                            header.Add(keyName, value, keyComment);
                        }
                        else
                        {
                            // Treat as a string
                            keyValue = $"{keyValue.TrimStart('\'').TrimEnd('\'', ' ').Replace(@"''", @"'")}";
                            header.Add(keyName, keyValue, keyComment);
                        }
                    }
                }
                DataObjects.FitsImage img = new DataObjects.FitsImage(width, height, intBitPix, header, null);
                return img;

            }
            catch (AccessViolationException ex)
            {
                Logger.Error($"{nameof(FITS)} - Access Violation Exception occurred during cfitsio load!", ex);
                // Finally blocks are not executed after corrupted state exception
                if (fitsPtr != IntPtr.Zero)
                {
                    try
                    {
                        CfitsioNative.fits_close_file(fitsPtr, out var status);
                    }
                    catch (Exception) { }
                }
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
                throw new Exception($"Unable to load FITS file from {filePath.LocalPath}");
            }
            finally
            {
                if (fitsPtr != IntPtr.Zero)
                {
                    CfitsioNative.fits_close_file(fitsPtr, out var status);
                }
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
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