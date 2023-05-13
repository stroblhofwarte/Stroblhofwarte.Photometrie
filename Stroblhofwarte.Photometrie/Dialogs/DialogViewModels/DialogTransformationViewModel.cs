#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.DataPackages;
using Stroblhofwarte.Photometrie.FileFormats;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.Dialogs.DialogViewModels
{
    public class DialogTransformationViewModel : BaseViewModel
    {

        #region Properties

        private ObservableCollection<ApertureMeasurementEntry> _measures;
        public ObservableCollection<ApertureMeasurementEntry> Measures 
        { 
            set 
            {
                _measures = value;
                string left = string.Empty;
                string right = string.Empty;
                if(ValidateData(out left, out right))
                {
                    LeftFilter = left;
                    RightFilter = right;
                    IsError = TransformationParameter();
                    Transform();
                    StdDeviation();
                }
                else
                {
                    IsError = true;
                }
            } 
            private get 
            { 
                return _measures; 
            } 
        }

        private ObservableCollection<TransformedEntry> _transformedMeas;
        public ObservableCollection<TransformedEntry> TransformedMeas
        {
            get { return _transformedMeas; }
            set {
                _transformedMeas = value;
                OnPropertyChanged("TransformedMeas");
            }
        }

        private ObservableCollection<TransformedEntry> _combinedTransformedMeas;
        public ObservableCollection<TransformedEntry> CombinedTransformedMeas
        {
            get { return _combinedTransformedMeas; }
            set
            {
                _combinedTransformedMeas = value;
                OnPropertyChanged("CombinedTransformedMeas");
            }
        }

        private bool _combinedMeasure;
        public bool CombinedMeasure
        {
            get { return _combinedMeasure; }
            set
            {
                _combinedMeasure = value;
                OnPropertyChanged("CombinedMeasure");
            }
        }

        private bool _isError;
        public bool IsError
        {
            set
            {
                _isError = value;
                OnPropertyChanged("IsError");
            }
            get
            {
                return _isError;
            }
        }

        private string _error;
        public string Error
        {
            set 
            { 
                _error = value;
                OnPropertyChanged("Error");
            }
            get
            {
                return _error;
            }
        }

        private string _leftFilterSmall;
        private string _rightFilterSmall;


        private string _txyName;
        public string TxyName
        {
            set
            {
                _txyName = value;
                OnPropertyChanged("TxyName");
            }
            get
            {
                return _txyName;
            }
        }

        private string _righttxyzName;
        public string RighttxyzName
        {
            set
            {
                _righttxyzName = value;
                OnPropertyChanged("RighttxyzName");
            }
            get
            {
                return _righttxyzName;
            }
        }

        private string _lefttxyzName;
        public string LefttxyzName
        {
            set
            {
                _lefttxyzName = value;
                OnPropertyChanged("LefttxyzName");
            }
            get
            {
                return _lefttxyzName;
            }
        }


        private string _leftFilter;
        public string LeftFilter
        {
            set
            {
                _leftFilter = value;
                OnPropertyChanged("LeftFilter");
            }
            get
            {
                return _leftFilter;
            }
        }

        private string _rightFilter;
        public string RightFilter
        {
            set
            {
                _rightFilter = value;
                OnPropertyChanged("RightFilter");
            }
            get
            {
                return _rightFilter;
            }
        }

        private double _Txy;
        public double Txy
        {
            set
            {
                _Txy = value;
                OnPropertyChanged("Txy");
            }
            get
            {
                return _Txy;
            }
        }

        private double _leftTx_yz;
        public double LeftTx_yz
        {
            set
            {
                _leftTx_yz = value;
                OnPropertyChanged("LeftTx_yz");
            }
            get
            {
                return _leftTx_yz;
            }
        }

        private double _rightTx_yz;
        public double RightTx_yz
        {
            set
            {
                _rightTx_yz = value;
                OnPropertyChanged("RightTx_yz");
            }
            get
            {
                return _rightTx_yz;
            }
        }

        private double _leftstd;
        public double LeftStd
        {
            set
            {
                _leftstd = value;
                OnPropertyChanged("LeftStd");
            }
            get
            {
                return _leftstd;
            }
        }

        private double _leftKstd;
        public double LeftKStd
        {
            set
            {
                _leftKstd = value;
                OnPropertyChanged("LeftKStd");
            }
            get
            {
                return _leftKstd;
            }
        }

        private double _rightstd;
        public double RightStd
        {
            set
            {
                _rightstd = value;
                OnPropertyChanged("RightStd");
            }
            get
            {
                return _rightstd;
            }
        }

        private double _rightKstd;
        public double RightKStd
        {
            set
            {
                _rightKstd = value;
                OnPropertyChanged("RightKStd");
            }
            get
            {
                return _rightKstd;
            }
        }

        private bool _report = true;

        #endregion

        #region Commands

        #region commandReportSerie
        private RelayCommand commandReportSerie;
        public ICommand CommandReportSerie
        {
            get
            {
                if (commandReportSerie == null)
                {
                    commandReportSerie = new RelayCommand(param => this.ReportSerie(), param => this.CanReportSerie());
                }
                return commandReportSerie;
            }
        }

        private bool CanReportSerie()
        {
            return _report;
        }

        private void ReportSerie()
        {
            foreach (TransformedEntry t in TransformedMeas)
            {
                AAVSOList newElement;
                string leftNote = "VMAGINS=" + t.LeftInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                   "|CMAGINS=" + t.LeftCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                   "|KMAGINS=" + t.LeftCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                   "|" + t.LeftTx_yz_Name + "=" + t.LeftTx_yz.ToString(CultureInfo.InvariantCulture) +
                   "|" + t.Txy_Name + "=" + t.Txy.ToString(CultureInfo.InvariantCulture);
                AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                    t.LeftJD.ToString("0.#####", CultureInfo.InvariantCulture),
                    t.LeftMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftErr.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftFilter,
                    "YES",
                    "STD",
                    StarDataRelay.Instance.CompName,
                    t.LeftCompInstrumentMag.ToString("0.####", CultureInfo.InvariantCulture),
                    StarDataRelay.Instance.CheckName,
                    t.LeftCheckInstrumentMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftAirmass.ToString("0.####", CultureInfo.InvariantCulture),
                    "na",
                    StarDataRelay.Instance.ChartId,
                    leftNote, out newElement);

                string rightNote = "VMAGINS=" + t.RightInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|CMAGINS=" + t.RightCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|KMAGINS=" + t.RightCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.RightTx_yz_Name + "=" + t.RightTx_yz.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.Txy_Name + "=" + t.Txy.ToString(CultureInfo.InvariantCulture);
                AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                    t.RightJD.ToString("0.#####", CultureInfo.InvariantCulture),
                    t.RightMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightErr.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightFilter,
                    "YES",
                    "STD",
                    StarDataRelay.Instance.CompName,
                    t.RightCompInstrumentMag.ToString("0.####", CultureInfo.InvariantCulture),
                    StarDataRelay.Instance.CheckName,
                    t.RightCheckInstrumentMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightAirmass.ToString("0.####", CultureInfo.InvariantCulture),
                    "na",
                    StarDataRelay.Instance.ChartId,
                    rightNote, out newElement);
            }
            _report = false;
        }
        #endregion

        #region commandReportCombined
        private RelayCommand commandReportCombined;
        public ICommand CommandReportCombined
        {
            get
            {
                if (commandReportCombined == null)
                {
                    commandReportCombined = new RelayCommand(param => this.ReportCombined(), param => this.CanReportCombined());
                }
                return commandReportCombined;
            }
        }

        private bool CanReportCombined()
        {
            return _report;
        }

        private void ReportCombined()
        {
            foreach (TransformedEntry t in CombinedTransformedMeas)
            {
                AAVSOList newElement;
                string leftNote = "VMAGINS=" + t.LeftInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|CMAGINS=" + t.LeftCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|KMAGINS=" + t.LeftCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.LeftTx_yz_Name + "=" + t.LeftTx_yz.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.Txy_Name + "=" + t.Txy.ToString(CultureInfo.InvariantCulture);
                AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                    t.LeftJD.ToString("0.#####", CultureInfo.InvariantCulture),
                    t.LeftMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftErr.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftFilter,
                    "YES",
                    "STD",
                    StarDataRelay.Instance.CompName,
                    t.LeftCompMag.ToString("0.####", CultureInfo.InvariantCulture),
                    StarDataRelay.Instance.CheckName,
                    t.LeftCheckMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.LeftAirmass.ToString("0.####", CultureInfo.InvariantCulture),
                    "na",
                    StarDataRelay.Instance.ChartId,
                    leftNote, out newElement);

                string rightNote = "VMAGINS=" + t.RightInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|CMAGINS=" + t.RightCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|KMAGINS=" + t.RightCheckInstrumentMag.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.RightTx_yz_Name + "=" + t.RightTx_yz.ToString(CultureInfo.InvariantCulture) +
                    "|" + t.Txy_Name + "=" + t.Txy.ToString(CultureInfo.InvariantCulture);

                AAVSOExtendedFileFormat.Instance.Add(StarDataRelay.Instance.Name,
                    t.RightJD.ToString("0.#####", CultureInfo.InvariantCulture),
                    t.RightMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightErr.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightFilter,
                    "YES",
                    "STD",
                    StarDataRelay.Instance.CompName,
                    t.RightCompMag.ToString("0.####", CultureInfo.InvariantCulture),
                    StarDataRelay.Instance.CheckName,
                    t.RightCheckMag.ToString("0.####", CultureInfo.InvariantCulture),
                    t.RightAirmass.ToString("0.####", CultureInfo.InvariantCulture),
                    "na",
                    StarDataRelay.Instance.ChartId,
                    rightNote, out newElement);
            }
            _report = false;
        }
        #endregion
        #endregion

        #region Ctor
        public DialogTransformationViewModel()
        {
            _transformedMeas = new ObservableCollection<TransformedEntry>();
            _combinedTransformedMeas = new ObservableCollection<TransformedEntry>();
        }
        #endregion

        private bool ValidateData(out string left, out string right)
        {
            List<string> filters = new List<string>();
            left = string.Empty;
            right = string.Empty;
            foreach(ApertureMeasurementEntry e in Measures)
            {
                if (!filters.Contains(e.Filter))
                    filters.Add(e.Filter);
            }
            if(filters.Count() != 2)
            {
                Error = "Only two different filters are allowed for Transformation!";
                return false;
            }
            if (filters.Contains("U") && filters.Contains("B"))
            {
                left = "U";
                right = "B";
                _leftFilterSmall = "u";
                _rightFilterSmall = "b";
                return true;
            }
            if (filters.Contains("B") && filters.Contains("V"))
            {
                left = "B";
                right = "V";
                _leftFilterSmall = "b";
                _rightFilterSmall = "v";
                return true;
            }
            if (filters.Contains("V") && filters.Contains("RJ"))
            {
                left = "V";
                right = "RJ";
                _leftFilterSmall = "v";
                _rightFilterSmall = "r";
                return true;
            }
            if (filters.Contains("RJ") && filters.Contains("IJ"))
            {
                left = "RJ";
                right = "IJ";
                _leftFilterSmall = "r";
                _rightFilterSmall = "i";
                return true;
            }
            return false;
        }

        private bool TransformationParameter()
        {
            InstrumentObject instr = AperturePhotometry.Instruments.Instance.Get(StroblhofwarteImage.Instance.GetTelescope(), StroblhofwarteImage.Instance.GetInstrument(),
               (int)StroblhofwarteImage.Instance.GetSensorGain(),
               (int)StroblhofwarteImage.Instance.GetSensorOffset(),
               StroblhofwarteImage.Instance.GetBinning(),
               StroblhofwarteImage.Instance.GetSensorSetTemp());
            string hash = Stroblhofwarte.AperturePhotometry.Instruments.Instance.Hash(instr);
            string strTxy = "T" + _leftFilterSmall + _rightFilterSmall;
            string strLeftTx_yz = "T" + _leftFilterSmall + "_" + _leftFilterSmall + _rightFilterSmall;
            string strRightTx_yz = "T" + _rightFilterSmall + "_" + _leftFilterSmall + _rightFilterSmall;

            TxyName = strTxy;
            LefttxyzName = strLeftTx_yz;
            RighttxyzName = strRightTx_yz;

            double dTxy = Stroblhofwarte.AperturePhotometry.Instruments.Instance.GetTransformationParam(hash, strTxy);
            double dleftTx_yz = Stroblhofwarte.AperturePhotometry.Instruments.Instance.GetTransformationParam(hash, strLeftTx_yz);
            double drightTx_yz = Stroblhofwarte.AperturePhotometry.Instruments.Instance.GetTransformationParam(hash, strRightTx_yz);
            if (double.IsNaN(dTxy) || double.IsNaN(dleftTx_yz) || double.IsNaN(drightTx_yz))
            {
                Error = "Not all required parameters are there: ";
                if (double.IsNaN(dTxy)) Error += strTxy + ": not found;";
                if (double.IsNaN(dleftTx_yz)) Error += strLeftTx_yz + ": not found;";
                if (double.IsNaN(drightTx_yz)) Error += strRightTx_yz + ": not found;";
                return false;
            }
            Txy = dTxy;
            LeftTx_yz = dleftTx_yz;
            RightTx_yz = drightTx_yz;
            return true;
        }

        private bool Transform()
        {
            List<ApertureMeasurementEntry> leftSideMeas = new List<ApertureMeasurementEntry>();
            List<ApertureMeasurementEntry> rightSideMeas = new List<ApertureMeasurementEntry>();

            foreach (ApertureMeasurementEntry e in Measures)
            {
                if (e.Filter == LeftFilter)
                    leftSideMeas.Add(e);
                if (e.Filter == RightFilter)
                    rightSideMeas.Add(e);
            }
            foreach(ApertureMeasurementEntry e in leftSideMeas)
            {
                foreach (ApertureMeasurementEntry ee in rightSideMeas)
                {
                    TransformedEntry t = new TransformedEntry()
                    {
                        Name = e.Name,
                        LeftFilter = LeftFilter,
                        RightFilter = RightFilter,
                        Txy = Txy,
                        LeftTx_yz = LeftTx_yz,
                        RightTx_yz = RightTx_yz,
                        LeftCompInstrumentMag = e.CompMachineMag,
                        LeftCheckInstrumentMag = e.KMachineMag,
                        Txy_Name = _txyName,
                        LeftTx_yz_Name = _lefttxyzName,
                        RightTx_yz_Name = _righttxyzName,
                        LeftCompMag = e.CompMag,
                        LeftInstrumentMag = e.MachineMag,
                        LeftJD = e.JD,
                        LeftAirmass = e.Airmass
                    };

                    t.LeftMag = LeftSideTransform(e, ee);
                    t.RightMag = RightSideTransform(e, ee);
                    t.LeftCheckMag = LeftSideCheckTransform(e, ee);
                    t.RightCheckMag = RightSideCheckTransform(e, ee);
                    t.RightCompInstrumentMag = ee.CompMachineMag;
                    t.RightCheckInstrumentMag = ee.KMachineMag;
                    t.RightCompMag = ee.CompMag;
                    t.RightInstrumentMag = ee.MachineMag;
                    t.RightJD = ee.JD;
                    t.RightAirmass = ee.Airmass;
                    TransformedMeas.Add(t);
                }
                
            }
            return true;
        }

        private double LeftSideTransform(ApertureMeasurementEntry leftMeas, ApertureMeasurementEntry rightMeas)
        {
            double bc = leftMeas.CompMachineMag;
            double vc = rightMeas.CompMachineMag;
            double bt = leftMeas.MachineMag;
            double vt = rightMeas.MachineMag;
            double Bc = leftMeas.CompMag;
            // For left side (more blue filter):
            return -LeftTx_yz * Txy * (bc - vc - bt + vt) + Bc - bc + bt;
        }

        private double LeftSideCheckTransform(ApertureMeasurementEntry leftMeas, ApertureMeasurementEntry rightMeas)
        {
            double bc = leftMeas.CompMachineMag;
            double vc = rightMeas.CompMachineMag;
            double bt = leftMeas.KMachineMag;
            double vt = rightMeas.KMachineMag;
            double Bc = leftMeas.CompMag;
            // For left side (more blue filter):
            return -LeftTx_yz * Txy * (bc - vc - bt + vt) + Bc - bc + bt;
        }

        private double RightSideTransform(ApertureMeasurementEntry leftMeas, ApertureMeasurementEntry rightMeas)
        {
            double bc = leftMeas.CompMachineMag;
            double vc = rightMeas.CompMachineMag;
            double bt = leftMeas.MachineMag;
            double vt = rightMeas.MachineMag;
            double Vc = rightMeas.CompMag;
            // For left side (more blue filter):
            return -RightTx_yz * Txy * (bc - vc - bt + vt) + Vc - vc + vt;
        }

        private double RightSideCheckTransform(ApertureMeasurementEntry leftMeas, ApertureMeasurementEntry rightMeas)
        {
            double bc = leftMeas.CompMachineMag;
            double vc = rightMeas.CompMachineMag;
            double bt = leftMeas.KMachineMag;
            double vt = rightMeas.KMachineMag;
            double Vc = rightMeas.CompMag;
            // For left side (more blue filter):
            return -RightTx_yz * Txy * (bc - vc - bt + vt) + Vc - vc + vt;
        }

        private bool StdDeviation()
        {
            // Doppelt hält besser! 
            List<ApertureMeasurementEntry> leftSideMeas = new List<ApertureMeasurementEntry>();
            List<ApertureMeasurementEntry> rightSideMeas = new List<ApertureMeasurementEntry>();

            foreach (ApertureMeasurementEntry e in Measures)
            {
                if (e.Filter == LeftFilter)
                    leftSideMeas.Add(e);
                if (e.Filter == RightFilter)
                    rightSideMeas.Add(e);
            }

            // Calculate all averages:
            double leftavrg = 0.0;
            double leftKavrg = 0.0;
            foreach (ApertureMeasurementEntry t in leftSideMeas)
            {
                leftavrg += t.MachineMag;
                leftKavrg += t.KMachineMag;
            }
            leftavrg = leftavrg / leftSideMeas.Count();
            leftKavrg = leftKavrg / leftSideMeas.Count();

            double rightavrg = 0.0;
            double rightKavrg = 0.0;
            foreach (ApertureMeasurementEntry t in rightSideMeas)
            {
                rightavrg += t.MachineMag;
                rightKavrg += t.KMachineMag;
            }
            rightavrg = rightavrg / rightSideMeas.Count();
            rightKavrg = rightKavrg / rightSideMeas.Count();

            // Calculate the quadrat sums:
            double leftsum = 0.0;
            double leftKsum = 0.0;
            foreach (ApertureMeasurementEntry t in leftSideMeas)
            {
                leftsum += (t.MachineMag - leftavrg) * (t.MachineMag - leftavrg);
                leftKsum += (t.KMachineMag - leftKavrg) * (t.KMachineMag - leftKavrg);
            }
            double rightsum = 0.0;
            double rightKsum = 0.0;
            foreach (ApertureMeasurementEntry t in rightSideMeas)
            {
                rightsum += (t.MachineMag - rightavrg) * (t.MachineMag - rightavrg);
                rightKsum += (t.KMachineMag - rightKavrg) * (t.KMachineMag - rightKavrg);
            }

            LeftStd = Math.Sqrt(leftsum / (leftSideMeas.Count() - 1));
            LeftKStd = Math.Sqrt(leftKsum / (leftSideMeas.Count() - 1));

            RightStd = Math.Sqrt(rightsum / (rightSideMeas.Count() - 1));
            RightKStd = Math.Sqrt(rightKsum / (rightSideMeas.Count() - 1));

            if (leftSideMeas.Count < 3)
            {
                LeftStd = leftSideMeas[0].MagErr;
                LeftKStd = leftSideMeas[0].MagErr;
            }
            if (rightSideMeas.Count < 3)
            {
                RightStd = rightSideMeas[0].MagErr;
                RightKStd = rightSideMeas[0].MagErr;
            }

            // Calculate combined roport (one line per filter)
            double leftvaravrg = 0.0;
            double leftvarinstravrg = 0.0;
            double leftcavrg = 0.0;
            double leftkavrg = 0.0;
            double rightvaravrg = 0.0;
            double rightvarinstravrg = 0.0;
            double rightcavrg = 0.0;
            double rightkavrg = 0.0;
            List<TransformedEntry> leftSide = new List<TransformedEntry>();
            List<TransformedEntry> rightSide = new List<TransformedEntry>();

            // Split for combined measurement 
            // and add error values to multi measurement:
            foreach (TransformedEntry t in TransformedMeas)
            {
                t.LeftErr = LeftKStd;
                t.RightErr = RightKStd;

                if (t.LeftFilter == LeftFilter)
                    leftSide.Add(t);
                if (t.RightFilter == RightFilter)
                    rightSide.Add(t);
            }
            TransformedEntry combined = new TransformedEntry();
            foreach (TransformedEntry t in leftSide)
            {
                leftvaravrg += t.LeftMag;
                leftvarinstravrg += t.LeftInstrumentMag;
                leftcavrg += t.LeftCompInstrumentMag;
                leftkavrg += t.LeftCheckInstrumentMag;
            }
            foreach (TransformedEntry t in rightSide)
            {
                rightvaravrg += t.RightMag;
                rightvarinstravrg += t.RightInstrumentMag;
                rightcavrg += t.RightCompInstrumentMag;
                rightkavrg += t.RightCheckInstrumentMag;
            }
            leftvaravrg = leftvaravrg / leftSide.Count();
            leftvarinstravrg = leftvarinstravrg / leftSide.Count();
            leftcavrg = leftcavrg / leftSide.Count();
            leftkavrg = leftkavrg / leftSide.Count();
            rightvaravrg = rightvaravrg / rightSide.Count();
            rightvarinstravrg = rightvarinstravrg / rightSide.Count();
            rightcavrg = rightcavrg / rightSide.Count();
            rightkavrg = rightkavrg / rightSide.Count();

            combined.Name = TransformedMeas[0].Name;
            combined.LeftFilter = LeftFilter;
            combined.RightFilter = RightFilter;
            combined.LeftCompMag = leftcavrg;
            combined.LeftCheckMag = leftkavrg;
            combined.LeftCompInstrumentMag = leftcavrg;
            combined.LeftCheckInstrumentMag = leftkavrg;
            combined.LeftMag = leftvaravrg;
            combined.RightCompMag = rightcavrg;
            combined.RightCheckMag = rightkavrg;
            combined.RightCompInstrumentMag = rightcavrg;
            combined.RightCheckInstrumentMag = rightkavrg;
            combined.RightMag = rightvaravrg;
            combined.RightInstrumentMag = rightvarinstravrg;
            combined.LeftInstrumentMag = leftvarinstravrg;
            combined.LeftErr = LeftStd;
            combined.RightErr = RightStd;
            combined.Txy_Name = _txyName;
            combined.LeftTx_yz_Name = _lefttxyzName;
            combined.RightTx_yz_Name = _righttxyzName;
            combined.Txy = Txy;
            combined.LeftTx_yz = LeftTx_yz;
            combined.RightTx_yz = RightTx_yz;
            combined.LeftJD = leftSideMeas[0].JD;
            combined.RightJD = rightSideMeas[0].JD;
            combined.LeftAirmass = leftSideMeas[0].Airmass;
            combined.RightAirmass = rightSideMeas[0].Airmass;

            CombinedTransformedMeas.Add(combined);
            return true;
        }
    }

    public class TransformedEntry : INotifyPropertyChanged
    {
        #region Properties

        public string Name { get; set; }
        public double Txy { get; set; }
        public double LeftTx_yz { get; set; }
        public double RightTx_yz { get; set; }
        public string LeftFilter { get; set; }
        public string RightFilter { get; set; }
        public double LeftMag { get; set; }
        public double RightMag { get; set; }
        public double LeftInstrumentMag { get; set; }
        public double RightInstrumentMag { get; set; }
        public double LeftCheckMag { get; set; }
        public double RightCheckMag { get; set; }
        public double LeftCompMag { get; set; }
        public double RightCompMag { get; set; }
        public double LeftCompInstrumentMag { get; set; }
        public double RightCompInstrumentMag { get; set; }
        public double LeftCheckInstrumentMag { get; set; }
        public double RightCheckInstrumentMag { get; set; }
        public double LeftErr { get; set; }
        public double RightErr { get; set; }

        public double LeftJD { get; set; }
        public double RightJD { get; set; }

        public double LeftAirmass { get; set; }
        public double RightAirmass { get; set; }



        private string _Txy_Name;
        public string Txy_Name { 
            get 
            {
                return _Txy_Name;
            } 
            set 
            {
                _Txy_Name = value;
                OnPropertyChanged("Txy_Name");
            } 
        }

        private string _LeftTx_yz_Name;
        public string LeftTx_yz_Name
        {
            get
            {
                return _LeftTx_yz_Name;
            }
            set
            {
                _LeftTx_yz_Name = value;
                OnPropertyChanged("LeftTx_yz_Name");
            }
        }

        private string _RightTx_yz_Name;
        public string RightTx_yz_Name
        {
            get
            {
                return _RightTx_yz_Name;
            }
            set
            {
                _RightTx_yz_Name = value;
                OnPropertyChanged("RightTx_yz_Name");
            }
        }

        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
