using OxyPlot;
using OxyPlot.Series;
using Stroblhofwarte.AperturePhotometry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class TransformationViewModel : DockWindowViewModel
    {
        #region Properties

        private ScatterSeries _tx_yz_series;

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

        #endregion
        #region Ctor
        public TransformationViewModel()
        {
            _transformModels = new ObservableCollection<TransformModel>()
            {   new TransformModel() { FromFilter = "B", ToFilter = "V"},
                new TransformModel() { FromFilter = "U", ToFilter = "B"},
                new TransformModel() { FromFilter = "V", ToFilter = "R"},
                new TransformModel() { FromFilter = "R", ToFilter = "I"},
            };
            _data = new ObservableCollection<TrandformationEntry>();

            TransformDataSet.Instance.NewMeasurement += Instance_NewMeasurement;

            Model_Tx_yz = new PlotModel { Title = "Tx_yz", Subtitle = "(Magnitude Transformation)"};

            _tx_yz_series = new ScatterSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
            Model_Tx_yz.Series.Add(_tx_yz_series);
            
        }

        private void Instance_NewMeasurement(object? sender, EventArgs e)
        {
            
            _data.Add(new TrandformationEntry() { Id= TransformDataSet.Instance.Id,
                Meas = TransformDataSet.Instance.Mag,
                V = TransformDataSet.Instance.V,
                BV = TransformDataSet.Instance.BV,
                UB = TransformDataSet.Instance.UB,
                VR = TransformDataSet.Instance.VR,
                RI = TransformDataSet.Instance.RI
            });
            _tx_yz_series.Points.Add(new ScatterPoint(TransformDataSet.Instance.BV, TransformDataSet.Instance.Mag + 20 - TransformDataSet.Instance.V));
            Model_Tx_yz.InvalidatePlot(true);
        }
        #endregion
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
