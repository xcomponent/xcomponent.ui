using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace XComponent.Common.UI.I18n
{
    public enum CultureEnum
    {
        fr,
        en,
    }

    public static class CultureManager
    {
        static public CultureEnum CurrentCulture { get; set; }

    }
}
