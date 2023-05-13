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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using System.Windows.Navigation;

namespace Stroblhofwarte.AperturePhotometry
{
    public class Filter
    {
        #region Singelton

        private static Filter _instance = null;

        public static Filter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Filter();
                return _instance;
            }
        }

        #endregion

        #region Properties

        private readonly string FILENAME = "Stroblhofwarte.Filter.csv";

        private List<FilterObject> _filters;
        public List<FilterObject> Filters { get; private set; }

        #endregion

        #region Ctor

        public Filter()
        {
            Filters = new List<FilterObject>();
            if (!Load())
            {
                Filters.Add(new FilterObject() { AAVSOName = "Johnson U", AAVSOShort = "U", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Johnson B", AAVSOShort = "B", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Johnson V", AAVSOShort = "V", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Johnson R", AAVSOShort = "RJ", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Johnson I", AAVSOShort = "IJ", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Cousins R", AAVSOShort = "R", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Cousins I", AAVSOShort = "I", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Near-Infrared J", AAVSOShort = "J", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Near-Infrared H", AAVSOShort = "H", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Near-Infrared K", AAVSOShort = "K", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Sloan u", AAVSOShort = "SU", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Sloan g", AAVSOShort = "SG", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Sloan r", AAVSOShort = "SR", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Sloan i", AAVSOShort = "SI", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Sloan z", AAVSOShort = "SZ", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren u", AAVSOShort = "STU", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren v", AAVSOShort = "STV", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren b", AAVSOShort = "STB", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren y", AAVSOShort = "STY", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren Hbw", AAVSOShort = "STHBW", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Stromgren Hbn", AAVSOShort = "STHBN", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Optec Wing A", AAVSOShort = "MA", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Optec Wing B", AAVSOShort = "MB", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Optec Wing C (or I)", AAVSOShort = "MI", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "PanSTARSS z-short", AAVSOShort = "ZS", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "PanSTARRS Y", AAVSOShort = "Y", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "DSLR Green", AAVSOShort = "TG", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "DSLR Blue", AAVSOShort = "TB", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "DSLR Red", AAVSOShort = "TR", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Clear reduced to V", AAVSOShort = "CV", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Clear reduced to R", AAVSOShort = "CR", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Clear with blue-blocking", AAVSOShort = "CBB", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Halpha", AAVSOShort = "HA", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Halpha Continuum", AAVSOShort = "HAC", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Blue visual filter", AAVSOShort = "Blue-vis.", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Green visual filter", AAVSOShort = "Green-vis.", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Red visual filter", AAVSOShort = "Red-vis.", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Orange filter", AAVSOShort = "Orange", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Yellow visual filter", AAVSOShort = "Yellow-vis.", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Visual", AAVSOShort = "Vis.", MyFilter = "" });
                Filters.Add(new FilterObject() { AAVSOName = "Other", AAVSOShort = "O", MyFilter = "" });
                Save();
            }
        }

        #endregion
        public bool Load()
        {
            try
            {

                Filters = new List<FilterObject>();
                string filename = FILENAME;
                if (Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath != string.Empty)
                    filename = Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath + "\\" + FILENAME;
                string[] lines = File.ReadAllLines(filename);

                foreach (string line in lines)
                {
                    if (line.StartsWith("#")) continue;
                    string[] fields = line.Split(";");
                    if (fields.Length != 3) continue;
                    Filters.Add(new FilterObject() { AAVSOName = fields[0], AAVSOShort = fields[1], MyFilter = fields[2] });
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public bool Save()
        {
            try
            {
                string filename = FILENAME;
                if (Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath != string.Empty)
                    filename = Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath + "\\" + FILENAME;
                using (StreamWriter writetext = new StreamWriter(filename))
                {
                    foreach (FilterObject filter in Filters)
                    {
                        string line = filter.AAVSOName + ";" + filter.AAVSOShort + ";" + filter.MyFilter;
                        writetext.WriteLine(line);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateFilter(string aavsoshort, string myfilter)
        {
            foreach(FilterObject f in Filters)
            {
                if(aavsoshort == f.AAVSOShort)
                {
                    f.MyFilter = myfilter;
                    break;
                }
            }
        }

        public string TranslateToMyFilter(string aavsoshort)
        {
            foreach (FilterObject f in Filters)
            {
                if (aavsoshort == f.AAVSOShort)
                {
                    return f.MyFilter;
                }
            }
            return string.Empty;
        }

        public string TranslateToAAVSOFilter(string myfilter)
        {
            foreach (FilterObject f in Filters)
            {
                if (myfilter == f.MyFilter)
                {
                    return f.AAVSOShort;
                }
            }
            return string.Empty;
        }
    }

    public class FilterObject
    {
        #region Properties

        public string AAVSOName { get; set; }
        public string AAVSOShort { get; set; }
        public string MyFilter { get; set; }

        #endregion

    }
}
