using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

namespace SelectAndTranslate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public WinAPI.KeyboardHook Hook;
        public AppNotifyIcon AppNotifyIcon;         
        private TranslationWindow translationWindow = new TranslationWindow();

        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            InitializeComponent();            
            AppNotifyIcon = new AppNotifyIcon(this);
            Hook = new WinAPI.KeyboardHook(translationWindow.hookAction);
            Hook.SetHook();
            this.Visibility = Visibility.Hidden;
        }

        internal void Exit()
        {
            this.Dispose();
            System.Windows.Application.Current.Shutdown(); 
        }

        public void Dispose()
        {
            Hook.Dispose();
            AppNotifyIcon.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
