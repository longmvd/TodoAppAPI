using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DL
{
    public static class DatabaseConstant
    {
        public static string ConnectionString { get; set; }

        public static int MaxReturnRecord = 5000;
        public static int MinReturnRecord = 30;
        public static int MaxPageIndex = 500;
    }
}
