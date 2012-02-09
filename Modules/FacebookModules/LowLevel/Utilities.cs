namespace FacebookModules.LowLevel
{
    public static class Utilities
    {
        /// <summary>
        /// Checks if there is a current Internet connection
        /// </summary>
        /// <returns>true if there is a current Internet connection</returns>
        public static bool ConnectedToInternet()
        {
            try
            {
                System.Net.IPHostEntry obj = System.Net.Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
