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
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public string AstrometrynetKey
        {
            get { return Stroblhofwarte.Config.Settings.Default.AstrometrynetKey; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AstrometrynetKey = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public double AAVSOFov
        {
            get { return Stroblhofwarte.Config.Settings.Default.AAVSOFov; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AAVSOFov = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public double AAVSOLimitMag
        {
            get { return Stroblhofwarte.Config.Settings.Default.AAVSOLimitMag; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.AAVSOLimitMag = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public int MagnificationN
        {
            get { return Stroblhofwarte.Config.Settings.Default.MagnificationN; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.MagnificationN = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }
        public string FilterDatabasePath
        {
            get { return Stroblhofwarte.Config.Settings.Default.FilterDatabasePath; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.FilterDatabasePath = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public string StandardFieldsPath
        {
            get { return Stroblhofwarte.Config.Settings.Default.StandardFields; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.StandardFields = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public string OBSCODE
        {
            get { return Stroblhofwarte.Config.Settings.Default.OBSCODE; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.OBSCODE = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        public string OBSTYPE
        {
            get { return Stroblhofwarte.Config.Settings.Default.OBSTYPE; }
            set
            {
                Stroblhofwarte.Config.Settings.Default.OBSTYPE = value;
                Stroblhofwarte.Config.Settings.Default.Save();
            }
        }

        #endregion
        #region Ctor

        #endregion
    }
}
