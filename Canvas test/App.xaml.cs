﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Canvas_test
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            if (main.logger.isConnected)
            {
                main.logger.EndConnection(); 
            }
            Environment.Exit(0);
        }
    }
}
