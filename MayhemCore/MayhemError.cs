using System;

namespace MayhemCore
{
    public class MayhemError
    {
        public DateTime Time { get; private set; }
        public string TimeString
        {
            get
            {
                return Time.ToShortTimeString();
            }
        }
        public ErrorType Type { get; private set; }
        public string Message { get; private set; }

        internal MayhemError(ErrorType type, string message)
        {
            this.Type = type;
            this.Message = message;

            this.Time = DateTime.Now;
        }
    }

    // The different types of errors we can have
    public enum ErrorType
    {
        Message = 0,
        Warning = 1,
        Failure = 2
    }
}
