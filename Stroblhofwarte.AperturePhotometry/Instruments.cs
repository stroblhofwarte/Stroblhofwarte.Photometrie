﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Stroblhofwarte.AperturePhotometry
{
    public class Instruments
    {
        #region Singelton

        private static Instruments _instance = null;

        public static Instruments Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Instruments();
                return _instance;
            }
        }

        #endregion

        #region Properties

        private readonly string FILENAME = "Stroblhofwarte.Instruments.csv";

        private Dictionary<string, InstrumentObject> _hashCache = new Dictionary<string, InstrumentObject>();

        public List<InstrumentObject> InstrumentsList
        {
            get
            {
                return _hashCache.Values.ToList();
            }
        }

        #endregion

        #region Ctor

        public Instruments()
        {
            if(!Load())
                Save(); // Create new file
        }

        #endregion
        public bool Load()
        {
            try
            {

                _hashCache.Clear();
                string filename = FILENAME;
                if (Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath != string.Empty)
                    filename = Stroblhofwarte.Config.GlobalConfig.Instance.FilterDatabasePath + "\\" + FILENAME;
                string[] lines = File.ReadAllLines(filename);

                foreach (string line in lines)
                {
                    if (line.StartsWith("#")) continue;
                    string[] fields = line.Split(";");
                    if (fields.Length != 11) continue;
                    InstrumentObject obj = new InstrumentObject()
                    {
                        Telescope = fields[0],
                        Instrument = fields[1],
                        Gain = Convert.ToInt32(fields[2]),
                        Offset = Convert.ToInt32(fields[3]),
                        SetTemp = Convert.ToDouble(fields[4], CultureInfo.InvariantCulture),
                        Gain_e_ADU = Convert.ToDouble(fields[5], CultureInfo.InvariantCulture),
                        ReadOutNoise = Convert.ToDouble(fields[6], CultureInfo.InvariantCulture),
                        DarkCurrent = Convert.ToDouble(fields[7], CultureInfo.InvariantCulture),
                        Aperture = Convert.ToInt32(fields[8]),
                        InnerAnnulus = Convert.ToInt32(fields[9]),
                        OuterAnnulus = Convert.ToInt32(fields[10]),
                    };
                    if(!_hashCache.ContainsKey(Hash(obj)))
                    {
                        _hashCache.Add(Hash(obj), obj);
                    }

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
                    foreach (InstrumentObject instr in _hashCache.Values)
                    {
                        string line = instr.Telescope + ";" +
                            instr.Instrument + ";" +
                            instr.Gain.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.Offset.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.SetTemp.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.Gain_e_ADU.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.ReadOutNoise.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.DarkCurrent.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.Aperture.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.InnerAnnulus.ToString(CultureInfo.InvariantCulture) + ";" +
                            instr.OuterAnnulus.ToString(CultureInfo.InvariantCulture);

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

        public string Hash(InstrumentObject obj)
        {
            return obj.Telescope + ":" + obj.Instrument + ":" + obj.Gain.ToString(CultureInfo.InvariantCulture) + ":" + obj.Offset.ToString(CultureInfo.InvariantCulture) + ":" + obj.SetTemp.ToString(CultureInfo.InvariantCulture);
        }

        public InstrumentObject Get(string teleskope, string instrument, int gain, int offset, double settemp)
        {
            InstrumentObject obj = new InstrumentObject()
            {
                Telescope = teleskope,
                Instrument = instrument,
                Gain = Convert.ToInt32(gain),
                Offset = Convert.ToInt32(offset),
                SetTemp = Convert.ToDouble(settemp)
            };
            string hash = Hash(obj);
            if (_hashCache.ContainsKey(hash))
                return _hashCache[hash];
            return null;
        }

        public InstrumentObject Get(string hash)
        {
            if (_hashCache.ContainsKey(hash))
                return _hashCache[hash];
            return null;
        }

        public bool Update(string hash, double gain_e_ADU, double readoutnoise, double darkcurrent)
        {
            if(_hashCache.ContainsKey(hash))
            {
                _hashCache[hash].Gain_e_ADU = gain_e_ADU;
                _hashCache[hash].ReadOutNoise = readoutnoise;
                _hashCache[hash].DarkCurrent = darkcurrent;
                return true;
            }
            return false;
        }

        public bool Update(string hash, int aperture, int innerAnnulus, int outerAnnulus)
        {
            if (_hashCache.ContainsKey(hash))
            {
                _hashCache[hash].Aperture = aperture;
                _hashCache[hash].InnerAnnulus = innerAnnulus;
                _hashCache[hash].OuterAnnulus = outerAnnulus;
                return true;
            }
            return false;
        }

        public bool Create(string teleskope, string instrument, int gain, int offset, double setTemp, int aperture, int innerAnnulus, int outerAnnulus)
        {
            InstrumentObject obj = new InstrumentObject()
            {
                Telescope = teleskope,
                Instrument = instrument,
                Gain = Convert.ToInt32(gain),
                Offset = Convert.ToInt32(offset),
                SetTemp = Convert.ToDouble(setTemp),
                Aperture = Convert.ToInt32(aperture),
                InnerAnnulus = Convert.ToInt32(innerAnnulus),
                OuterAnnulus = Convert.ToInt32(outerAnnulus)
            };
            string hash = Hash(obj);
            if (_hashCache.ContainsKey(hash))
                return false;
            _hashCache.Add(hash, obj);
            Save();
            return true;
        }

        public bool Create(InstrumentObject instr)
        {
            InstrumentObject obj = new InstrumentObject()
            {
                Telescope = instr.Telescope,
                Instrument = instr.Instrument,
                Gain = instr.Gain,
                Offset = instr.Offset,
                SetTemp = instr.SetTemp,
                Aperture = instr.Aperture,
                InnerAnnulus = instr.InnerAnnulus,
                OuterAnnulus = instr.OuterAnnulus
            };
            string hash = Hash(obj);
            if (_hashCache.ContainsKey(hash))
                return false;
            _hashCache.Add(hash, obj);
            Save();
            return true;
        }
    }

    public class InstrumentObject
    {
        #region Properties

        public string Telescope { get; set; }
        public string Instrument { get; set; }
        public double Gain { get; set; }
        public double Offset { get; set; }
        public double SetTemp { get; set; }
        public double Gain_e_ADU { get; set; }
        public double ReadOutNoise { get; set; }
        public double DarkCurrent { get; set; }

        public double Aperture { get; set; }
        public double InnerAnnulus { get; set; }
        public double OuterAnnulus { get; set; }

        #endregion

    }
}

