using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SelectAndTranslate.WinAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Point Location
        {
            get
            {
                return new Point(this.Left, this.Top);
            }
        }

        public Point Size
        {
            get
            {
                return new Point
                    (
                        this.Right - this.Left,
                        this.Bottom - this.Top
                    );
            }
        }

        private RECT rect;

        public Rectangle(IntPtr windowHandle)
        {
            WinAPI.NativeMethods.GetWindowRect(windowHandle, out rect);

            this.Left = rect.left;
            this.Top = rect.top;
            this.Right = rect.right;
            this.Bottom = rect.bottom;
        }      

        public static Rectangle getWindowRect(IntPtr window)
        {
            return new Rectangle(window);
        }
    }
}
