using System;
using System.Threading;
using System.Windows;
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
