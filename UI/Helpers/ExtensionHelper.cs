using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XComponent.Common.UI.Helpers
{
    public class ExtensionHelper
    {        
        public static bool HasExecutable(string path)
        {
            if (path.EndsWith(".exe"))
            {
                return true;
            }
            var executable = FindExecutable(path);
            return !string.IsNullOrEmpty(executable);
        }

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            var result = NativeMethods.FindExecutable(path, string.Empty, executable);
            return result.ToInt32() >= 32 ? executable.ToString() : string.Empty;
        }

        internal static class NativeMethods
        {
            [DllImport("shell32.dll", EntryPoint = "FindExecutable", CharSet = CharSet.Unicode)]
            internal static extern IntPtr FindExecutable(string lpFile, string lpDirectory, StringBuilder lpResult);
        }
    }
}
