using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.DataPackages
{
    public class StarDataRelay
    {
        #region Singelton

        private static StarDataRelay _instance;
        public static StarDataRelay Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new StarDataRelay();
                return _instance;
            }
        }

        #endregion

        #region Events

        public event EventHandler CheckStarChanged;
        public event EventHandler CompStarChanged;
        public event EventHandler StarChanged;
        public event EventHandler UserInfoChanged;


        #endregion

        public bool CValid { get; set; }
        public bool KValid { get; set; }

        private string _userInfo;
        public string UserInfo
        {
            get { return _userInfo; }
            set
            {
                _userInfo = value;
                if (UserInfoChanged != null)
                {
                    UserInfoChanged(this, null);
                }
            }
        }

        public bool UserInfoVisibility { get; set; }

        private double _compMag;
        public double CompMag
        {
            set
            {
                _compMag = value;
                if (CompStarChanged != null)
                {
                    CompStarChanged(this, null);
                }
            }
            get { return _compMag; }
        }

        private string _compName;
        public string CompName
        {
            set
            {
                _compName = value;
                if (CompStarChanged != null)
                {
                    CompStarChanged(this, null);
                }
            }
            get { return _compName; }
        }

        private string _compAUID;
        public string CompAUID
        {
            set
            {
                _compAUID = value;
                if (CompStarChanged != null)
                {
                    CompStarChanged(this, null);
                }
            }
            get { return _compAUID; }
        }

        private double _checkMag;
        public double CheckMag
        {
            set
            {
                _checkMag = value;
                if (CheckStarChanged != null)
                {
                    CheckStarChanged(this, null);
                }
            }
            get { return _checkMag; }
        }

        private string _checkName;
        public string CheckName
        {
            set
            {
                _checkName = value;
                if (CheckStarChanged != null)
                {
                    CheckStarChanged(this, null);
                }
            }
            get { return _checkName; }
        }

        private string _checkAUID;
        public string CheckAUID
        {
            set
            {
                _checkAUID = value;
                if (CheckStarChanged != null)
                {
                    CheckStarChanged(this, null);
                }
            }
            get { return _checkAUID; }
        }

        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                if (StarChanged != null)
                {
                    StarChanged(this, null);
                }
            }
            get { return _name; }
        }

        private string _chartId;
        public string ChartId
        {
            set
            {
                _chartId = value;
                if (StarChanged != null)
                {
                    StarChanged(this, null);
                }
            }
            get { return _chartId; }
        }

        private string _filter;
        public string Filter
        {
            set
            {
                _filter = value;
            }
            get { return _filter; }
        }

        #region CTor

        public StarDataRelay()
        {
            CValid = false;
            KValid = false;
        }

        #endregion
    }
}
