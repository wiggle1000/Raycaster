using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal static class Debug
    {
        public static void Log(string toPrint)
        {
            Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "]" + toPrint);
        }
        public static void Log(params string[] args)
        {
            Log(String.Concat(args));
        }
        public static void Log(string format, params object[] args)
        {
            Log(String.Format(format, args));
        }
    }
}
