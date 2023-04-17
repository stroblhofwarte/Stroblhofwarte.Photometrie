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

        #endregion
    }
}
