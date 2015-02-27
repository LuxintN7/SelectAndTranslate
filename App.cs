using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SelectAndTranslate
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var curentProcess = Process.GetCurrentProcess();

            if (Process.GetProcessesByName(curentProcess.ProcessName).Length > 1)
            {
                MessageBox.Show("Application is already running!");
                return;
            }

            var app = new App {StartupUri = new Uri("MainWindow.xaml", UriKind.Relative)};
            app.Run();
        }
    }
}