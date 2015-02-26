using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace SelectAndTranslate
{
    public class AppNotifyIcon : IDisposable
    {
        public NotifyIcon notifyIcon = new NotifyIcon();
        private OptionsWindow optionsWindow = new OptionsWindow();
        private MainWindow mainWindow;

        public AppNotifyIcon(MainWindow mainWindow)
        {            
            setIcon("logo32.png");
            notifyIcon.ContextMenu = getNewContextMenu();
            this.mainWindow = mainWindow;
            notifyIcon.Visible = true;
        }

        public Icon GetIconFromImage(string pathToImage)
        {
            var x = @"C:\!prog\Select and Translate\SelectAndTranslate\img\logo32.png"; 
            return System.Drawing.Icon.FromHandle(new Bitmap(x).GetHicon());
        }

        public string pathToAssembly;

        private void setIcon(string imageName)
        {
            string pathToImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\img\" + imageName);            
            notifyIcon.Icon = GetIconFromImage(pathToImage);
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
