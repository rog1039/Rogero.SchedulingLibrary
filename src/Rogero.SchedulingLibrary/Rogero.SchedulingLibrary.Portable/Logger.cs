using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogero.SchedulingLibrary
{
    public static class Logger
    {
        public static Action<string> LogAction { get; set; }

        public static void Log(string message)
        {
            LogAction?.Invoke(message);
        }
    }
}
