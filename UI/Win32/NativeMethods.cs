using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XComponent.Common.UI.Win32
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_SHOWWINDOW = 0x0040;

    }
}
