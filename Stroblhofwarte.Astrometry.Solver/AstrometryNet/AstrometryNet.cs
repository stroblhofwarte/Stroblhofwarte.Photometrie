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

        public bool Solve(string file)
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
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile("https://nova.astrometry.net/new_fits_file/" + submissionStatusResponse.Result.jobs[0], "WCS_" + file);
                    }
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
    }
}
