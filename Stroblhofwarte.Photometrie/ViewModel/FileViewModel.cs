#region "copyright"

/*
    Copyright © 2023 Othmar Ehrhardt, https://astro.stroblhof-oberrohrbach.de

    This file is part of the Stroblhof.Photometrie application

    This Source Code Form is subject to the terms of the GNU General Public License 3.
    If a copy of the GPL v3.0 was not distributed with this
    file, You can obtain one at https://www.gnu.org/licenses/gpl-3.0.de.html.
*/

#endregion "copyright"

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Stroblhofwarte.Astap.Solver;
using Stroblhofwarte.Astrometry.Solver.AstrometryNet;
using Stroblhofwarte.FITS;
using Stroblhofwarte.Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class FileViewModel : DockWindowViewModel
    {
        #region Properties

        public FileSystemWatcher _fileWatcher;
        private readonly MemoryCache _memCache;
        private readonly CacheItemPolicy _cacheItemPolicy;
        private const int CacheTimeMilliseconds = 1000;

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
                string folderPath = System.IO.Path.GetDirectoryName(dlg.FileName);
                string[] files = Directory.GetFiles(folderPath);
                MyFolder = folderPath;

                Task.Factory.StartNew(() =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        WaitAnimation = Visibility.Visible;
                    }));

                    foreach (string file in files)
                    {
                        if (file.EndsWith(".fits") || file.EndsWith(".fit"))
                        {
                            AddFile(file);
                        }
                    }

                    // Create a file watcher to detect when new images arrive:
                    if(_fileWatcher != null)
                    {
                        _fileWatcher.Changed -= _fileWatcher_Changed;
                    }

                    _fileWatcher = new FileSystemWatcher();
                    _fileWatcher.Path = folderPath;
                    _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                    _fileWatcher.Filter = "*.fits";
                    _fileWatcher.Changed += _fileWatcher_Changed;
                    _fileWatcher.EnableRaisingEvents = true;
                    
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        WaitAnimation = Visibility.Hidden;
                    }));

                });
            }

        }

        private bool AddFile(string fullPath)
        {
            try
            {
                string wcs = "";
                StroblhofwarteHEADER checkFile = new StroblhofwarteHEADER(fullPath);
              
                if (checkFile.WCS != null)
                    wcs = "yes";
                string date = ParseFITSDate(checkFile.RawImage.DATE_LOC);
                date = date.Replace("T", " ");
                string pureFilename = System.IO.Path.GetFileName(fullPath);
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Files.Add(new FitsFileModel()
                    {
                        Name = pureFilename,
                        Date = date,
                        WCS = wcs
                    });
                }));
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        // FileSystemWatcher fires more than once. At the beginning of the copy process, at the end, sometimes also in between.
        // If the code reactes on the first event, the copy process is not completed and a incomplete file is loaded. 
        // For this a  cache item is created and after one second the file is accessed.
        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!AutomatisationHub.AutomatisationHub.Instance.FileScanEnabled)
                return;
            string pureFilename = System.IO.Path.GetFileName(e.FullPath);
            foreach(FitsFileModel f in Files)
            {
                if(f.Name == pureFilename)
                {
                    // File was loaded before. Do nothing.
                    return;
                }
            }
            _cacheItemPolicy.AbsoluteExpiration =
            DateTimeOffset.Now.AddMilliseconds(CacheTimeMilliseconds);
            _memCache.AddOrGetExisting(e.Name, e, _cacheItemPolicy);
        }

        private void OnRemovedFromCache(CacheEntryRemovedArguments args)
        {
            if (args.RemovedReason != CacheEntryRemovedReason.Expired) return;
            var e = (FileSystemEventArgs)args.CacheItem.Value;
            if (AddFile(e.FullPath))
                AutomatisationHub.AutomatisationHub.Instance.NewImageArrived();
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
                // Stupid: Replace with base class. 
                AstrometryNet astrometry = new AstrometryNet(Config.GlobalConfig.Instance.AstrometrynetHost, Config.GlobalConfig.Instance.AstrometrynetKey);
                AstapSolver astap = new AstapSolver(Config.GlobalConfig.Instance.AstapExe, Config.GlobalConfig.Instance.AstapArgs);
                foreach (FitsFileModel file in _files)
                {
                    if (file.WCS == "yes") continue;
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        file.State = "solving...";
                        OnPropertyChanged("Files");
                    }));

                    bool solveResult = false;
                    if (Config.GlobalConfig.Instance.Astrometry)
                        solveResult = await astrometry.Solve(MyFolder + "\\" + file.Name);
                    if (Config.GlobalConfig.Instance.Astap)
                        solveResult = await astap.Solve(MyFolder + "\\" + file.Name);
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

        private void NewImage(object param)
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
                file.Loaded = false;
            }
            item.State = "loaded";
            item.Loaded = true;
            int currentImageIdx = -1;
            int i = 0;
            foreach (FitsFileModel file in _files)
            {
                if (file.Loaded == true)
                {
                    currentImageIdx = i;
                    break;
                }
                i++;
            }
            if (currentImageIdx < _files.Count - 1)
            {
                AutomatisationHub.AutomatisationHub.Instance.NextImageAvailabel = true;
            }
            WaitAnimation = Visibility.Hidden;
        }

        

        #endregion

        #region Ctor

        public FileViewModel()
        {
            _files = new ObservableCollection<FitsFileModel>();
            ContentId = "FileViewModel";
            AutomatisationHub.AutomatisationHub.Instance.NextImageRequest += Instance_NextImageRequest;

            _memCache = MemoryCache.Default;
            _cacheItemPolicy = new CacheItemPolicy()
            {
                RemovedCallback = OnRemovedFromCache
            };
        }


        private void Instance_NextImageRequest(object? sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                int currentImageIdx = -1;
                int i = 0;
                foreach (FitsFileModel file in _files)
                {
                    if (file.Loaded == true)
                    {
                        currentImageIdx = i;
                        break;
                    }
                    i++;
                }
                if (currentImageIdx == -1 && _files.Count > 0)
                {
                    // nothing loaded yet, load the first image
                    StroblhofwarteImage.Instance.Load(MyFolder + "\\" + _files[0].Name);
                    _files[0].State = "loaded";
                    _files[0].Loaded = true;
                    return;
                }
                if (currentImageIdx > -1 && currentImageIdx < _files.Count)
                {
                    // the last image was not reached yet, load the next one:
                    _files[currentImageIdx].State = "";
                    _files[currentImageIdx].Loaded = false;
                    _files[currentImageIdx + 1].State = "loaded";
                    _files[currentImageIdx + 1].Loaded = true;
                    StroblhofwarteImage.Instance.Load(MyFolder + "\\" + _files[currentImageIdx + 1].Name);
                }

                //Inform the world if more images are availabel:
                if (currentImageIdx == _files.Count - 2)
                {
                    AutomatisationHub.AutomatisationHub.Instance.NextImageAvailabel = false;
                }
                else
                {
                    AutomatisationHub.AutomatisationHub.Instance.NextImageAvailabel = true;
                }
            });
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

        public bool Loaded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
