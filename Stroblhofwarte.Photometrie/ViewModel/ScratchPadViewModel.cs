using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Stroblhofwarte.Image;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Stroblhofwarte.Photometrie.ViewModel
{
    public class ScratchPadViewModel : DockWindowViewModel
    {
        #region Properties

        public string ScratchPage
        {
            get
            {
                return ScratchPad.ScratchPad.Instance.GetPage(_page);
            }
        }

        private int _page = 0;
        public int Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged("Page");
                OnPropertyChanged("ScratchPage");
            }
        }

        #endregion

        #region Ctor

        public ScratchPadViewModel()
        {
            ScratchPad.ScratchPad.Instance.eventPadChanged += Instance_eventPadChanged;
            ScratchPad.ScratchPad.Instance.eventNewPageCreated += Instance_eventNewPageCreated;
        }

        private void Instance_eventNewPageCreated(object? sender, EventArgs e)
        {
            Page = ScratchPad.ScratchPad.Instance.CurrentPagePtr();
        }

        private void Instance_eventPadChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged("ScratchPage");
        }

        #endregion

        #region Commands

        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(param => this.Save(), param => this.CanSave());
                }
                return saveCommand;
            }
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ScratchPad file (*.pad)|*.pad";
            if (saveFileDialog.ShowDialog() == true)
            {
                ScratchPad.ScratchPad.Instance.Save(saveFileDialog.FileName);
            }
        }

        private RelayCommand loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (loadCommand == null)
                {
                    loadCommand = new RelayCommand(param => this.Load(), param => this.CanLoad());
                }
                return loadCommand;
            }
        }

        private bool CanLoad()
        {
            return true;
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ScratchPad file (*.pad)|*.pad";
            if (openFileDialog.ShowDialog() == true)
            {
                ScratchPad.ScratchPad.Instance.Load(openFileDialog.FileName);
                OnPropertyChanged("ScratchPage");
            }
        }

        private RelayCommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new RelayCommand(param => this.Clear(), param => this.CanClear());
                }
                return clearCommand;
            }
        }

        private bool CanClear()
        {
            return true;
        }

        private async void Clear()
        {
            var metroWindow = (Application.Current.MainWindow as MetroWindow);
            var result = await metroWindow.ShowMessageAsync("Clear ScratchPad",
                "All content of the ScratchPad will be lost!",
                MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Negative) return;
            ScratchPad.ScratchPad.Instance.Clear();
            Page = 0;
        }

        private RelayCommand commandPlus;
        public ICommand CommandPlus
        {
            get
            {
                if (commandPlus == null)
                {
                    commandPlus = new RelayCommand(param => this.Plus(), param => this.CanPlus());
                }
                return commandPlus;
            }
        }

        private bool CanPlus()
        {
            if (Page < ScratchPad.ScratchPad.Instance.CurrentPagePtr()) return true;
            return false;
        }

        private void Plus()
        {
            Page++;
        }

        private RelayCommand commandMinus;
        public ICommand CommandMinus
        {
            get
            {
                if (commandMinus == null)
                {
                    commandMinus = new RelayCommand(param => this.Minus(), param => this.CanMinus());
                }
                return commandMinus;
            }
        }

        private bool CanMinus()
        {
            if (Page > 0) return true;
            return false;
        }

        private async void Minus()
        {
            Page--;
        }
        #endregion
    }
}
