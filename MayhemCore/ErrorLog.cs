using System.Collections.Generic;
using System.Linq;

namespace MayhemCore
{
    public static class ErrorLog
    {
        // TODO: Implement trimming on the error log so it doesn't eat up ram

        // Collection that stores all of the errors
        private static BindingCollection<MayhemError> errors = new BindingCollection<MayhemError>();

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
