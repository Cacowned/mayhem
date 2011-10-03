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

        public static void Write(object value)
        {
            Debug.Write(GetCallStackString() + value);
        }

        public static void WriteLine(object value)
        {
            Debug.WriteLine(GetCallStackString() + value);
        }

        public static void WriteLine(string format, params object [] args)
        {
            Debug.WriteLine(GetCallStackString() + format, args);
        }

        public static void WriteLineIf(bool condition, object value)
        {
            if (condition)
            {
                WriteLine(value);
            }
        }
    }
}
