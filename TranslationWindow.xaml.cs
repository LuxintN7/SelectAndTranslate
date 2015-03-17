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
        public static List<Translator> Translators;
        private List<Task> translationTasks = new List<Task>();  

        // ClipboardUpdate message
        private const int WM_CLIPBOARDUPDATE = 0x031D;        

        private static TranslationWindow tw;

        private bool translationIsFinished = true;
        private bool keyHasBeenReleased = true;
        private bool clipboardContentChanged = true;
                
        private IntPtr handle;
        private HwndSource sourceHandle;
        private HwndSourceHook sourceHookHandle;

        public TranslationWindow()
        {                      
            InitializeComponent();
            
            // set focus
            this.Visibility = Visibility.Visible; 
            this.Visibility = Visibility.Hidden;
            this.Topmost = true;  
             
            tw = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.handle = new WindowInteropHelper(this).Handle;            
            sourceHandle = HwndSource.FromHwnd(this.handle);
            
            // the hook allows to recieve Windows messages
            sourceHookHandle = new HwndSourceHook(WndProc);
            sourceHandle.AddHook(sourceHookHandle);
            
            // allows WndProc() to receive clipboard messages
            WinAPI.NativeMethods.AddClipboardFormatListener(this.handle);            
        }

        #region Translation      

        public async void RunTranslationAsync()
        {
            if (!Clipboard.ContainsText())
            {
                return;
            }

            string clipboardText = Clipboard.GetText();

            SourceLabel.Content = clipboardText;
            SourceLabel.ToolTip = clipboardText;
            ResultTextBlock.Text = "";

            translationTasks = StartAllTranslators(clipboardText);
            await Task.WhenAny(translationTasks);
            this.Visibility = Visibility.Visible;

            await Task.WhenAll(translationTasks);
            translationIsFinished = true;
        }

        private List<Task> StartAllTranslators(string text)
        {
            List<Task> translationTasks = new List<Task>();

            foreach (var translator in Translators)
            {
                translationTasks.Add(RunTranslatorAsync(translator, text));
            }

            return translationTasks;
        }

        private async Task RunTranslatorAsync(Translator translator, string text)
        {
            var translatedText = await translator.TranslateAsync(text);
            AppendResult(translator.GetType().Name, translatedText);
        }

        private void AppendResult(string translatorName, string text)
        {
            var translatorElement = new Run(translatorName);
            var textElement = new Bold(new Run(text));

            ResultTextBlock.Inlines.Add(translatorElement);
            ResultTextBlock.Inlines.Add(":\n");
            ResultTextBlock.Inlines.Add(textElement);
            ResultTextBlock.Inlines.Add("\n\n");
        }
        #endregion        
                 
        #region Key hook

        public Action<int, IntPtr, IntPtr> KeyboardHookAction = delegate(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (tw.AltIsPressed(lParam))
            {
                if (tw.translationIsFinished && tw.keyHasBeenReleased)
                {
                    tw.keyHasBeenReleased = false;
                    tw.SetLocation(tw.GetCursorPosition());                    
                    tw.translationTasks.Clear();                    
                    tw.SimulateCtrlC(); 
                }

                // wait until data is copied to the clipboard completely
                if (tw.clipboardContentChanged) 
                {                   
                    tw.clipboardContentChanged = false;
                    tw.RunTranslationAsync();                    
                }
            }
            else
            {
                tw.keyHasBeenReleased = true;
                tw.Visibility = Visibility.Hidden;
            }
        };

        private void SimulateCtrlC()
        {
            // SendWait() doesnt't really wait for a data to be copied completely
            System.Windows.Forms.SendKeys.SendWait("^c");  
        }

        private bool AltIsPressed(IntPtr lParam)
        {           
            var hookStruct = (WinAPI.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinAPI.KBDLLHOOKSTRUCT));

            // the 5th bit is the Alt key pressed flag (1 means any Alt is pressed)
            return ((hookStruct.flags >> 5) & 1) == 1; 
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

        #region Window location

        public WinAPI.POINT GetCursorPosition()
        {
            WinAPI.POINT point;
            WinAPI.NativeMethods.GetCursorPos(out point);
            return point;
        }

        private void SetLocation(WinAPI.POINT location)
        {
            this.Left = location.x;
            this.Top = location.y;
        }
        #endregion

        public void Dispose()
        {
            WinAPI.NativeMethods.RemoveClipboardFormatListener(this.handle);
            if (sourceHookHandle != null) sourceHandle.RemoveHook(sourceHookHandle);
        }

    }
}
