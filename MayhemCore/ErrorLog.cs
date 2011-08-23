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
    public class ErrorLog
    {
        // What do we do if this thing gets huge? It will keep using up memory. 
        // Do we want to remove things from the beginning?

        // Collection that stores all of the errors
        private static BindingCollection<Error> errors = new BindingCollection<Error>();

        public ErrorLog()
        { }

        // private delegate void AddHandler(ErrorType error, string message);

        public static void AddError(ErrorType error, string message) {
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

            Error err = new Error(error, message);

            errors.Add(err);

            // For the time being, write the error to Debug as well
            Debug.WriteLine(message);
        }


        public static BindingCollection<Error> Errors
        {
			get {
				return errors;
			}
		}

        // Get all of the errors in the collection
        // with an error type of at least
        // minimum type
        public static IEnumerable<Error> GetErrors(ErrorType minimumType)
        {
            return errors.Where(x => x.Type > minimumType);
        }
    }

    public class Error
    {
        public DateTime Time { get; private set; }
        public ErrorType Type { get; private set; }
        public string Message { get; private set; }

        public Error(ErrorType type, string message)
        {
            this.Type = type;
            this.Message = message;

            this.Time = DateTime.Now;
        }
    }

    // The different types of errors we can have
    public enum ErrorType {
        Message = 1,
        Warning = 2,
        Failure = 3
    }
}
