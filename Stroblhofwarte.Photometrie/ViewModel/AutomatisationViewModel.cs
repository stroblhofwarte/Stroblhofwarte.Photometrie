using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class AutomatisationViewModel : DockWindowViewModel
    {
        #region Properties

        public bool AutoImageLoad
        {
            get {  return AutomatisationHub.AutomatisationHub.Instance.AutomaticImageLoadEnabled; }
            set
            {
                AutomatisationHub.AutomatisationHub.Instance.AutomaticImageLoadEnabled = value;
                if (value) _autoPhotometryCanStart = false;
                OnPropertyChanged("AutoImageLoad");
            }
        }

        public bool AutoPhotometry
        {
            get { return AutomatisationHub.AutomatisationHub.Instance.AutomaticPhotometryEnabled; }
            set
            {
                AutomatisationHub.AutomatisationHub.Instance.AutomaticPhotometryEnabled = value;
                OnPropertyChanged("AutoPhotometry");
            }
        }

        public bool FileScan
        {
            get { return AutomatisationHub.AutomatisationHub.Instance.FileScanEnabled; }
            set
            {
                AutomatisationHub.AutomatisationHub.Instance.FileScanEnabled = value;
                AutomatisationHub.AutomatisationHub.Instance.AutomatedModeFinished();
                OnPropertyChanged("FileScan");
            }
        }

        private bool _autoPhotometryCanStart = false;
        private bool _cancel = false;
        #endregion

        #region Ctor

        public AutomatisationViewModel()
        {
            AutomatisationHub.AutomatisationHub.Instance.PhotometryDone += Instance_PhotometryDone;
            StroblhofwarteImage.Instance.NewImageLoaded += Instance_NewImageLoaded;
            AutomatisationHub.AutomatisationHub.Instance.Cancel += Instance_Cancel;
            AutomatisationHub.AutomatisationHub.Instance.NewFileArrived += Instance_NewFileArrived;
        }

        private void Instance_NewFileArrived(object? sender, EventArgs e)
        {
            _autoPhotometryCanStart = true;
            AutomatisationHub.AutomatisationHub.Instance.NextImage();
        }

        private void Instance_Cancel(object? sender, EventArgs e)
        {
            _cancel = true;
        }

        private void Instance_NewImageLoaded(object? sender, EventArgs e)
        {
            if(_cancel)
            {
                _cancel = false;
                AutomatisationHub.AutomatisationHub.Instance.AutomatedModeFinished();
                _autoPhotometryCanStart = false;
                return;
            }
            if(AutoPhotometry && _autoPhotometryCanStart)
            {
                AutomatisationHub.AutomatisationHub.Instance.StartPhotometry();
            }
        }

        private void Instance_PhotometryDone(object? sender, EventArgs e)
        {
            if (_cancel)
            {
                _cancel = false;
                AutomatisationHub.AutomatisationHub.Instance.AutomatedModeFinished();
                _autoPhotometryCanStart = false;
                return;
            }
            if(AutoImageLoad)
            {
                if(AutomatisationHub.AutomatisationHub.Instance.NextImageAvailabel)
                {
                    if (AutoPhotometry && _autoPhotometryCanStart == false)
                        _autoPhotometryCanStart = true;
                    AutomatisationHub.AutomatisationHub.Instance.NextImage();
                }
                else
                {
                    _autoPhotometryCanStart = false;
                    if(!AutomatisationHub.AutomatisationHub.Instance.FileScanEnabled)
                        AutomatisationHub.AutomatisationHub.Instance.AutomatedModeFinished();
                }
            }
        }

        #endregion
    }
}
