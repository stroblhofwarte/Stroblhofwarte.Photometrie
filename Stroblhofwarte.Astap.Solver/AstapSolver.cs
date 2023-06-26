using Stroblhofwarte.Astap.Solver.Interface;
using System.Reflection.Metadata;
using System;
using System.IO;
using System.Collections;
using Stroblhofwarte.FITS.Interface;
using Stroblhofwarte.FITS;
using nom.tam.fits;
using nom.tam.util;
using System.IO.Pipes;
using System.Text;

namespace Stroblhofwarte.Astap.Solver
{
    public class AstapSolver : IAstapSolver
    {
        private string _path = string.Empty;
        private string _arguments = string.Empty;

        private volatile bool _retVal = false;

        private string _errorText = string.Empty;
        public string ErrorText
        {
            get { return _errorText; }
        }

        #region CTor

        public AstapSolver(string path, string arguments)
        {
            _path = path;
            _arguments = arguments;
        }

        #endregion
        public async Task<bool> Solve(string filename)
        {
            try
            {
                CancellationToken ct = new CancellationToken(); // TODO
                string workingdir = Path.GetDirectoryName(filename);
                _retVal = true;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                startInfo.FileName = "\"" + _path + "\"";
                startInfo.WorkingDirectory = workingdir;
                startInfo.UseShellExecute = true;
                startInfo.RedirectStandardOutput = false;
                startInfo.CreateNoWindow = false;
                startInfo.Arguments = " -f \"" + filename + "\" " + _arguments;
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;

                process.OutputDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                {
                    _errorText = e.Data;
                };

                process.ErrorDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                {
                    _errorText = e.Data;
                    _retVal = false;
                };
                process.Start();
                await process.WaitForExitAsync(ct);
                if (_retVal)
                {
                    string wcsFilename = Path.ChangeExtension(filename, ".wcs");
                    MergeHeaderIntoImage(wcsFilename, filename);
                }
                return _retVal;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        // Stupid copy and paste from AstrometryNet. A abstract base class is required!
        private bool MergeHeaderIntoImage(string wcs, string img)
        {
            
            Fits f2 = new Fits(img);
            ImageHDU hdu2 = (ImageHDU)f2.ReadHDU();
            Array[] arr = (Array[])hdu2.Data.DataArray;

            var fileStream = new FileStream(wcs, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] keyValueComment = line.Split("=");
                    if (keyValueComment.Length != 2)
                        continue;
                    string[] valueComment = keyValueComment[1].Split("/");
                    if (valueComment.Length != 2)
                        continue;
                    if (keyValueComment[0].Trim() == "WCSAXES") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CTYPE1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CTYPE2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "EQUINOX") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "LONPOLE") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "LATPOLE") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CRVAL1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CRVAL2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CRPIX1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CRPIX2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CUNIT1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CUNIT2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CD1_1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CD1_2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CD2_1") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "CD2_2") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "IMAGEW") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                    if (keyValueComment[0].Trim() == "IMAGEH") hdu2.AddValue(keyValueComment[0], valueComment[0], valueComment[1]);
                }
            }
            f2.Close();
            BufferedFile bf = new BufferedFile(img /*args[1]*/, FileAccess.ReadWrite, FileShare.ReadWrite);
            f2.Write(bf);
            bf.Close();
            return true;
        }
    }
}