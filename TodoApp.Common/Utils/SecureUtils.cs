using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Common.Utils
{
    public class SecurityUtils
    {
        public static string SafetyCharsForLIKEOperator(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Replace("\\", "\\\\\\\\").Replace("%", "\\%");
        }

    }
}
