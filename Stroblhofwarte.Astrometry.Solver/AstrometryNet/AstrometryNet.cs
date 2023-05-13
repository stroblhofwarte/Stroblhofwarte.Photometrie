#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using software.elendil.AstrometryNet.Enum;
using software.elendil.AstrometryNet.Json;
using software.elendil.AstrometryNet;
using Stroblhofwarte.Astrometry.Solver.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.FITS;
using nom.tam.fits;
using nom.tam.util;
using System.Collections;

namespace Stroblhofwarte.Astrometry.Solver.AstrometryNet
{
    public class AstrometryNet : IAstrometrySolver
    {
        #region Properties

        private string _apiKey;
        private string _host;
        

        #endregion

        #region CTor

        public AstrometryNet(string host, string apiKey)
        {
            _apiKey = apiKey;
            _host = host;
        }

        private string _errorText = string.Empty;
        public string ErrorText
        {
            get { return _errorText; }
        }

        #endregion

        public async Task<bool> Solve(string file)
        {
            try
            {
                var client = new Client(_apiKey, _host);
                var res = client.Login();
                if(res.status != ResponseStatus.success)
                {
                    _errorText = "Login : " + res.status;
                    return false;
                }
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;

                var uploadArguments = new UploadArgs { publicly_visible = Visibility.n };
                var uploadResponse = client.Upload(file, uploadArguments);

                Task<SubmissionStatusResponse> submissionStatusResponse = client.GetSubmissionStatus(uploadResponse.subid, token);
                Task<JobStatusResponse> jobStatusResponse = client.GetJobStatus(submissionStatusResponse.Result.jobs[0], token);

                if (jobStatusResponse.Result.status.Equals(ResponseJobStatus.success))
                {
                    var calibrationResponse = client.GetCalibration(submissionStatusResponse.Result.jobs[0]);
                    var objectsInFieldResponse = client.GetObjectsInField(submissionStatusResponse.Result.jobs[0]);

                    // Das Ergebnis-Image mit WCS ist unter https://nova.astrometry.net/new_fits_file/{submissionStatusResponse.Result.jobs[0]}
                    // Herunterladen über C#
                   
                    string path = Path.GetDirectoryName(file);
                    string filename = Path.GetFileName(file);
                    
                    Uri uri = new Uri("https://nova.astrometry.net/wcs_file/" + submissionStatusResponse.Result.jobs[0]);
                    HttpClient webClient = new HttpClient();
                    webClient.Timeout = new TimeSpan(0, 5, 0);
                    var response = await webClient.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    await using var ms = await response.Content.ReadAsStreamAsync();
                    await using var fs = File.Create(path + "\\" + "wcs_" + filename + ".wcs");
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fs);
                    fs.Close();
                    IFits fitsMerge = new StroblhofwarteFITS();
                    MergeHeaderIntoImage(path + "\\" + "wcs_" + filename + ".wcs", file);

                    //webClient.Fi .DownloadFile("https://nova.astrometry.net/new_fits_file/" + submissionStatusResponse.Result.jobs[0], path + "\\" + "wcs_" + filename);
                    //System.IO.File.Move(path + "\\" + filename, path + "\\" + filename + ".bak", true);
                    //System.IO.File.Move(path + "\\" + "wcs_" + filename, path + "\\" + filename, true);

                    return true;
                }
                else
                {
                    _errorText = "Status : " + jobStatusResponse.Result.status;
                    return false;
                }
            }
            catch (Exception e)
            {
                _errorText = e.ToString();
                return false;
            }
            return false;
        }

        private bool MergeHeaderIntoImage(string wcs, string img)
        {
            Fits f = new Fits(wcs);
            ImageHDU hdu = (ImageHDU)f.ReadHDU();
            f.Close();
            
            Fits f2 = new Fits(img);
            ImageHDU hdu2 = (ImageHDU)f2.ReadHDU();
            Array[] arr = (Array[])hdu2.Data.DataArray;


            var iterator = hdu.Header.GetCursor();
            while (iterator.MoveNext())
            {
                HeaderCard card = (HeaderCard)((DictionaryEntry)iterator.Current).Value;
                if (card.Key == "WCSAXES") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CTYPE1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CTYPE2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "EQUINOX") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "LONPOLE") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "LATPOLE") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CRVAL1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CRVAL2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CRPIX1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CRPIX2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CUNIT1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CUNIT2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CD1_1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CD1_2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CD2_1") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "CD2_2") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "IMAGEW") hdu2.AddValue(card.Key, card.Value, card.Comment);
                if (card.Key == "IMAGEH") hdu2.AddValue(card.Key, card.Value, card.Comment);
            }

            f2.Close();
            BufferedFile bf = new BufferedFile(img /*args[1]*/, FileAccess.ReadWrite, FileShare.ReadWrite);
            f2.Write(bf);
            bf.Close();
            return true;
        }
    }
}
