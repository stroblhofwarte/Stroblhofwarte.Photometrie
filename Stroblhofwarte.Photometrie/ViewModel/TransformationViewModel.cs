using ControlzEx.Standard;
using OxyPlot;
using OxyPlot.Series;
using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.FileFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class TransformationViewModel : DockWindowViewModel
    {
        #region Constant

        private double _MACHINE_MAG_C = 20.0;

        #endregion

        #region Properties

        private ScatterSeries _tx_yz_series;
        private LineSeries _tx_yz_fit;

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
                OnPropertyChanged("Model");
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
        }
        public string MagErrorParameterName
        {
            get
            {
                string errorColor = "";
                if (ColorMagErrorFilter.Filter == "U") errorColor = "u";
                if (ColorMagErrorFilter.Filter == "B") errorColor = "b";
                if (ColorMagErrorFilter.Filter == "V") errorColor = "v";
                if (ColorMagErrorFilter.Filter == "R") errorColor = "r";
                if (ColorMagErrorFilter.Filter == "I") errorColor = "i";

                string index = "";
                if (TransfromModelSelection.FromFilter == "U" && TransfromModelSelection.ToFilter == "B") index = "ub";
                if (TransfromModelSelection.FromFilter == "B" && TransfromModelSelection.ToFilter == "V") index = "bv";
                if (TransfromModelSelection.FromFilter == "V" && TransfromModelSelection.ToFilter == "R") index = "vr";
                if (TransfromModelSelection.FromFilter == "R" && TransfromModelSelection.ToFilter == "I") index = "ri";

                return "T" + errorColor + "__" + index;
            }
        }

        private double _magErrorSlope;
        public double MagErrorSlope
        {
            get { return _magErrorSlope;  }
            set {
                _magErrorSlope = value;
                OnPropertyChanged("MagErrorParameterName");
                OnPropertyChanged("MagErrorSlope");
            }
        }

        #endregion

        #region Ctor
        public TransformationViewModel()
        {
            _transformModels = new ObservableCollection<TransformModel>()
            {   new TransformModel() { FromFilter = "B", ToFilter = "V"},
                new TransformModel() { FromFilter = "U", ToFilter = "B"},
                new TransformModel() { FromFilter = "V", ToFilter = "R"},
                new TransformModel() { FromFilter = "R", ToFilter = "I"}
            };
            _data = new ObservableCollection<TrandformationEntry>();

            TransformDataSet.Instance.NewMeasurement += Instance_NewMeasurement;

            Model_Tx_yz = new PlotModel { Title = "Tx_yz", Subtitle = "(Magnitude Transformation)"};

            _tx_yz_series = new ScatterSeries { Title = "Magnitudes", MarkerType = MarkerType.Circle };
            _tx_yz_fit = new LineSeries { Title = "Transform slop", MarkerType = MarkerType.Circle };
            Model_Tx_yz.Series.Add(_tx_yz_series);
            Model_Tx_yz.Series.Add(_tx_yz_fit);
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
                ScatterPoint p = new ScatterPoint(indice, t.Meas + _MACHINE_MAG_C - t.V);
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
                    double indice = ColorIndexFromData(_transfromModel, d);

                    x[idx] = indice;
                    y[idx++] = d.Meas + _MACHINE_MAG_C - d.V;
                    if (indice > xmax)
                        xmax = indice;
                }
                double rSuqared = 0.0;
                double yIntersect = 0.0;
                double slope = 0.0;
                LinearRegression.FitLinear(x, y, 0, _data.Count, out rSuqared, out yIntersect, out slope);
                MagErrorSlope = slope;
                _tx_yz_fit.Points.Clear();
                _tx_yz_fit.Points.Add(new DataPoint(0, yIntersect));
                _tx_yz_fit.Points.Add(new DataPoint(xmax, yIntersect + (slope * xmax)));

            }
            Model_Tx_yz.InvalidatePlot(true);
        }
        private double ColorIndexFromData(TransformModel filter, TrandformationEntry dataline)
        {
            double indice = 0.0;
            if (filter.FromFilter == "U" && filter.ToFilter == "B") indice = dataline.UB;
            if (filter.FromFilter == "B" && filter.ToFilter == "V") indice = dataline.BV;
            if (filter.FromFilter == "V" && filter.ToFilter == "R") indice = dataline.VR;
            if (filter.FromFilter == "R" && filter.ToFilter == "I") indice = dataline.RI;
            return indice;
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
}
