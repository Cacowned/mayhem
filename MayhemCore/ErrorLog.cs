using System.Collections.Generic;
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
            MayhemError err = new MayhemError(error, message);

            errors.Insert(0, err);

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

        /// <summary>
        /// Get all of the errors in the collection with an error type with a given minimum type
        /// </summary>
        /// <param name="minimumType">The minimum type of error to retrieve.</param>
        /// <returns>An enumeration of the errors that fit the criteria</returns>
        public static IEnumerable<MayhemError> GetErrorsAtLevel(ErrorType minimumType)
        {
            return errors.Where(x => x.Type > minimumType);
        }
    }
}
