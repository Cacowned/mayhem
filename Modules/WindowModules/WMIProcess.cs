using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;

namespace WindowModules
{
    public static class WMIProcess
    {
        static ConnectionOptions options;
        static ManagementScope connectionScope;

        static WMIProcess()
        {
            options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;

            connectionScope = new ManagementScope();
            connectionScope.Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2");
            connectionScope.Options = options;

            try
            {
                connectionScope.Connect();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An Error Occurred: " + e.Message.ToString());
            }
        }

        public static string GetFilename(int pid)
        {
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Process Where ProcessId = '" + pid + "'");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(connectionScope, msQuery);

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
