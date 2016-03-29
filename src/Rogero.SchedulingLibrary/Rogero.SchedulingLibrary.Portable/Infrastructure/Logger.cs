using System;

namespace Rogero.SchedulingLibrary.Infrastructure
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
