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