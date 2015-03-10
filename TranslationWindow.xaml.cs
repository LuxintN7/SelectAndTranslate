using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
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

        private const int VK_LMENU = 0xA4; // virtual-key code for left ALT
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
            this.Visibility = Visibility.Visible; // set focus
            this.Visibility = Visibility.Hidden;
            this.Topmost = true;  
             
            tw = this;
            Hotkey = VK_LMENU;
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
            Debug.WriteLine(e.Message + ";\n    INNER:" + innerExeption);
        }

        // Translation
        #region Translation      

        public async void RunTranslationAsync()
        {
            string clipboard = "";

            try
            {               
                clipboard = Clipboard.ContainsText() ? WinAPI.Clipboard.GetClipboardText() : " | Clipboard is empty | ";                 
            }
            catch (COMException) { throw; }
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
            if (tw.hotkeyPressed(lParam))
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

        private void simulateCtrlC()
        {
            System.Windows.Forms.SendKeys.SendWait("^c"); // SendWait() doesnt't really wait for a data 
                                                          // to be copied completely  
        }

        private bool hotkeyPressed(IntPtr lParam)
        {
            return Hotkey == VK_LMENU ? altIsPressed(lParam) : keyIsPressed(lParam);
        }

        private bool altIsPressed(IntPtr lParam)
        {           
            var hookStruct = (WinAPI.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.KBDLLHOOKSTRUCT));

            return ((hookStruct.flags >> 5) & 1) == 1; // the 5th bit is the Alt key pressed flag (1 means any Alt is pressed)
        }
        
        bool keyIsPressed(IntPtr lParam) // this method can be used for any key, however, it does not work properly
                                         // on Windows 8 (the key pressed flag is set to 1 when it should still be 0,
                                         // in this case 1 means the key is released)
        {
            var hookStruct = (WinAPI.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.KBDLLHOOKSTRUCT));

            return hookStruct.vkCode == Hotkey && ((hookStruct.flags >> 7) & 1) == 0; // 7th bit - key pressed flag 
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

        private void setLocation(WinAPI.POINT location)
        {
            this.Left = location.x;
            this.Top = location.y;
        }
        #endregion        
    

        public void Dispose()
        {
            WinAPI.NativeMethods.RemoveClipboardFormatListener(this.handle);
            if (hwndSourceHook != null) hwndSource.RemoveHook(hwndSourceHook);
        }

    }
}
