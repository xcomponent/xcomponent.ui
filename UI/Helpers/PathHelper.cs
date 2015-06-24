using System;
using System.IO;

namespace XComponent.Common.UI.Helpers
{
    public static class PathHelper
    {
        public static string GetResearchCachePath(string applicationName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, "Research");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetDockingLayoutPath(string user, string applicationName)
        {
            var path = !string.IsNullOrEmpty(user) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, "Layout", user) : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, "Layout");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        // ELYON-306: (a bit of a hack) Due to a change in the layout files path we need a way to import old layouts in the new location.
        // This shoud only be done the 1st time XComponent is started after upgrade.
        public static string GetOldDockingLayoutPath(string applicationName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, "Layout");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
