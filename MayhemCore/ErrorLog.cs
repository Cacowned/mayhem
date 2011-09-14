using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MayhemCore
{
    // There should be only one of these
    // We should be able to write to the error log from
    // any class, thus it is static
    public static class ErrorLog
    {
        // What do we do if this thing gets huge? It will keep using up memory. 
        // Do we want to remove things from the beginning?

        // Collection that stores all of the errors
        private static BindingCollection<MayhemError> errors = new BindingCollection<MayhemError>();

        // private delegate void AddHandler(ErrorType error, string message);

        public static void AddError(ErrorType error, string message)
        {
            /*
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Send, new AddHandler(Add), item);
            }
            else
            {
                _underlyingCollection.Add(item);
            }
            */

            MayhemError err = new MayhemError(error, message);

            errors.Insert(0,err);

            // For the time being, write the error to Debug as well
            Logger.WriteLine(message);
        }


        public static BindingCollection<MayhemError> Errors
        {
            get
            {
                return errors;
            }
        }

        // Get all of the errors in the collection
        // with an error type of at least
        // minimum type
        public static IEnumerable<MayhemError> GetErrors(ErrorType minimumType)
        {
            return errors.Where(x => x.Type > minimumType);
        }
    }

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

        public MayhemError(ErrorType type, string message)
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
