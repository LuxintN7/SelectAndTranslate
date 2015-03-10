using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace SelectAndTranslate
{
    public sealed class AppNotifyIcon : IDisposable
    {
        private NotifyIcon notifyIcon = new NotifyIcon();
        private OptionsWindow optionsWindow = new OptionsWindow();
        private MainWindow mainWindow;

        public AppNotifyIcon(MainWindow mainWindow)
        {
            notifyIcon.Icon = Icon.FromHandle(new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/img/logo32.png")).Stream).GetHicon());
            notifyIcon.ContextMenu = getNewContextMenu();
            this.mainWindow = mainWindow;
            notifyIcon.Visible = true;
        }

        private ContextMenu getNewContextMenu()
        {
            MenuItem[] menuItems = new MenuItem[] 
            {
                new MenuItem("Options...", options_Click),
                new MenuItem("-"),
                new MenuItem("Exit", exit_Click)
            };

            return new ContextMenu(menuItems);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            mainWindow.Exit();            
        }

        private void options_Click(object sender, EventArgs e)
        {
            optionsWindow = new OptionsWindow();
            optionsWindow.Visibility = System.Windows.Visibility.Visible;
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
