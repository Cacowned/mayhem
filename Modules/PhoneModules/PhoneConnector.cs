using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Windows;
using MayhemCore;
using NetFwTypeLib;

namespace PhoneModules
{
    public class PhoneConnector
    {
        private readonly int portNumber;

        private bool isServiceRunning;

        public bool IsServiceRunning
        {
            get { return isServiceRunning; }
        }

        private WebServiceHost host;
        private IMayhemService service;

        public delegate void EventCalledHandler(string eventText);

        public event EventCalledHandler EventCalled;

        public bool HasBeenSerialized
        {
            get;
            set;
        }

        public bool HasBeenDeserialized
        {
            get;
            set;
        }

        #region Singleton
        private static readonly PhoneConnector instance = new PhoneConnector();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PhoneConnector()
        {
        }

        private PhoneConnector()
        {
            portNumber = 19283;
            MayhemEntry.Instance.ShuttingDown += Mayhem_ShuttingDown;
        }

        private void Mayhem_ShuttingDown(object sender, EventArgs e)
        {
            if (service != null)
            {
                service.ShuttingDown();
            }
        }

        public static PhoneConnector Instance
        {
            get { return instance; }
        }
        #endregion

        public void SetNewData()
        {
            if (service != null)
            {
                try
                {
                    string insideDiv;
                    string html = PhoneLayout.Instance.SerializeToHtml(true, out insideDiv);
                    service.SetHtml(html);
                    service.SetInsideDiv(insideDiv);
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Phone Remote: Could not communicate with server");
                }
            }
        }

        public bool Enable(bool includeButtons)
        {
            if (!isServiceRunning)
            {
                isServiceRunning = true;
                return StartService(includeButtons);
            }

            return true;
        }

        public void Disable()
        {
            // if (isServiceRunning)
            // {
            //     refCount--;
            //     if (refCount == 0)
            //     {
            //         if (service != null)
            //         {
            //             service.ShuttingDown();
            //         }
            //         //isServiceRunning = false;
            //         //host.Close();
            //         service = null;
            //     }
            // }
        }

        private bool StartService(bool includeButtons)
        {
            Uri address = new Uri("http://localhost:" + portNumber + "/Mayhem");

            MayhemService svc = new MayhemService();
            svc.EventCalled += service_EventCalled;

            string insideDiv;
            string html = PhoneLayout.Instance.SerializeToHtml(includeButtons, out insideDiv);
            svc.SetHtml(html);
            svc.SetInsideDiv(insideDiv);

            host = new WebServiceHost(svc, address);

            // Enable Mex
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            WebHttpBinding binding = new WebHttpBinding();
            binding.ReaderQuotas.MaxArrayLength = 2147483647;

            host.AddServiceEndpoint(typeof(IMayhemService), binding, string.Empty);

            bool portFound = false;

            Type netFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(netFwMgrType);
            INetFwOpenPorts ports = mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts;
            System.Collections.IEnumerator enumerate = ports.GetEnumerator();
            while (enumerate.MoveNext())
            {
                INetFwOpenPort port = (INetFwOpenPort)enumerate.Current;
                if (port.Port == portNumber)
                {
                    portFound = true;
                    break;
                }
            }

            if (!portFound || !AclHelper.FindUrlPrefix("http://+:" + portNumber + "/"))
            {
                OpenServerHelper();
            }

            try
            {
                host.Open();
            }
            catch (Exception e)
            {
                isServiceRunning = false;
                Logger.WriteLine(e);
                ErrorLog.AddError(ErrorType.Failure, "Error starting Phone Remote service");
                return false;
            }

            isServiceRunning = true;
            Logger.WriteLine("Phone service started");

            WebChannelFactory<IMayhemService> myChannelFactory = new WebChannelFactory<IMayhemService>(new Uri(address.ToString()));
            service = myChannelFactory.CreateChannel();
            service.Html(false);

            return true;
        }

        private void OpenServerHelper()
        {
            FileInfo fi = new FileInfo(Assembly.GetCallingAssembly().Location);
            string pathOfHelper = Path.Combine(fi.DirectoryName, "PhoneServerHelper.exe");
            if (File.Exists(pathOfHelper))
            {
                MessageBox.Show("Mayhem will automatically configure your network settings. Click OK.", "Mayhem Phone Modules");

                Process p = Process.Start(new ProcessStartInfo(pathOfHelper));
                p.WaitForExit();
            }
        }

        private void service_EventCalled(string eventText)
        {
            Logger.WriteLine("Event called: " + eventText);
            if (EventCalled != null)
            {
                EventCalled(eventText);
            }
        }
    }
}
