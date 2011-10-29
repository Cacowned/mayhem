using System;

namespace MayhemCore
{
    /// <summary>
    /// This class represents a single error in the system
    /// </summary>
    public class MayhemError
    {
        /// <summary>
        /// The time the error occurred
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// The string representation of the Time
        /// </summary>
        public string TimeString
        {
            get
            {
                return Time.ToShortTimeString();
            }
        }

        /// <summary>
        /// The error type
        /// </summary>
        public ErrorType Type { get; private set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; private set; }

        internal MayhemError(ErrorType type, string message)
        {
            Type = type;
            Message = message;

            Time = DateTime.Now;
        }
    }

    /// <summary>
    /// The different types of errors we can have 
    /// </summary>
    public enum ErrorType
    {
        Message = 0,
        Warning = 1,
        Failure = 2
    }
}
