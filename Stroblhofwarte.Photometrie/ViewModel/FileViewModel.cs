using Stroblhofwarte.FITS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class FileViewModel : DockWindowViewModel
    {
        #region Properties

        private string _myFolder;

        public string MyFolder
        {
            get {  return _myFolder;}
            set { _myFolder = value;
                OnPropertyChanged("MyFolder");
            }
        }

        private ObservableCollection<FitsFileModel> _files;

        public ObservableCollection<FitsFileModel> Files
        {
            get { 
                return _files; 
            }
            set
            {
                _files = value;
                OnPropertyChanged("Files");
            }
        }
        #endregion

        #region Commands

        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                {
                    openCommand = new RelayCommand(param => this.Open(), param => this.CanOpen());
                }
                return openCommand;
            }
        }

        private bool CanOpen()
        {
            return true;
        }

        private void Open()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            dlg.ValidateNames = false;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            // Always default to Folder Selection.
            dlg.FileName = "Folder Selection.";
            if (dlg.ShowDialog() == true)
            {
                Files.Clear();
                string folderPath = Path.GetDirectoryName(dlg.FileName);
                string[] files = Directory.GetFiles(folderPath);
                MyFolder = folderPath;
                foreach (string file in files)
                {
                    if(file.EndsWith(".fits"))
                    {
                        string wcs = "";
                        StroblhofwarteHEADER checkFile = new StroblhofwarteHEADER(file);
                        if (checkFile.WCS != null)
                            wcs = "yes";
                        string date = ParseFITSDate(checkFile.RawImage.DATE_LOC);
                        date = date.Replace("T", " ");
                        string pureFilename = Path.GetFileName(file);
                        Files.Add(new FitsFileModel() { Name = pureFilename, Date = date, 
                         WCS = wcs});
                    }
                }
            }
        }

        private string ParseFITSDate(string date)
        {
            string[] parts = date.Split('T');
            if (parts.Length != 2) return date;
            string dati = parts[0];
            string timi = parts[1];
            string[] timeFracts = timi.Split('.');
            string time = timeFracts[0];
            return dati + " " + time;
        }

        #endregion

        #region Ctor

        public FileViewModel()
        {
            _files = new ObservableCollection<FitsFileModel>();
        }

        #endregion
    }

    public class FitsFileModel
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public string WCS { get; set; }
    }
}
