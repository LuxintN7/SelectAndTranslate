using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndTranslate.WinAPI
{
    public static class Clipboard
    {
        public const uint CF_UNICODETEXT = 13; 

        private static object lockToken = new object();

        public static string GetClipboardText()
        {
            string clipboard = null;

            try
            {
                lock (lockToken)
                {
                    OpenClipboard(IntPtr.Zero);
                    IntPtr dataHandle = GetClipboardData(CF_UNICODETEXT);
                    clipboard = Marshal.PtrToStringUni(dataHandle);
                    CloseClipboard(IntPtr.Zero);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

            return clipboard;
        }

        public static void SetClipboardText(string text)
        {
            try
            {
                lock (lockToken)
                {
                    IntPtr textHandle = Marshal.StringToHGlobalUni(text);
                    OpenClipboard(IntPtr.Zero);
                    //EmptyClipboard();
                    SetClipboardData(CF_UNICODETEXT, textHandle);
                    CloseClipboard(IntPtr.Zero);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWnd);

        [DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseClipboard(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();
    }
}
