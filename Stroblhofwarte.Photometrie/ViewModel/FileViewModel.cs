using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Stroblhofwarte.Astrometry.Solver.AstrometryNet;
using Stroblhofwarte.FITS;
using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private Visibility _waitAnimation = Visibility.Hidden;
        public Visibility WaitAnimation
        {
            get { return _waitAnimation; }
            set
            {
                _waitAnimation = value;
                OnPropertyChanged("WaitAnimation");
            }
        }

        private FitsFileModel _selectedFile;
        public FitsFileModel SelectedFile
        {
            get { 
                return _selectedFile;
            }
            set { _selectedFile = value;
                OnPropertyChanged("SelectedFile");
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

        private async void Open()
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

                await Task.Factory.StartNew(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        WaitAnimation = Visibility.Visible;
                    }));

                    foreach (string file in files)
                    {
                        if (file.EndsWith(".fits"))
                        {
                            string wcs = "";
                            StroblhofwarteHEADER checkFile = new StroblhofwarteHEADER(file);
                            if (checkFile.WCS != null)
                                wcs = "yes";
                            string date = ParseFITSDate(checkFile.RawImage.DATE_LOC);
                            date = date.Replace("T", " ");
                            string pureFilename = Path.GetFileName(file);
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                Files.Add(new FitsFileModel()
                                {
                                    Name = pureFilename,
                                    Date = date,
                                    WCS = wcs
                                });
                            }));
                            
                        }
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        WaitAnimation = Visibility.Hidden;
                    }));

                });
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

        private RelayCommand solveCommand;
        public ICommand SolveCommand
        {
            get
            {
                if (solveCommand == null)
                {
                    solveCommand = new RelayCommand(param => this.Solve(), param => this.CanSolve());
                }
                return solveCommand;
            }
        }

        private bool CanSolve()
        {
            foreach(FitsFileModel file in _files)
            {
                if (file.WCS != "yes") return true;
            }
            return false;
        }

        private async void Solve()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var result = await metroWindow.ShowMessageAsync("Image Solver", 
                "All images without valid WCS will be solved via Astrometry.net and the orginal fille will be obverwritten with the new file with WCS data:",
                MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Negative) return;
            await Task.Factory.StartNew(async () =>
            {
                AstrometryNet solver = new AstrometryNet(Properties.Settings.Default.AstrometrynetHost, Properties.Settings.Default.AstrometrynetKey);
                foreach (FitsFileModel file in _files)
                {
                    if (file.WCS == "yes") continue;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        file.State = "solving...";
                        OnPropertyChanged("Files");
                    }));

                    bool solveResult = await solver.Solve(MyFolder + "\\" + file.Name);
                    if (!solveResult)
                    {
                        file.State = "error";
                        OnPropertyChanged("Files");
                    }
                    else
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            file.State = "success";
                            file.WCS = "yes";
                            OnPropertyChanged("Files");
                        }));
                    }
                }
            });
        }

        private RelayCommand _selectNewImage;
        public ICommand SelectNewImage
        {
            get
            {
                if (_selectNewImage == null)
                {
                    _selectNewImage = new RelayCommand(param => this.NewImage(param), param => this.CanNewImage());
                }
                return _selectNewImage;
            }
        }

        private bool CanNewImage()
        {
            return true;
        }

        private async void NewImage(object param)
        {
            FitsFileModel item = param as FitsFileModel;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                WaitAnimation = Visibility.Visible;
            }));

            StroblhofwarteImage.Instance.Load(MyFolder + "\\" + item.Name);
            foreach(FitsFileModel file in _files)
            {
                if(file.State == "loaded") file.State = "";
            }
            item.State = "loaded";
            WaitAnimation = Visibility.Hidden;
        }

        

        #endregion

        #region Ctor

        public FileViewModel()
        {
            _files = new ObservableCollection<FitsFileModel>();
            ContentId = "FileViewModel";
        }

        #endregion
    }

    public class FitsFileModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }
        private string _date;
        public string Date
        {
            get { return _date; }
            set { _date = value; OnPropertyChanged("Date"); }
        }


        private string _wcs;
        public string WCS
        {
            get { return _wcs; }
            set { _wcs = value; OnPropertyChanged("WCS"); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged("State"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
