﻿using Stroblhofwarte.Photometrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Stroblhofwarte.Photometrie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /* create the window */
            var mainWindow = new MainWindow();
            this.MainWindow = mainWindow;

            /* attach the mainViewModel */
            var mainViewModel = new MainViewModel();
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }
}
