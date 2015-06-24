using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XComponent.Common.UI.Helpers
{
    public static class ExceptionHelper
    {

        public static string FullExceptionTrace(this Exception e)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(e.Message);
            result.AppendLine(e.StackTrace);

            Exception innerException = e.InnerException;
            while(innerException!=null)
            {
                result.AppendLine(innerException.Message);
                result.AppendLine(innerException.StackTrace);
                innerException = innerException.InnerException;
            }
            return result.ToString();
        }
    }
}
