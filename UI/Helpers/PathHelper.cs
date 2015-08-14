using System;
using System.IO;

namespace XComponent.Common.UI.Helpers
{
    public static class PathHelper
    {
        public const string DefaultProfil = "default";
        public static string GetResearchCachePath(string applicationName, string applicationProfil = DefaultProfil)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, applicationProfil, "Research");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetDockingLayoutPath(string user, string applicationName, string applicationProfil = DefaultProfil)
        {
            var path = !string.IsNullOrEmpty(user) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, applicationProfil, "Layout", user) : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, applicationProfil, "Layout");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        // ELYON-306: (a bit of a hack) Due to a change in the layout files path we need a way to import old layouts in the new location.
        // This shoud only be done the 1st time XComponent is started after upgrade.
        public static string GetOldDockingLayoutPath(string applicationName, string applicationProfil = DefaultProfil)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName, applicationProfil, "Layout");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
