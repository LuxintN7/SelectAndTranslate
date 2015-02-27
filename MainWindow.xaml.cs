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
        private WinAPI.KeyboardHook hook;
        private AppNotifyIcon appNotifyIcon;         
        private TranslationWindow translationWindow = new TranslationWindow();

        public MainWindow()
        {            
            InitializeComponent();            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            appNotifyIcon = new AppNotifyIcon(this);
            hook = new WinAPI.KeyboardHook(translationWindow.KeyboardHookAction);
            hook.SetHook();
            
            this.Visibility = Visibility.Hidden;
        }

        internal void Exit()
        {
            this.Dispose();
            System.Windows.Application.Current.Shutdown(); 
        }

        public void Dispose()
        {
            hook.Dispose();
            appNotifyIcon.Dispose();
            translationWindow.Dispose();
        }
    }
}
