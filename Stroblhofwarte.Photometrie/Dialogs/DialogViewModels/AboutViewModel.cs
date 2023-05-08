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
