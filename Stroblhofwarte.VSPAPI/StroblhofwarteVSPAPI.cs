﻿#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using Stroblhofwarte.VSPAPI.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Stroblhofwarte.VSPAPI
{
    public class StroblhofwarteVSPAPI
    {

        public async Task<VariableStar> GetAAVSOData(string name, string fov, string maglimit)
        {
            try
            {
                AAVSOChartRequestParameter p = new AAVSOChartRequestParameter();
                p.Star = name;
                p.Fov = fov;
                p.Maglimit = maglimit;

                string jsonStr = RESTRequest(p);
                JObject json = JObject.Parse(jsonStr);
                if (json["error"] != null)
                {
                    return null;
                }
                VariableStar varStar = new VariableStar(new Star(json["ra"].ToString(),
                    json["dec"].ToString(),
                    json["star"].ToString(),
                    json["auid"].ToString(),
                    json["fov"].ToString(),
                    json["maglimit"].ToString()));
                varStar.ChartId = json["chartid"].ToString();

                var references = json["photometry"];
                foreach (var refstar in references)
                {
                    Star refStar = new Star(refstar["ra"].ToString(),
                    refstar["dec"].ToString(),
                    refstar["label"].ToString(),
                    refstar["auid"].ToString(),
                    "",
                    "");
                    refStar.Comment = refstar["comments"].ToString();
                    var bands = refstar["bands"];
                    foreach (var item in bands)
                    {
                        Mag band = new Mag(item["band"].ToString(),
                            item["mag"].ToString(),
                            item["error"].ToString());
                        refStar.AddMagnitude(band);
                    }
                    varStar.AddReferenceStar(refStar);
                }
                return varStar;
            } catch (Exception ex)
            {
                return new VariableStar();
            }
        }

        public async Task<VariableStar> GetAAVSOData(double ra, double dec, string fov, string maglimit)
        {
            AAVSOChartRequestParameter p = new AAVSOChartRequestParameter();
            p.Ra = ra.ToString(CultureInfo.InvariantCulture);
            p.Dec = dec.ToString(CultureInfo.InvariantCulture);
            p.Fov = fov;
            p.Maglimit = maglimit;

            string jsonStr = RESTRequest(p);
            JObject json = JObject.Parse(jsonStr);

            VariableStar varStar = new VariableStar(new Star(json["ra"].ToString(),
                json["dec"].ToString(),
                "Unknown",
                "Unknown",
                json["fov"].ToString(),
                json["maglimit"].ToString()));

            var references = json["photometry"];
            foreach (var refstar in references)
            {
                Star refStar = new Star(refstar["ra"].ToString(),
                refstar["dec"].ToString(),
                refstar["label"].ToString(),
                refstar["auid"].ToString(),
                "",
                "");
                refStar.Comment = refstar["comments"].ToString();
                var bands = refstar["bands"];
                foreach (var item in bands)
                {
                    Mag band = new Mag(item["band"].ToString(),
                        item["mag"].ToString(),
                        item["error"].ToString());
                    refStar.AddMagnitude(band);
                }
                varStar.AddReferenceStar(refStar);
            }
            return varStar;
        }
        private string RESTRequest(AAVSOChartRequestParameter param)
        {
            try
            {
                var client = new RestClient("https://app.aavso.org");
                var request = new RestRequest("vsp/api/chart", Method.Get);
                Dictionary<string, string> restParams = param.Params;
                foreach(KeyValuePair<string, string> pair in restParams)
                {
                    request.AddParameter(pair.Key, pair.Value);
                }
                request.RequestFormat = DataFormat.Json;
              
                RestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}