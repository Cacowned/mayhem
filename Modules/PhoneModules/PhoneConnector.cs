using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.ServiceModel.Web;
using System.IO;
using NetFwTypeLib;
using System.Windows;
using System.Reflection;

namespace PhoneModules
{
    public class PhoneConnector
    {
        const int PortNumber = 19283;
        bool isServiceRunning = false;
        int refCount = 0;
        WebServiceHost host;
        IMayhemService service = null;

        public delegate void EventCalledHandler(string eventText);
        public event EventCalledHandler EventCalled;

        string formData = null;
        public string FormData
        {
            get
            {
                return formData;
            }
            set
            {
                formData = value;
                if (service != null)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
                        {
                            if (service != null)
                            {
                                string insideDiv;
                                string html = PhoneLayout.Instance.SerializeToHtml(out insideDiv);
                                service.SetHtml(html);
                                service.SetInsideDiv(insideDiv);
                                //service.SetFormData(formData);
                            }
                        }));
                }
            }
        }

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
        static readonly PhoneConnector instance = new PhoneConnector();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PhoneConnector()
        {
        }

        PhoneConnector()
        {
            MayhemEntry.Instance.ShuttingDown += new EventHandler(Mayhem_ShuttingDown);
        }

        void Mayhem_ShuttingDown(object sender, EventArgs e)
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

        public bool Enable()
        {
            refCount++;
            if (!isServiceRunning)
            {
                isServiceRunning = true;
                return StartService();
            }
            return true;
        }


        public void Disable()
        {
            //if (isServiceRunning)
            //{
            //    refCount--;
            //    if (refCount == 0)
            //    {
            //        if (service != null)
            //        {
            //            service.ShuttingDown();
            //        }
            //        //isServiceRunning = false;
            //        //host.Close();
            //        service = null;
            //    }
            //}
        }

        private bool StartService()
        {
            Uri address = new Uri("http://localhost:"+PortNumber+"/Mayhem");

            MayhemService svc = new MayhemService();
            svc.EventCalled += new MayhemService.EventCalledHandler(service_EventCalled);
            if (formData != null)
            {
                string insideDiv;
                string html = PhoneLayout.Instance.SerializeToHtml(out insideDiv);
                svc.SetHtml(html);
                svc.SetInsideDiv(insideDiv);
            }

            host = new WebServiceHost(svc, address);

            // Enable Mex
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            WebHttpBinding binding = new WebHttpBinding();
		    binding.ReaderQuotas.MaxArrayLength = 2147483647;

            host.AddServiceEndpoint(typeof(IMayhemService), binding, "");

            bool portFound = false;
            INetFwOpenPorts ports;
            INetFwOpenPort port;

            Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);
            ports = (INetFwOpenPorts)mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts;
            System.Collections.IEnumerator enumerate = ports.GetEnumerator();
            while (enumerate.MoveNext())
            {
                port = (INetFwOpenPort)enumerate.Current;
                if (port.Port == PortNumber)
                    portFound = true;
            }
            if (!portFound)
            {
                OpenServerHelper();
            }

            try
            {
                host.Open();
            }
            catch (System.ServiceModel.AddressAccessDeniedException)
            {
                OpenServerHelper();
                try
                {
                    host.Open();
                }
                catch 
                {
                    isServiceRunning = false;
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(e);
            }
            isServiceRunning = true;
            Logger.WriteLine("Phone service started");

            WebChannelFactory<IMayhemService> myChannelFactory = new WebChannelFactory<IMayhemService>(new Uri(address.ToString()));
            service = myChannelFactory.CreateChannel();

            return true;
        }

        void OpenServerHelper()
        {
            FileInfo fi = new FileInfo(Assembly.GetCallingAssembly().Location);
            string pathOfHelper = Path.Combine(fi.DirectoryName, "PhoneServerHelper.exe");
            if (File.Exists(pathOfHelper))
            {
                MessageBox.Show("Mayhem will automatically configure your network settings. Say yes.", "Mayhem Phone Modules");

                Process p = Process.Start(new ProcessStartInfo(pathOfHelper));
                p.WaitForExit();
            }
        }

        void service_EventCalled(string eventText)
        {
            Logger.WriteLine("Event called: " + eventText);
            if (EventCalled != null)
            {
                EventCalled(eventText);
            }
        }
    }
}
