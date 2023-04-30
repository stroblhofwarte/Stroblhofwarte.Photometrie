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

        private ObservableCollection<TrandformationEntry> _data;
        public ObservableCollection<TrandformationEntry> Data
        {
            get { return _data; }
        }

        private PlotModel _plotModel;
        public PlotModel Model
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

            MagMeasurement.Instance.NewMeasurement += Instance_NewMeasurement;

            Model = new PlotModel { Title = "Simple example", Subtitle = "using OxyPlot" };

            var series1 = new ScatterSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
            series1.Points.Add(new ScatterPoint(1, 1));
            series1.Points.Add(new ScatterPoint(2, 2));
            series1.Points.Add(new ScatterPoint(3, 3));
            series1.Points.Add(new ScatterPoint(4, 4));
            series1.Points.Add(new ScatterPoint(5, 5));

            Model.Series.Add(series1);
        }

        private void Instance_NewMeasurement(object? sender, EventArgs e)
        {
            MeasurementEventArgs args = e as MeasurementEventArgs;
            if (args == null) return;
            if (!args.IsMachine) return;
            _data.Add(new TrandformationEntry() { Meas = args.Mag });
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

        private double _index;
        public double Index
        {
            get { return _index; }
            set { _index = value; OnPropertyChanged("Index"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
