using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace SelectAndTranslate
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            bool createdNew = true;

            using (Mutex mutex = new Mutex(true, "SelectAndTranslate", out createdNew))
            {
                if (createdNew)
                {
                    var app = new App {StartupUri = new Uri("MainWindow.xaml", UriKind.Relative)};
                    app.Run();
                }
                else
                {
                    MessageBox.Show("The application is already running!","SelectAndTranslate");
                }
            }
        }
    }
}