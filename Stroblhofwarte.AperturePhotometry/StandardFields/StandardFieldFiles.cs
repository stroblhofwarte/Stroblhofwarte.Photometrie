using Stroblhofwarte.FITS.DataObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.AperturePhotometry.StandardFields
{
    public class StandardFieldFiles
    {
        #region Properties

        #endregion

        #region Ctor

        #endregion

        public List<string> Fields()
        {
            List<string> fields = new List<string>();
            string path = Stroblhofwarte.Config.GlobalConfig.Instance.StandardFieldsPath;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                fields.Add(Path.GetFileName(file));
            }
            return fields;
        }

        public List<StandardStar> StandardField(string id)
        {
            List<StandardStar> stars = new List<StandardStar>();
            List<string> fields = Fields();
            foreach(string field in fields)
            {
                string fieldpath = Filename(field);
                StandardFieldHeader h = HeaderFrom(field);
                if(h.Id == id)
                {
                    Dictionary<string, int> slots = new Dictionary<string, int>();
                    bool tabelheader = false;
                    string[] lines = System.IO.File.ReadAllLines(fieldpath);
                    foreach(string line in lines)
                    {
                        if (line.StartsWith("#")) continue;
                        // Find the tabel names:
                        if(tabelheader == false)
                        {
                            tabelheader = true;
                            string[] names = line.Split(';');
                            int idx = 0;
                            foreach(string name in names)
                            {
                                if(!slots.ContainsKey(name))
                                {
                                    slots.Add(name, idx);
                                }
                                idx++;
                            }
                        }
                        else
                        {
                            StandardStar star = new StandardStar();
                            string[] values = line.Split(';');
                            double U = 0.0;
                            double B = 0.0;
                            double V = 0.0;
                            double R = 0.0;
                            double I = 0.0;
                            try
                            {
                                if (slots.ContainsKey("U")) U = Convert.ToDouble(values[slots["U"]], CultureInfo.InvariantCulture);
                                if (slots.ContainsKey("B")) B = Convert.ToDouble(values[slots["B"]], CultureInfo.InvariantCulture);
                                if (slots.ContainsKey("V")) V = Convert.ToDouble(values[slots["V"]], CultureInfo.InvariantCulture);
                                if (slots.ContainsKey("Rc")) R = Convert.ToDouble(values[slots["Rc"]], CultureInfo.InvariantCulture);
                                if (slots.ContainsKey("Ic")) I = Convert.ToDouble(values[slots["Ic"]], CultureInfo.InvariantCulture);
                                star.BV = B - V;
                                star.UB = U - B;
                                star.VR = V - R;
                                star.RI = R - I;
                                star.V = V;
                                if (slots.ContainsKey("AUID")) star.AUID = values[slots["AUID"]];
                                if (slots.ContainsKey("RA")) star.Ra = AstroUtil.HMSToDegrees(values[slots["RA"]]);
                                if (slots.ContainsKey("DEC")) star.DEC = AstroUtil.DMSToDegrees(values[slots["DEC"]]);
                                if (slots.ContainsKey("Label")) star.Id = values[slots["Label"]];
                                stars.Add(star);

                            }
                            catch (Exception ex)
                            {
                                // Invalid format
                            }
                        }
                    }
                }
            }
            return stars;
        }

        public StandardFieldHeader HeaderFrom(string filename)
        {
            string path = Filename(filename);
            string[] lines = System.IO.File.ReadAllLines(path);
            StandardFieldHeader header = new StandardFieldHeader();
            foreach (string line in lines)
            {
                if (!line.StartsWith("#")) continue;
                string[] fields = line.Split('=');
                if (fields[0] == "#Id") header.Id = fields[1];
                if (fields[0] == "#Name") header.Name = fields[1];
                if (fields[0] == "#RA") header.RA = fields[1];
                if (fields[0] == "#DEC") header.DEC = fields[1];
                if (fields[0] == "#N") header.N = fields[1];
            }
            return header;
        }

        private string Filename(string file)
        {
            string path = file;
            if (Stroblhofwarte.Config.GlobalConfig.Instance.StandardFieldsPath != string.Empty)
                path = Stroblhofwarte.Config.GlobalConfig.Instance.StandardFieldsPath + "\\" + file;
            return path;
        }
    }

    public class StandardFieldHeader
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RA { get; set; }
        public string DEC { get; set; }
        public string N { get; set; }




    }
}
