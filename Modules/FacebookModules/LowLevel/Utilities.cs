using System;
using System.Threading;
namespace FacebookModules.LowLevel
{
    public static class Utilities
    {
        #region Check Internet Connection
        /// <summary>
        /// Checks if there is a current Internet connection
        /// </summary>
        /// <returns>true if there is a current Internet connection</returns>
        public static bool ConnectedToInternet()
        {
            try
            {
                // Make a request with a 640ms timeout
                CallWithTimeout(MakeRequest, 640);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Make a request to google.com
        /// </summary>
        private static void MakeRequest()
        {
            // TODO this happens every time once net has been disabled
            System.Net.IPHostEntry obj = System.Net.Dns.GetHostEntry("www.google.com");
        }

        /// <summary>
        /// A method that will trigger a "timeout" error to be used when running a method with a 
        /// specific timout trigger
        /// </summary>
        /// <param name="action">The method being run</param>
        /// <param name="timeout">The timeout in milliseconds</param>
        public static void CallWithTimeout(Action action, int timeout)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeout))
            {
                wrappedAction.EndInvoke(result);
                //throw new TimeoutException();
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }
        #endregion
    }
}
