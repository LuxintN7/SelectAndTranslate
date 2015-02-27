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
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
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
