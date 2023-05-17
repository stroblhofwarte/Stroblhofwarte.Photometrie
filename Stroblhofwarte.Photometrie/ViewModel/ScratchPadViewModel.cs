using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ScratchPadViewModel : DockWindowViewModel
    {
        #region Properties

        public string ScratchPage
        {
            get
            {
                return ScratchPad.ScratchPad.Instance.GetCurrentPage();
            }
        }

        #endregion

        #region Ctor

        public ScratchPadViewModel()
        {
            ScratchPad.ScratchPad.Instance.eventPadChanged += Instance_eventPadChanged;
        }

        private void Instance_eventPadChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged("ScratchPage");
        }

        #endregion
    }
}
