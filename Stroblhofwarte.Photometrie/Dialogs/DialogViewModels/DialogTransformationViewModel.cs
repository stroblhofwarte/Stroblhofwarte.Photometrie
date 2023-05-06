using Stroblhofwarte.AperturePhotometry;
using Stroblhofwarte.Image;
using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

        #region Commands

        #endregion

        #region Ctor
        public DialogTransformationViewModel()
        {

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
    }
}
