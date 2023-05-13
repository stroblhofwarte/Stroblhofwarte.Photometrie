#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Stroblhofwarte.Photometrie.DataPackages;
using Stroblhofwarte.Photometrie.FileFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ReportViewModel : DockWindowViewModel
    {
        #region Properties

        private ObservableCollection<AAVSOList> _values;
        public ObservableCollection<AAVSOList> Values
        {
            get { return _values; }
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
            if(o is AAVSOList)
            {
                AAVSOExtendedFileFormat.Instance.Delete((o as AAVSOList).ID);
            }
        }


        #endregion

        #region Ctor

        public ReportViewModel()
        {
            _values = new ObservableCollection<AAVSOList>();
            foreach (AAVSOList l in AAVSOExtendedFileFormat.Instance.Values)
            {
                _values.Add(l);
            }
            AAVSOExtendedFileFormat.Instance.DatabaseChanged += Instance_DatabaseChanged;
            ContentId = "ReportViewModel";
        }

        private void Instance_DatabaseChanged(object? sender, EventArgs e)
        {
            _values = new ObservableCollection<AAVSOList>();
            foreach (AAVSOList l in AAVSOExtendedFileFormat.Instance.Values)
            {
                _values.Add(l);
            }
            OnPropertyChanged("Values");
        }

        #endregion
    }
}
