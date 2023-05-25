#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"


using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stroblhofwarte.Photometrie.Dialogs;

namespace Stroblhofwarte.Photometrie.ViewModel
{
  public class MainViewModel
  {


    public DockManagerViewModel DockManagerViewModel { get; private set; }
    public MenuViewModel MenuViewModel { get; private set; }

    public MainViewModel()
    {
        var documents = new List<DockWindowViewModel>();

        documents.Add(new FileViewModel() { Title = "Files" });
        documents.Add(new ImageViewModel() { Title = "Image" });
        documents.Add(new AavsoViewModel() { Title = "AAVSO Data" });
        documents.Add(new MagnificationViewModel() { Title = "Magnification" });
        documents.Add(new ApertureViewModel() { Title = "Aperture View" });
        documents.Add(new ImageInfoViewModel() { Title = "Image Info" });
        documents.Add(new ReportViewModel() { Title = "Report" });
        documents.Add(new StandardFieldViewModel() { Title = "Standard Field" });
        documents.Add(new TransformationViewModel() { Title = "Transformation" });
        documents.Add(new ScratchPadViewModel() { Title = "ScratchPad" });
        documents.Add(new AutomatisationViewModel() { Title = "Automatisation" });

        this.DockManagerViewModel = new DockManagerViewModel(documents);
        this.MenuViewModel = new MenuViewModel(documents);
    }

        #region Commands
        private RelayCommand settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (settingsCommand == null)
                {
                    settingsCommand = new RelayCommand(param => this.Settings(), param => this.CanSettings());
                }
                return settingsCommand;
            }
        }

        private bool CanSettings()
        {
            return true;
        }

        private async void Settings()
        {
            DialogSettings dlg = new DialogSettings();
            dlg.ShowDialog();
        }

        private RelayCommand filterCommand;
        public ICommand FilterCommand
        {
            get
            {
                if (filterCommand == null)
                {
                    filterCommand = new RelayCommand(param => this.Filter(), param => this.CanFilter());
                }
                return filterCommand;
            }
        }

        private bool CanFilter()
        {
            return true;
        }

        private async void Filter()
        {
            DialogFilterSetup dlg = new DialogFilterSetup();
            dlg.ShowDialog();
        }

        private RelayCommand instrumentCommand;
        public ICommand InstrumentCommand
        {
            get
            {
                if (instrumentCommand == null)
                {
                    instrumentCommand = new RelayCommand(param => this.Instrument(), param => this.CanInstrument());
                }
                return instrumentCommand;
            }
        }

        private bool CanInstrument()
        {
            return true;
        }

        private async void Instrument()
        {
            DialogInstrumentView dlg = new DialogInstrumentView();
            dlg.ShowDialog();
        }

        private RelayCommand aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (aboutCommand == null)
                {
                    aboutCommand = new RelayCommand(param => this.About(), param => this.CanAbout());
                }
                return aboutCommand;
            }
        }

        private bool CanAbout()
        {
            return true;
        }

        private async void About()
        {
            AboutView dlg = new AboutView();
            dlg.ShowDialog();
        }


        #endregion
    }
}
