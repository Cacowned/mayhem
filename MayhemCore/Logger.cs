using System.Diagnostics;

namespace MayhemCore
{
    /// <summary>
    /// This class logs messages prefixed by the calling class and method name.
    /// Messages are written to Debug output.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Write the given object to output
        /// </summary>
        /// <param name="value">The object to write</param>
        public static void Write(object value)
        {
            Debug.Write(GetCallStackString() + value);
        }

        /// <summary>
        /// Write the given object to output followed by a newline character.
        /// </summary>
        /// <param name="value">The object to write</param>
        public static void WriteLine(object value)
        {
            Debug.WriteLine(GetCallStackString() + value);
        }

        /// <summary>
        /// Write the given object to output, followed by a newline character and populated 
        /// with the given arguments
        /// </summary>
        /// <param name="format">The string format</param>
        /// <param name="args">The arguments to populate the string with</param>
        public static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(GetCallStackString() + format, args);
        }

        /// <summary>
        /// Write the given object to output followed by a newline character only if
        /// the condition is true
        /// </summary>
        /// <param name="condition">If true, writes to output. If false, does nothing.</param>
        /// <param name="value">The object to write</param>
        public static void WriteLineIf(bool condition, object value)
        {
            if (condition)
            {
                WriteLine(value);
            }
        }

        private static string GetCallStackString()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(2);
            string callingClass = frame.GetMethod().DeclaringType.Name;
            string callingMethod = frame.GetMethod().Name;
            return "[" + callingClass + "." + callingMethod + "] ";
        }
    }
}
