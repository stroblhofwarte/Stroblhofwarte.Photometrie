#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Stroblhofwarte.Photometrie.Dialogs.DialogViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        #region Properties

        private string _disclaimer;
        public string Disclaimer
        {
            get { return _disclaimer; }
            set
            {
                _disclaimer = value;
                OnPropertyChanged("Disclaimer");
            }
        }

        #endregion
        #region Ctor
        public AboutViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Stroblhofwarte.Photometrie.Disclaimer.txt";
            //string[] res = Assembly.GetCallingAssembly().GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                Disclaimer = reader.ReadToEnd();

            }
        }

        #endregion
    }

}
