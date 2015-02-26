using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Interop;

namespace SelectAndTranslate 
{
    /// <summary>
    /// Interaction logic for TranslationWindow.xaml
    /// </summary>
    public partial class TranslationWindow : Window
    {    
        private const int VK_LCONTROL = 0xA2; // virtual-key code for left CTRL   

        private const int WM_KEYDOWN = 0x0100; 
        private const int WM_KEYUP = 0x0101;

        public static List<WebTranslator> Translators;
        private List<Task> translationTasks = new List<Task>();

        private static TranslationWindow tw;

        private bool translationIsFinished = true;
        private bool firstResultIsGotten = false;
        private bool keyHasBeenReleased = true;

                
        public int Hotkey;
        private IntPtr handle;
        private IntPtr activeWindow;

        public TranslationWindow()
        {                      
            InitializeComponent();
            
            activeWindow = IntPtr.Zero;
            this.Visibility = Visibility.Hidden;
            this.Topmost = true;               
            tw = this;
            Hotkey = VK_LCONTROL;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.handle = new WindowInteropHelper(this).Handle;
        }

        public void AppendResultText(string text)
        {
            try
            {
                this.Dispatcher.Invoke(() => textBlock.Inlines.Add(new Run(text)));
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                string innerExeption = (e.InnerException != null) ? e.InnerException.Message : "";
                System.Windows.MessageBox.Show(e.Message + "; inner:" + innerExeption);
            }            
        }

        public void AppendBoldResultText(string text)
        {
            try
            {               
                this.Dispatcher.Invoke(() => 
                {
                     var boldResult = new Bold(new Run(text));
                     textBlock.Inlines.Add(boldResult);
                });
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {               
                string innerExeption = (e.InnerException != null) ? e.InnerException.Message : "";
                System.Windows.MessageBox.Show(e.Message + "; inner:" + innerExeption);
            }
        }

        // Translation
        #region Translation      

        public async void RunTranslationAsync()
        {
            string clipboard = WinAPI.Clipboard.GetClipboardText();

            lblSource.Content = clipboard;
            lblSource.ToolTip = clipboard;
            textBlock.Text = "";

            firstResultIsGotten = false;
            translationIsFinished = false;
            
            runTranslationTasks(clipboard);

            await Task.WhenAny(translationTasks);
            firstResultIsGotten = true;

            await Task.WhenAll(translationTasks);
            translationIsFinished = true;
        }

        private void runTranslationTasks(string clipboard)
        {
            translationTasks.Clear();

            foreach (var translator in Translators)
            {
                translationTasks.Add(Task.Run(() =>
                {
                    string result = translator.Translate(clipboard);

                    AppendResultText(translator + ":\n");
                    AppendBoldResultText(result);
                    AppendResultText("\n\n");
                }));
            }
        }

        #endregion
                 
        // Key hook
        #region Key hook

        public Action<int, IntPtr, IntPtr> hookAction = delegate(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (tw.hotkeyPressed(nCode, wParam, lParam))
            {      
                if (tw.translationIsFinished && tw.keyHasBeenReleased)
                {
                    tw.keyHasBeenReleased = false;

                    foreach (var task in tw.translationTasks)
                    {
                        if (task != null)task.Dispose();
                    }

                    tw.setLocation(tw.GetCursorPosition());
                    tw.simulateCtrlC();                    
                    tw.RunTranslationAsync();                            
                }

                if (tw.Visibility != Visibility.Visible && tw.firstResultIsGotten)
                    tw.Visibility = Visibility.Visible;
            } 
            else
            {
                if (wParam == (IntPtr)WM_KEYUP)
                {
                    tw.keyHasBeenReleased = true;
                    tw.Visibility = Visibility.Hidden;
                }
            }
        };

        private bool hotkeyPressed(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && Marshal.ReadInt32(lParam) == Hotkey; 
        }

        private void simulateCtrlC()
        {
            System.Windows.Forms.SendKeys.SendWait("^c");
        }
        
        #endregion               
        
        // Window location
        #region Window location

        public WinAPI.POINT GetCursorPosition()
        {
            WinAPI.POINT point;
            WinAPI.NativeMethods.GetCursorPos(out point);
            return point;
        }

        private void setSize(WinAPI.POINT size)
        {
            this.Width = size.x;
            this.Height = size.y;
        }

        private void setLocation(WinAPI.POINT location)
        {
            this.Left = location.x;
            this.Top = location.y;
        }
    
        private WinAPI.Rectangle getWindowRect(IntPtr window)
        {
            return new WinAPI.Rectangle(window);
        }

        #endregion        
    }
}
