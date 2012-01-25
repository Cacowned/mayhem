using System;
using System.Management;

namespace WindowModules
{
    public static class WMIProcess
    {
        private static readonly ConnectionOptions Options;
        private static readonly ManagementScope ConnectionScope;

        static WMIProcess()
        {
            Options = new ConnectionOptions();
            Options.Impersonation = ImpersonationLevel.Impersonate;
            Options.Authentication = AuthenticationLevel.Default;
            Options.EnablePrivileges = true;

            ConnectionScope = new ManagementScope();
            ConnectionScope.Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2");
            ConnectionScope.Options = Options;

            try
            {
                ConnectionScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message);
            }
        }

        public static string GetFilename(int pid)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where ProcessId = '" + pid + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(ConnectionScope, msQuery);

            foreach (ManagementObject item in searchProcedure.Get())
            {
                object o = item["ExecutablePath"];
                string s = o as string;
                return s;
            }
            return null;
        }
    }
}
