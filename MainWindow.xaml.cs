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
        private OptionsWindow optionsWindow; 

        public MainWindow()
        {            
            InitializeComponent();  
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            optionsWindow = new OptionsWindow();

            appNotifyIcon = new AppNotifyIcon();
            appNotifyIcon.OptionsClicked +=
                (sender, args) =>
                {
                    if (optionsWindow.IsClosed) 
                        optionsWindow = new OptionsWindow() {Visibility = Visibility.Visible};
                    else 
                        optionsWindow.Show();
                };
            appNotifyIcon.ExitClicked += (sender, args) => Exit();

            hook = new WinAPI.KeyboardHook(translationWindow.KeyboardHookAction);
            hook.SetHook();

            this.Visibility = Visibility.Hidden;
        }

        public void Exit()
        {
            this.Dispose();
            Application.Current.Shutdown(); 
        }

        public void Dispose()
        {
            hook.Dispose();
            appNotifyIcon.Dispose();
            translationWindow.Dispose();
        }
    }
}
