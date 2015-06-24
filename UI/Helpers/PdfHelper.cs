using System;
using System.Linq;
using Microsoft.Win32;

namespace XComponent.Common.UI.Helpers
{
    public static class PdfHelper
    {        
        public static bool IsAcrobatReaderInstalled()
        {
            var acrobatType = Type.GetTypeFromProgID("AcroPDF.PDF");
            return acrobatType != null;
        }
    }
}
