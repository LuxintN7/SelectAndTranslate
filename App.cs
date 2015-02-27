using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SelectAndTranslate
{
    /// <summary>
    /// App
    /// </summary>
    class App : Application
    {
        /// <summary>
        /// InitializeComponent
        /// </summary>
        public void InitializeComponent()
        {
            this.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);

#line default
#line hidden
        }       

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main()
        {
            Process curentProcess = Process.GetCurrentProcess();

            if (Process.GetProcessesByName(curentProcess.ProcessName).Length > 1)
            {
                MessageBox.Show("Application is already running!");
                return;
            }
            else
            {
                SelectAndTranslate.App app = new SelectAndTranslate.App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
