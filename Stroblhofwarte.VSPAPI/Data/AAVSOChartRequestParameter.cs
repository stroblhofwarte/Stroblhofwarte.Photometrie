#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Stroblhofwarte.VSPAPI.Data
{
    public class AAVSOChartRequestParameter
    {
        #region Properties
        private Dictionary<string, string> paramCache = new Dictionary<string, string>();
        public Dictionary<string, string> Params {  get { return paramCache; } }

        private string _star;
        public string Star { get { return _star; } set { _star = value; AddParamCache("star", value); } }
     
        private string _ra;
        public string Ra { get { return _ra; } set { _ra = value; AddParamCache("ra", value); } }


        private string _dec;
        public string Dec { get { return _dec; } set { _dec = value; AddParamCache("dec", value); } }

        private string _fov;
        public string Fov { get { return _fov; } set { _fov = value; AddParamCache("fov", value); } }

        private string _maglimit;
        public string Maglimit { get { return _maglimit; } set { _maglimit = value; AddParamCache("maglimit", value); } }

        #endregion
        #region Ctor
        #endregion

        private void AddParamCache(string name, string value)
        {
            if(paramCache.ContainsKey(name))
            {
                paramCache[name] = value;
            }
            else
            {
                paramCache.Add(name, value);
            }
        }
    }
}