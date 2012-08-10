using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MayhemCore
{
    public static class ErrorLog
    {
        // TODO: Implement trimming on the error log so it doesn't eat up ram
        static ErrorLog()
        {
            Errors = new BindingCollection<MayhemError>();
        }

        /// <summary>
        /// Add an error to the error log.
        /// </summary>
        /// <param name="error">The error type for this error</param>
        /// <param name="message">The error message text</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
        public static void AddError(ErrorType error, string message)
        {
            MayhemError err = new MayhemError(error, message);

            Errors.Insert(0, err);

            // For the time being, write the error to Debug as well
            Logger.WriteLine(message);
        }

        /// <summary>
        /// The collection of all errors logged.
        /// </summary>
        public static BindingCollection<MayhemError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Get all of the errors in the collection with an error type with a given minimum type
        /// </summary>
        /// <param name="minimumType">The minimum type of error to retrieve.</param>
        /// <returns>An enumeration of the errors that fit the criteria</returns>
        public static IEnumerable<MayhemError> GetErrorsAtLevel(ErrorType minimumType)
        {
            return Errors.Where(x => x.Type > minimumType);
        }
    }
}
