using System;
using System.Diagnostics;
using System.IO;

namespace Dowsingman2.UtilityClass
{
    internal class MyTraceSource
    {
        private const bool IsEnabled = true;

        public static void TraceEvent(TraceEventType eventType, Exception exception)
        {
            if (IsEnabled)
            {
                string message = DateTime.Now + "\n" + exception.ToString();
                Debug.WriteLine(message);
            }
        }

        public static void TraceEvent(TraceEventType eventType, string message)
        {
            if (IsEnabled)
            {
                Debug.WriteLine(message);
            }
        }
    }
}