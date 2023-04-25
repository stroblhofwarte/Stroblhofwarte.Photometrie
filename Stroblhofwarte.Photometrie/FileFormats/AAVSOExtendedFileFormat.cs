using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.FITS.DataObjects;
using Stroblhofwarte.Photometrie.Dialogs;
using Stroblhofwarte.VSPAPI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;

namespace Stroblhofwarte.Photometrie.FileFormats
{
    public class AAVSOExtendedFileFormat
    {
        #region Singelton

        private static AAVSOExtendedFileFormat _instance;
        public static AAVSOExtendedFileFormat Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AAVSOExtendedFileFormat();
                }
                return _instance;
            }
        }

        #endregion

        #region Event

        public event EventHandler DatabaseChanged;

        #endregion

        #region Properties

        private string FILENAME = "aavso.report.csv";
        public AAVSOHeader Header { get; set;}
        public List<AAVSOList> Values { get; set; }

        #endregion
        #region Ctor
        public AAVSOExtendedFileFormat()
        {
            Header = new AAVSOHeader() { OBSCODE = Stroblhofwarte.Config.GlobalConfig.Instance.OBSCODE, OBSTYPE = Stroblhofwarte.Config.GlobalConfig.Instance.OBSTYPE };
            Load();
        }

        public AAVSOExtendedFileFormat(string filename)
        {
            Header = new AAVSOHeader() { OBSCODE = Stroblhofwarte.Config.GlobalConfig.Instance.OBSCODE, OBSTYPE = Stroblhofwarte.Config.GlobalConfig.Instance.OBSTYPE };
            Load(filename);
        }
        #endregion

        public bool Load()
        {
            return Load(FILENAME);
        }

        public bool Load(string filename)
        {
            try
            {
                string path = filename;
                Values = new List<AAVSOList>();
                if (Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath != string.Empty)
                    path = Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath + "\\" + filename;
                string[] lines = System.IO.File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    if (Header.CheckAndUpdate(line)) continue;
                    if (line.StartsWith("#")) continue;
                    string[] fields = line.Split(";");
                    if (fields.Length != 15) continue;
                    Values.Add(new AAVSOList() { NAME = fields[0],
                        DATE = fields[1],
                        MAG = fields[2],
                        MERR = fields[3],
                        FILT = fields[4],
                        TRANS = fields[5],
                        MTYPE = fields[6],
                        CNAME = fields[7],
                        CMAG = fields[8],
                        KNAME = fields[9],
                        KMAG = fields[10],
                        AMASS = fields[11],
                        GROUP = fields[12],
                        CHART = fields[13],
                        NOTES = fields[14]
                    });
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
            return Save(FILENAME);
        }

        public bool Save(string filename)
        {
            try
            {
                string path = filename;
                if (Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath != string.Empty)
                    path = Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath + "\\" + filename;
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine("#TYPE=" + Header.TYPE);
                    writetext.WriteLine("#OBSCODE=" + Header.OBSCODE);
                    writetext.WriteLine("#SOFTWARE=" + Header.SOFTWARE);
                    writetext.WriteLine("#DELIM=" + Header.DELIM);
                    writetext.WriteLine("#DATE=" + Header.DATE);
                    writetext.WriteLine("#OBSTYPE=" + Header.OBSTYPE);

                    writetext.WriteLine("#NAME;DATE;MAG;MERR;FILT;TRANS;MTYPE;CNAME;CMAG;KNAME;KMAG;AMASS;GROUP;CHART;NOTES");
                    foreach (AAVSOList e in Values)
                    {
                        string line = e.NAME + ";" + e.DATE + ";" + e.MAG + ";" + e.MERR + ";" + e.FILT + ";" + e.TRANS + ";" + e.MTYPE + ";" + e.CNAME + ";" + e.CMAG
                             + ";" + e.KNAME + ";" + e.KMAG + ";" + e.AMASS + ";" + e.GROUP + ";" + e.CHART + ";" + e.NOTES;
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

        public bool Add(string name, string date, string mag, string merr, string filt, string trans, string mtype, string cname, string cmag, string kname, string kmag, string amass, string group, string chart, string notes)
        {
            AAVSOList e = new AAVSOList()
            {
                NAME = name,
                DATE = date,
                MAG = mag,
                MERR = merr,
                FILT = filt,
                TRANS = trans,
                MTYPE = mtype,
                CNAME = cname,
                CMAG = cmag,
                KNAME = kname,
                KMAG = kmag,
                AMASS = amass,
                GROUP = group,
                CHART = chart,
                NOTES = notes
            };
            Values.Add(e);
            if (DatabaseChanged != null)
                DatabaseChanged(this, null);
            return Save();
        }

        public bool Delete(string id)
        {
            int idx = 0;
            foreach(AAVSOList l in Values)
            {
                if(l.ID == id)
                {
                    break;
                }
                idx++;
            }
            if (idx >= Values.Count) return false;
            Values.RemoveAt(idx);
            if (DatabaseChanged != null)
                DatabaseChanged(this, null);
            return Save();
        }
    }

    public class AAVSOHeader
    {
        #region Properties

        public string TYPE { get; private set; }
        public string OBSCODE { get; set; }
        public string SOFTWARE { get; private set; }
        public string DELIM { get; private set; }
        public string DATE { get; private set; }
        public string OBSTYPE { get; set; }

        #endregion
        #region Ctor
        public AAVSOHeader()
        {
            TYPE = "Extended";
            SOFTWARE = "Stroblhofwarte.Photometrie";
            DELIM = ";";
            DATE = "JD";
        }
        #endregion

        public bool CheckAndUpdate(string line)
        {
            if (!line.StartsWith("#")) return false;
            if (line.StartsWith("#TYPE=")) { TYPE = Destill(line); return true;};
            if (line.StartsWith("#OBSCODE=")) { OBSCODE = Destill(line); return true; }
            if (line.StartsWith("#SOFTWARE=")) { SOFTWARE = Destill(line); return true; }
            if (line.StartsWith("#DELIM=")) { DELIM = Destill(line); return true; }
            if (line.StartsWith("#DATE=")) { DATE = Destill(line); return true; }
            if (line.StartsWith("#OBSTYPE=")) { OBSTYPE = Destill(line); return true; }
            return false;
        }

        private string Destill(string line)
        {
            string[] split = line.Split("=");
            if (split.Length != 2) return string.Empty;
            return split[1];

        }

    }

    public class AAVSOList
    {
        #region Properties

        public string ID { get; private set; }
        public string NAME { get; set; }
        public string DATE { get; set; }
        public string MAG { get; set; }
        public string MERR { get; set; }
        public string FILT { get; set; }
        public string TRANS { get; set; }
        public string MTYPE { get; set; }
        public string CNAME { get; set; }
        public string CMAG { get; set; }
        public string KNAME { get; set; }
        public string KMAG { get; set; }
        public string AMASS { get; set; }
        public string GROUP { get; set; }
        public string CHART { get; set; }
        public string NOTES { get; set; }


        #endregion
        #region Ctor
        public AAVSOList()
        {
            ID = Guid.NewGuid().ToString();
            DATE = "na";
            MAG = "na";
            MERR = "na";
            FILT = "na";
            TRANS = "NO";
            MTYPE = "na";
            CNAME = "na";
            CMAG = "na";
            KNAME = "na";
            KMAG = "na";
            AMASS = "na";
            GROUP = "na";
            CHART = "na";
            NOTES = "na";
        }
        #endregion
    }
}