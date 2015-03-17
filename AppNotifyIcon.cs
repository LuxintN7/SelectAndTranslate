using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace SelectAndTranslate
{
    public sealed class AppNotifyIcon : IDisposable
    {
        private NotifyIcon notifyIcon = new NotifyIcon();

        public event EventHandler ExitClicked;
        public event EventHandler OptionsClicked;

        public AppNotifyIcon()
        {
            notifyIcon.Icon = Icon.FromHandle(new Bitmap(Application.GetResourceStream(new Uri("pack://application:,,,/img/logo32.png")).Stream).GetHicon());
            notifyIcon.ContextMenu = CreateContextMenu();
            notifyIcon.Visible = true;
        }

        private ContextMenu CreateContextMenu()
        {
            MenuItem[] menuItems =  
            {
                new MenuItem("Options...", OnOptionsClick),
                new MenuItem("-"),
                new MenuItem("Exit", OnExitClick)
            };

            return new ContextMenu(menuItems);
        }

        private void OnExitClick(object sender, EventArgs e)
        {
            if (ExitClicked != null)
            {
                ExitClicked(sender, e);
            }
        }

        private void OnOptionsClick(object sender, EventArgs e)
        {
            if (OptionsClicked != null)
            {
                OptionsClicked(sender, e);
            }
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
