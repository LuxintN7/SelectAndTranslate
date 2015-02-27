﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SelectAndTranslate.WinAPI
{
    public class KeyboardHook : IDisposable
    {
        // KeyboardProc http://msdn.microsoft.com/en-us/library/ms644984(v=vs.85).aspx
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public bool IsSet = false;

        private const int WH_KEYBOARD_LL = 13;
        private HookProc hookProc; 
        private Action<int, IntPtr, IntPtr> hookAction;
        private IntPtr hookID = IntPtr.Zero;        
        private IntPtr hMod = LoadLibrary("User32");

        public KeyboardHook(Action<int, IntPtr, IntPtr> hookAction)
        {
            this.hookAction = hookAction;
            hookProc = hookCallbackProc;
        }

        public void SetHook() 
        {
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hMod, 0);
            IsSet = true;
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(hookID);
            IsSet = false;
        }

        private IntPtr hookCallbackProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            hookAction(nCode, wParam, lParam);
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {   
            if (IsSet) Unhook();        
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}
