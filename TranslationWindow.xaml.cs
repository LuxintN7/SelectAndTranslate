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
    public partial class TranslationWindow : Window, IDisposable
    {  
        public static List<WebTranslator> Translators;
        private List<Task> translationTasks = new List<Task>();  

        private const int VK_LCONTROL = 0xA2; // virtual-key code for left CTRL
        private const int WM_KEYDOWN = 0x0100; // keydown message
        private const int WM_CLIPBOARDUPDATE = 0x031D; // ClipboardUpdate message

        private static TranslationWindow tw;

        private bool translationIsFinished = true;
        private bool firstResultIsGotten = false;
        private bool keyHasBeenReleased = true;
        private bool clipboardContentChanged = true;
                
        public int Hotkey;
        private IntPtr handle;
        private HwndSource hwndSource;
        private HwndSourceHook hwndSourceHook;

        public TranslationWindow()
        {                      
            InitializeComponent();            
            this.Visibility = Visibility.Hidden;
            this.Topmost = true;  
             
            tw = this;
            Hotkey = VK_LCONTROL;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.handle = new WindowInteropHelper(this).Handle;            
            hwndSource = HwndSource.FromHwnd(this.handle);
            
            // the hook allows to recieve Windows messages
            hwndSourceHook = new HwndSourceHook(WndProc);
            hwndSource.AddHook(hwndSourceHook);
            
            // allows WndProc() to receive clipboard messages
            WinAPI.NativeMethods.AddClipboardFormatListener(this.handle);
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
                printException(e);
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
                printException(e);
            }
        }

        private static void printException(Exception e)
        {
            string innerExeption = (e.InnerException != null) ? e.InnerException.Message : "";
            System.Windows.MessageBox.Show(e.Message + "; inner:" + innerExeption);
        }

        // Translation
        #region Translation      

        public async void RunTranslationAsync()
        {
            string clipboard;

            try
            {
                clipboard = Clipboard.ContainsText() ? Clipboard.GetText() : " | Clipboard is empty | "; 
            }
            catch (Exception e)
            {
                printException(e);
                clipboard = " | Exception: " + e + " | ";
            }            

            lblSource.Content = clipboard;
            lblSource.ToolTip = clipboard;
            textBlock.Text = "";

            firstResultIsGotten = false;
            translationIsFinished = false;
            
            runTranslationTasks(clipboard);

            await Task.WhenAny(translationTasks);
            if (!translationIsFinished) firstResultIsGotten = true;

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

        public Action<int, IntPtr, IntPtr> KeyboardHookAction = delegate(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (tw.hotkeyPressed(nCode, wParam, lParam))
            {
                if (tw.translationIsFinished && tw.keyHasBeenReleased)
                {
                    tw.keyHasBeenReleased = false;
                    tw.setLocation(tw.GetCursorPosition());                    
                    tw.translationTasks.Clear();                    
                    tw.simulateCtrlC(); 
                }

                if (tw.clipboardContentChanged) // wait until data is copied to the clipboard completely
                {                   
                    tw.clipboardContentChanged = false;
                    tw.RunTranslationAsync();                    
                }

                if (tw.translationTasks.Count != 0 && tw.firstResultIsGotten)
                    tw.Visibility = Visibility.Visible;
            }
            else
            {
                tw.keyHasBeenReleased = true;
                tw.Visibility = Visibility.Hidden;
            }
        };

        private bool hotkeyPressed(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN && Marshal.ReadInt32(lParam) == Hotkey; 
        }

        private void simulateCtrlC()
        {
            System.Windows.Forms.SendKeys.SendWait("^c"); // SendWait() doesnt't really wait for a data 
                                                          // to be copied completely  
        }

        // Processes Windows messages
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                clipboardContentChanged = true;
                handled = true;
            }
            return IntPtr.Zero;
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
    

        public void Dispose()
        {
            WinAPI.NativeMethods.RemoveClipboardFormatListener(this.handle);
            if (hwndSourceHook != null) hwndSource.RemoveHook(hwndSourceHook);
        }
    }
}
