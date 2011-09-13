using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MayhemCore
{
    public static class Logger
    {
        static string GetCallStackString()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(2);
            string callingClass = frame.GetMethod().DeclaringType.Name;
            string callingMethod = frame.GetMethod().Name;
            return "[" + callingClass + "." + callingMethod + "] ";
        }

        public static void Write(object o)
        {
            Debug.Write(GetCallStackString() + o);
        }

        public static void WriteLine(object o)
        {
            Debug.WriteLine(GetCallStackString() + o);
        }

        public static void WriteLine(string format, params object [] args)
        {
            Debug.WriteLine(GetCallStackString() + format, args);
        }

        public static void WriteLineIf(bool condition, object o)
        {
            if (condition)
            {
                WriteLine(o);
            }
        }
    }
}
