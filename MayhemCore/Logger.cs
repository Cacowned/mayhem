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

        public static void Write(object obj)
        {
            Debug.Write(GetCallStackString() + obj);
        }

        public static void WriteLine(object obj)
        {
            Debug.WriteLine(GetCallStackString() + obj);
        }

        public static void WriteLine(string format, params object [] args)
        {
            Debug.WriteLine(GetCallStackString() + format, args);
        }

        public static void WriteLineIf(bool condition, object obj)
        {
            if (condition)
            {
                WriteLine(obj);
            }
        }
    }
}
