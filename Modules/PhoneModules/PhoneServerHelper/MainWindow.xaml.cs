using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetFwTypeLib;
using System.Security.Principal;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PhoneServerHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Open the port in the firewall

            Type type = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
            INetFwOpenPort port = Activator.CreateInstance(type) as INetFwOpenPort;

            INetFwOpenPorts ports;
            port.Port = 19283;
            port.Name = "Mayhem";
            port.Enabled = true;

            Type netFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(netFwMgrType);
            ports = (INetFwOpenPorts)mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts;

            ports.Add(port);

            // Add the ACL

            string name = WindowsIdentity.GetCurrent().Name;
            SecurityIdentity sid = SecurityIdentity.SecurityIdentityFromName(name);
            string acl = "D:(A;;GA;;;" + sid.ToString() + ")";
            Debug.WriteLine(acl);
            SetHttpNamespaceAcl("http://+:19283/", acl);

            this.Close();
        }

        public void SetHttpNamespaceAcl(string urlPrefix, string acl)
        {
            HTTPAPI_VERSION version = new HTTPAPI_VERSION();
            version.HttpApiMajorVersion = 1;
            version.HttpApiMinorVersion = 0;

            HttpInitialize(version, HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

            HTTP_SERVICE_CONFIG_URLACL_SET urlAclConfig = new HTTP_SERVICE_CONFIG_URLACL_SET();
            urlAclConfig.KeyDesc.pUrlPrefix = urlPrefix;
            urlAclConfig.ParamDesc.pStringSecurityDescriptor = acl;

            IntPtr pUrlAclConfig = Marshal.AllocHGlobal(Marshal.SizeOf(urlAclConfig));

            Marshal.StructureToPtr(urlAclConfig, pUrlAclConfig, false);

            try
            {
                uint retval = HttpSetServiceConfiguration(IntPtr.Zero, HTTP_SERVICE_CONFIG_ID.HttpServiceConfigUrlAclInfo, pUrlAclConfig, (uint)Marshal.SizeOf(urlAclConfig), IntPtr.Zero);

                if (retval != 0 && retval != 183)
                {
                    throw new ExternalException("Error Setting Configuration: " + SecurityIdentity.GetErrorMessage(retval));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                if (pUrlAclConfig != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(pUrlAclConfig, typeof(HTTP_SERVICE_CONFIG_URLACL_SET));
                    Marshal.FreeHGlobal(pUrlAclConfig);
                }
            }
        }

        [DllImport("Httpapi.dll")]
        internal static extern uint HttpInitialize(HTTPAPI_VERSION Version, uint Flags, IntPtr pReserved);

        [DllImport("Httpapi.dll")]
        internal static extern uint HttpSetServiceConfiguration(IntPtr ServiceHandle, HTTP_SERVICE_CONFIG_ID ConfigId, IntPtr pConfigInformation, uint ConfigInformationLength, IntPtr pOverlapped);

        internal const uint HTTP_INITIALIZE_CONFIG = 2;

        internal struct HTTPAPI_VERSION
        {
            public ushort HttpApiMajorVersion;
            public ushort HttpApiMinorVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HTTP_SERVICE_CONFIG_URLACL_SET
        {
            public HTTP_SERVICE_CONFIG_URLACL_KEY KeyDesc;
            public HTTP_SERVICE_CONFIG_URLACL_PARAM ParamDesc;
        }

        internal enum HTTP_SERVICE_CONFIG_ID
        {
            HttpServiceConfigIPListenList,
            HttpServiceConfigSSLCertInfo,
            HttpServiceConfigUrlAclInfo,
            HttpServiceConfigTimeout,
            HttpServiceConfigMax
        }

        internal struct HTTP_SERVICE_CONFIG_URLACL_KEY
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pUrlPrefix;
        }

        internal struct HTTP_SERVICE_CONFIG_URLACL_PARAM
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pStringSecurityDescriptor;
        }
    }
}
