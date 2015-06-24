using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XComponent.Common.UI.Helpers
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DevMode
    {        
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        public short dmLogPixels;
        public short dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    public static class ResolutionHelper
    {
        public const int ResolutionWidthLimit = 2000;

        internal static class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            internal static extern int EnumDisplaySettings(
              string deviceName, int modeNum, ref DevMode devMode);            

            public const int ENUM_CURRENT_SETTINGS = -1;
            public const int CDS_UPDATEREGISTRY = 0x01;
            public const int CDS_TEST = 0x02;
            public const int DISP_CHANGE_SUCCESSFUL = 0;
            public const int DISP_CHANGE_RESTART = 1;
            public const int DISP_CHANGE_FAILED = -1;
        }

        public static bool IsHighResolution()
        {
            DevMode dm = new DevMode();
            dm.dmDeviceName = new String (new char[32]);
            dm.dmFormName = new String (new char[32]);
            dm.dmSize = (short)Marshal.SizeOf (dm);

            if (0 != NativeMethods.EnumDisplaySettings(null,
                NativeMethods.ENUM_CURRENT_SETTINGS, ref dm))
            {
                var width = dm.dmPelsWidth;
                if (width > ResolutionWidthLimit)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
