using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Config
{
    public class GlobalConfig
    {
        #region Singelton

        private static Stroblhofwarte.Config.GlobalConfig _instance;
        public static Stroblhofwarte.Config.GlobalConfig Instance { get
            {
                if(_instance == null)
                    _instance = new Stroblhofwarte.Config.GlobalConfig();
                return _instance;
            }
        }

        #endregion

        #region Properties

        public string AstrometrynetHost 
        {
            get { return Stroblhofwarte.Config.Settings.Default.AstrometrynetHost; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AstrometrynetHost = value;
            }
        }

        public string AstrometrynetKey
        {
            get { return Stroblhofwarte.Config.Settings.Default.AstrometrynetKey; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AstrometrynetKey = value;
            }
        }

        public double AAVSOFov
        {
            get { return Stroblhofwarte.Config.Settings.Default.AAVSOFov; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AAVSOFov = value;
            }
        }

        public double AAVSOLimitMag
        {
            get { return Stroblhofwarte.Config.Settings.Default.AAVSOLimitMag; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AAVSOLimitMag = value;
            }
        }

        public int MagnificationN
        {
            get { return Stroblhofwarte.Config.Settings.Default.MagnificationN; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.MagnificationN = value;
            }
        }
        public string FilterDatabasePath
        {
            get { return Stroblhofwarte.Config.Settings.Default.FilterDatabasePath; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.FilterDatabasePath = value;
            }
        }

        #endregion
        #region Ctor

        #endregion
    }
}
