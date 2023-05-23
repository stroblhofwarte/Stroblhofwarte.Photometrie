#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using ControlzEx.Standard;
using OxyPlot;
using OxyPlot.Series;
using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.DataPackages;
using Stroblhofwarte.Photometrie.FileFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class TransformationViewModel : DockWindowViewModel
    {
        #region Constant

        private double _MACHINE_MAG_C = 0.0;

        #endregion

        #region Properties

        private ScatterSeries _tx_yz_series;
        private LineSeries _tx_yz_fit;
        private double _tx_yz_err;
        public double Tx_yzErr
        {
            get { 
                return _tx_yz_err; 
            }
            set
            {
                _tx_yz_err = value;
                OnPropertyChanged("Tx_yzErr");
            }
        }

        private ScatterSeries _txy_series;
        private LineSeries _txy_fit;
        private double _txy_err;
        public double TxyErr
        {
            get { return _txy_err; }
            set
            {
                _txy_err = value;
                OnPropertyChanged("TxyErr");
            }
        }

        private bool _tx_yzChanged = false;
        private bool _txyChanged = false;


        private ObservableCollection<ColorMagErrorEntry> _colorMagErrorFilters = new ObservableCollection<ColorMagErrorEntry>();
        public ObservableCollection<ColorMagErrorEntry> ColorMagErrorFilters
        {
            get { return _colorMagErrorFilters; }
        }

        private ObservableCollection<TrandformationEntry> _data;
        public ObservableCollection<TrandformationEntry> Data
        {
            get { return _data; }
        }

        private PlotModel _plotModel;
        public PlotModel Model_Tx_yz
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                OnPropertyChanged("Model_Tx_yz");
            }
        }

        private PlotModel _plotModelTxy;
        public PlotModel Model_Txy
        {
            get { return _plotModelTxy; }
            set
            {
                _plotModelTxy = value;
                OnPropertyChanged("Model_Txy");
            }
        }

        private ObservableCollection<TransformModel> _transformModels;
        public ObservableCollection<TransformModel> TransfromModels
        {
            get { return _transformModels; }
            set { 
                _transformModels = value;
                OnPropertyChanged("TransfromModels");
            }
        }

        private TransformModel _transfromModel;
        public TransformModel TransfromModelSelection
        {
            get { return _transfromModel; }
            set
            {
                _transfromModel = value;
                UpdateTx_yz();
                UpdateTxy();
                OnPropertyChanged("TransfromModelSelection");
            }
        }

        private ColorMagErrorEntry _colorMagErrorFilter;
        public ColorMagErrorEntry ColorMagErrorFilter
        {
            get { return _colorMagErrorFilter; }
            set
            {
                _colorMagErrorFilter = value;
                UpdateTx_yz();
                UpdateTxy();
                OnPropertyChanged("ColorMagErrorFilter");
            }
        }

        #endregion

        #region Commands

        private RelayCommand commandDel;
        public ICommand CommandDel
        {
            get
            {
                if (commandDel == null)
                {
                    commandDel = new RelayCommand(param => this.Del(param), param => this.CanDel());
                }
                return commandDel;
            }
        }

        private bool CanDel()
        {
            return true;
        }

        private void Del(object o)
        {
            TrandformationEntry t = o as TrandformationEntry;
            if (t == null) return;
            int idx = -1;
            int i = 0;
            foreach(TrandformationEntry te in _data)
            {
                if(te.Ident == t.Ident)
                {
                    idx = i;
                    break;
                }
                i++;
            }
            if(idx != -1)
            {
                _data.RemoveAt(idx);
            }
            UpdateTx_yz();
            UpdateTxy();
        }

        private RelayCommand commandAddTx_yz;
        public ICommand CommandAddTx_yz
        {
            get
            {
                if (commandAddTx_yz == null)
                {
                    commandAddTx_yz = new RelayCommand(param => this.AddTx_yz(param), param => this.CanAddTx_yz());
                }
                return commandAddTx_yz;
            }
        }

        private bool CanAddTx_yz()
        {
            return _tx_yzChanged;
        }

        private void AddTx_yz(object o)
        {
            string newTelescope = StroblhofwarteImage.Instance.GetTelescope();
            string newInstrument = StroblhofwarteImage.Instance.GetInstrument();
            double newGain = StroblhofwarteImage.Instance.GetSensorGain();
            double newOffset = StroblhofwarteImage.Instance.GetSensorOffset();
            string newBinning = StroblhofwarteImage.Instance.GetBinning();
            double newSetTemp = StroblhofwarteImage.Instance.GetSensorSetTemp();
            InstrumentObject newobj = new InstrumentObject()
            {
                Telescope = newTelescope,
                Instrument = newInstrument,
                Gain = newGain,
                Offset = newOffset,
                Binning = newBinning,
                SetTemp = newSetTemp
            };
            string hash = Stroblhofwarte.AperturePhotometry.Instruments.Instance.Hash(newobj);

            Stroblhofwarte.AperturePhotometry.Instruments.Instance.AddOrUpdateTransformationParameter(hash, MagErrorParameterName, MagErrorSlope);
            Stroblhofwarte.AperturePhotometry.Instruments.Instance.AddOrUpdateTransformationParameter(hash, MagErrorParameterName+"Err", Tx_yzErr);
            _tx_yzChanged = false;
        }

        private RelayCommand commandAddTxy;
        public ICommand CommandAddTxy
        {
            get
            {
                if (commandAddTxy == null)
                {
                    commandAddTxy = new RelayCommand(param => this.AddTxy(param), param => this.CanAddTxy());
                }
                return commandAddTxy;
            }
        }

        private bool CanAddTxy()
        {
            return _txyChanged;
        }

        private void AddTxy(object o)
        {
            string newTelescope = StroblhofwarteImage.Instance.GetTelescope();
            string newInstrument = StroblhofwarteImage.Instance.GetInstrument();
            double newGain = StroblhofwarteImage.Instance.GetSensorGain();
            double newOffset = StroblhofwarteImage.Instance.GetSensorOffset();
            string newBinning = StroblhofwarteImage.Instance.GetBinning();
            double newSetTemp = StroblhofwarteImage.Instance.GetSensorSetTemp();
            InstrumentObject newobj = new InstrumentObject()
            {
                Telescope = newTelescope,
                Instrument = newInstrument,
                Gain = newGain,
                Offset = newOffset,
                Binning = newBinning,
                SetTemp = newSetTemp
            };
            string hash = Stroblhofwarte.AperturePhotometry.Instruments.Instance.Hash(newobj);

            Stroblhofwarte.AperturePhotometry.Instruments.Instance.AddOrUpdateTransformationParameter(hash, ColorErrorParameterName, ColorErrorSlope);
            Stroblhofwarte.AperturePhotometry.Instruments.Instance.AddOrUpdateTransformationParameter(hash, ColorErrorParameterName + "Err", TxyErr);
            _txyChanged = false;
        }


        public string MagErrorParameterName
        {
            get
            {
                try
                {
                    string errorColor = "";
                    if (ColorMagErrorFilter.Filter == "U") errorColor = "u";
                    if (ColorMagErrorFilter.Filter == "B") errorColor = "b";
                    if (ColorMagErrorFilter.Filter == "V") errorColor = "v";
                    if (ColorMagErrorFilter.Filter == "RJ") errorColor = "r";
                    if (ColorMagErrorFilter.Filter == "I") errorColor = "i";

                    string index = "";
                    if (TransfromModelSelection.FromFilter == "U" && TransfromModelSelection.ToFilter == "B") index = "ub";
                    if (TransfromModelSelection.FromFilter == "B" && TransfromModelSelection.ToFilter == "V") index = "bv";
                    if (TransfromModelSelection.FromFilter == "V" && TransfromModelSelection.ToFilter == "RJ") index = "vr";
                    if (TransfromModelSelection.FromFilter == "RJ" && TransfromModelSelection.ToFilter == "I") index = "ri";

                    return "T" + errorColor + "_" + index;
                } catch (Exception ex)
                {
                    return "";
                }
            }
        }

        private double _magErrorSlope;
        public double MagErrorSlope
        {
            get { return _magErrorSlope;  }
            set {
                _tx_yzChanged = true;
                _magErrorSlope = value;
                OnPropertyChanged("MagErrorParameterName");
                OnPropertyChanged("MagErrorSlope");
            }
        }

        public string ColorErrorParameterName
        {
            get
            {
                try
                {
                    string index = "";
                    if (TransfromModelSelection.FromFilter == "U" && TransfromModelSelection.ToFilter == "B") index = "ub";
                    if (TransfromModelSelection.FromFilter == "B" && TransfromModelSelection.ToFilter == "V") index = "bv";
                    if (TransfromModelSelection.FromFilter == "V" && TransfromModelSelection.ToFilter == "RJ") index = "vr";
                    if (TransfromModelSelection.FromFilter == "RJ" && TransfromModelSelection.ToFilter == "I") index = "ri";

                    return "T" + index;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        private double _colorErrorSlope;
        public double ColorErrorSlope
        {
            get { return _colorErrorSlope; }
            set
            {
                _txyChanged = true;
                _colorErrorSlope = value;
                OnPropertyChanged("ColorErrorParameterName");
                OnPropertyChanged("ColorErrorSlope");
            }
        }
        #endregion

        #region Ctor
        public TransformationViewModel()
        {
            _transformModels = new ObservableCollection<TransformModel>()
            {   new TransformModel() { FromFilter = "B", ToFilter = "V"},
                new TransformModel() { FromFilter = "U", ToFilter = "B"},
                new TransformModel() { FromFilter = "V", ToFilter = "RJ"},
                new TransformModel() { FromFilter = "RJ", ToFilter = "I"}
            };
            _data = new ObservableCollection<TrandformationEntry>();

            TransformDataSet.Instance.NewMeasurement += Instance_NewMeasurement;

            Model_Tx_yz = new PlotModel { Title = "Tx_yz", Subtitle = "(Magnitude Transformation)"};
            Model_Txy = new PlotModel { Title = "Txy", Subtitle = "(Measured Color Index)" };

            _tx_yz_series = new ScatterSeries { Title = "Magnitudes", MarkerType = MarkerType.Circle };
            _tx_yz_fit = new LineSeries { Title = "Transform slop", MarkerType = MarkerType.Circle };
            Model_Tx_yz.Series.Add(_tx_yz_series);
            Model_Tx_yz.Series.Add(_tx_yz_fit);

            _txy_series = new ScatterSeries { Title = "Color Index", MarkerType = MarkerType.Circle };
            _txy_fit = new LineSeries { Title = "Transform slop", MarkerType = MarkerType.Circle };
            Model_Txy.Series.Add(_txy_series);
            Model_Txy.Series.Add(_txy_fit);
        }

        private void Instance_NewMeasurement(object? sender, EventArgs e)
        {
            string filter = Stroblhofwarte.AperturePhotometry.Filter.Instance.TranslateToAAVSOFilter(StroblhofwarteImage.Instance.GetFilter());
            _data.Add(new TrandformationEntry() { Id= TransformDataSet.Instance.Id,
                Meas = TransformDataSet.Instance.Mag,
                V = TransformDataSet.Instance.V,
                BV = TransformDataSet.Instance.BV,
                UB = TransformDataSet.Instance.UB,
                VR = TransformDataSet.Instance.VR,
                RI = TransformDataSet.Instance.RI,
                Filter = filter
            });

            ColorMagErrorEntry cme = new ColorMagErrorEntry() { Filter = filter };
            if(!_colorMagErrorFilters.Contains(cme))
            {
                _colorMagErrorFilters.Add(cme);
                if (_colorMagErrorFilters.Count == 1)
                {
                    // First add -> select this filter as color mag error
                    ColorMagErrorFilter = cme;
                }
                
                OnPropertyChanged("ColorMagErrorFilters");
            }
            UpdateTx_yz();
            UpdateTxy();
        }
        #endregion
        private void UpdateTx_yz()
        {
            _tx_yz_series.Points.Clear();
            foreach (TrandformationEntry t in _data)
            {
                if (t.Filter != ColorMagErrorFilter.Filter) 
                    continue;
                double indice = ColorIndexFromData(_transfromModel, t);
                double corr = ColorCorrValue(_transfromModel, t);
                ScatterPoint p = new ScatterPoint(indice, t.Meas - _MACHINE_MAG_C - (t.V + corr));
                _tx_yz_series.Points.Add(p);
            }

            if (_data.Count > 5)
            {
                double[] x = new double[_data.Count];
                double[] y = new double[_data.Count];
                int idx = 0;
                double xmax = double.MinValue;
                foreach (TrandformationEntry d in _data)
                {
                    if (d.Filter != ColorMagErrorFilter.Filter)
                        continue;
                    double indice = ColorIndexFromData(_transfromModel, d);
                    double corr = ColorCorrValue(_transfromModel, d);

                    x[idx] = indice;
                    y[idx++] = d.Meas + _MACHINE_MAG_C - (d.V + corr);
                    if (indice > xmax)
                        xmax = indice;
                }
                double rSuqared = 0.0;
                double yIntersect = 0.0;
                double slope = 0.0;
                LinearRegression.FitLinear(x, y, 0, idx, out rSuqared, out yIntersect, out slope);
                MagErrorSlope = slope;
                _tx_yz_fit.Points.Clear();
                _tx_yz_fit.Points.Add(new DataPoint(0, yIntersect));
                _tx_yz_fit.Points.Add(new DataPoint(xmax, yIntersect + (slope * xmax)));
                Tx_yzErr = StdDiv(yIntersect, slope, _tx_yz_series);
            }
            Model_Tx_yz.InvalidatePlot(true);
        }

        private double StdDiv(double yIntersect, double slope, ScatterSeries data)
        {
            double qdrsum = 0.0;
            foreach(ScatterPoint pnt in data.Points) 
            {
                double avrg = pnt.X * slope + yIntersect;
                qdrsum += (pnt.Y - avrg) * (pnt.Y - avrg);
            }
            return Math.Sqrt(qdrsum / (data.Points.Count - 1));
        }
        private double ColorIndexFromData(TransformModel filter, TrandformationEntry dataline)
        {
            try
            {
                double indice = 0.0;
                if (filter.FromFilter == "U" && filter.ToFilter == "B") indice = dataline.UB;
                if (filter.FromFilter == "B" && filter.ToFilter == "V") indice = dataline.BV;
                if (filter.FromFilter == "V" && filter.ToFilter == "RJ") indice = dataline.VR;
                if (filter.FromFilter == "RJ" && filter.ToFilter == "I") indice = dataline.RI;
                return indice;
            } catch (Exception ex)
            {
                return 0.0;
            }
        }

        private double ColorCorrValue(TransformModel filter, TrandformationEntry dataline)
        {
            try
            {
                double corr = 0.0;
                if (filter.FromFilter == "U" && filter.ToFilter == "B" && dataline.Filter == "U") corr = dataline.UB + dataline.BV;
                if (filter.FromFilter == "U" && filter.ToFilter == "B" && dataline.Filter == "B") corr = dataline.BV;
                if (filter.FromFilter == "B" && filter.ToFilter == "V" && dataline.Filter == "B") corr = dataline.BV;
                if (filter.FromFilter == "B" && filter.ToFilter == "V" && dataline.Filter == "V") corr = 0.0;
                if (filter.FromFilter == "V" && filter.ToFilter == "RJ" && dataline.Filter == "V") corr = 0.0;
                if (filter.FromFilter == "V" && filter.ToFilter == "RJ" && dataline.Filter == "RJ") corr = - dataline.VR;
                return corr;
            }
            catch (Exception ex)
            {
                return 0.0;
            }
        }

        private void UpdateTxy()
        {
            _txy_series.Points.Clear();
            Dictionary<string, MeasuredColorIndex> measIdx = new Dictionary<string, MeasuredColorIndex>();
            foreach (TrandformationEntry t in _data)
            {
                if(!measIdx.ContainsKey(t.Id))
                {
                    MeasuredColorIndex idx = new MeasuredColorIndex() { Id = t.Id };
                    idx.Add(t.Filter, t.Meas);
                    idx.Tag = t;
                    measIdx.Add(idx.Id, idx);
                }
                else
                {
                    measIdx[t.Id].Add(t.Filter, t.Meas);
                }
            }
            double xmax = double.MinValue;
            int i = 0;
            double[] x = new double[_data.Count];
            double[] y = new double[_data.Count];
            foreach (MeasuredColorIndex idx in measIdx.Values)
            {
                double left = double.NaN;
                double right = double.NaN;

                if (idx.MeasValues.ContainsKey(_transfromModel.FromFilter))
                    left = idx.MeasValues[_transfromModel.FromFilter];
                if (idx.MeasValues.ContainsKey(_transfromModel.ToFilter))
                    right = idx.MeasValues[_transfromModel.ToFilter];
                if (double.IsNaN(left) || double.IsNaN(right))
                    continue;
                double index = left - right;
                double indice = ColorIndexFromData(_transfromModel, idx.Tag);
                ScatterPoint p = new ScatterPoint(indice, index);
                x[i] = indice;
                y[i++] = index;
                if (indice > xmax)
                    xmax = indice;
                _txy_series.Points.Add(p);
            }
            if (measIdx.Count > 5)
            {
                double rSuqared = 0.0;
                double yIntersect = 0.0;
                double slope = 0.0;
                LinearRegression.FitLinear(x, y, 0, i, out rSuqared, out yIntersect, out slope);
                ColorErrorSlope = 1.0/slope;
                _txy_fit.Points.Clear();
                _txy_fit.Points.Add(new DataPoint(0, yIntersect));
                _txy_fit.Points.Add(new DataPoint(xmax, yIntersect + (slope * xmax)));
                TxyErr = StdDiv(yIntersect, slope, _txy_series);
            }
            Model_Txy.InvalidatePlot(true);
        }
    }

    
    
    public class TransformModel
    {
        public string FromFilter { get; set; }
        public string ToFilter { get; set; }
        public override string ToString()
        {
            return FromFilter + "-" + ToFilter;
        }
    }

    public class TrandformationEntry : INotifyPropertyChanged
    {
        private string _id;
        public string Id { get { return _id; } set { _id = value; OnPropertyChanged("Id"); } }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; OnPropertyChanged("Filter"); } 
        }

        private double _meas;
        public double Meas
        {
            get { return _meas; }
            set { _meas = value; OnPropertyChanged("Meas"); }
        }

        private double _V;
        public double V
        {
            get { return _V; }
            set { _V = value; OnPropertyChanged("V"); }
        }

        private double _BV;
        public double BV
        {
            get { return _BV; }
            set { _BV = value; OnPropertyChanged("BV"); }
        }

        private double _UB;
        public double UB
        {
            get { return _UB; }
            set { _UB = value; OnPropertyChanged("UB"); }
        }

        private double _VR;
        public double VR
        {
            get { return _VR; }
            set { _VR = value; OnPropertyChanged("VR"); }
        }

        private double _RI;
        public double RI
        {
            get { return _RI; }
            set { _RI = value; OnPropertyChanged("RI"); }
        }

        private Guid _ident;
        public Guid Ident
        {
            get { return _ident; }
        }

        

        #region Ctor

        public TrandformationEntry()
        {
            _ident = Guid.NewGuid();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ColorMagErrorEntry : INotifyPropertyChanged
    {
        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; OnPropertyChanged("Filter"); }
        }

        #region Ctor


        #endregion

        public override bool Equals(object obj)
        {
            ColorMagErrorEntry f = obj as ColorMagErrorEntry;
            if (f == null) return false;
            return f.Filter == Filter;
        }

        public override int GetHashCode()
        {
            return Filter.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Filter;
        }

    }

    public class MeasuredColorIndex 
    {
        public string Id { get; set; }
        public TrandformationEntry Tag { get; set; }
        public Dictionary<string, double> MeasValues { get; set; }

        public MeasuredColorIndex()
        {
            MeasValues = new Dictionary<string, double>();
        }

        public void Add(string filter, double machineMag)
        {
            if(!MeasValues.ContainsKey(filter))
            {
                MeasValues.Add(filter, machineMag);
            }
            else
            {
                MeasValues[filter] = machineMag;
            }
        }
    }
}
