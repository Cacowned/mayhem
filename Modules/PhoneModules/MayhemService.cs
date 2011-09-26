using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using MayhemCore;

namespace PhoneModules
{
    [ServiceContract(Name = "IMayhemService")]
    public interface IMayhemService
    {
        [OperationContract]
        [WebGet(UriTemplate = "Event/{text}", BodyStyle = WebMessageBodyStyle.Bare)]
        void Event(string text);

        [OperationContract]
        [WebInvoke]
        void SetHtml(string html);

        [OperationContract]
        [WebInvoke]
        void SetInsideDiv(string insideDiv);

        [OperationContract]
        [WebGet]
        Stream Html(bool update);

        [OperationContract]
        [WebGet(UriTemplate = "Images/{id}")]
        Stream Images(string id);

        [OperationContract]
        [WebGet]
        void ShuttingDown();
    }

    [ServiceBehavior(Name = "MayhemService", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MayhemService : IMayhemService
    {
        private string html = null;
        private string htmlWP7 = null;
        private string htmlIPhone = null;
        private string htmlIPad = null;
        private string htmlAndroid = null;
        private string insideDiv = null;
        private object locker = new object();
        private bool isShuttingDown = false;

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
        private ManualResetEvent killResetEvent = new ManualResetEvent(false);
        private int numToKill = 0;

        private Dictionary<string, AutoResetEvent> resetEvents = new Dictionary<string, AutoResetEvent>();

        public Stream Images(string id)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\" + id + ".png");
            return new FileStream(file, FileMode.Open, FileAccess.Read);
        }

        public Stream Html(bool update)
        {
            if (isShuttingDown)
                return new MemoryStream(ASCIIEncoding.Default.GetBytes("kill"));
            else if (html == null)
                return null;

            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string key = endpointProperty.Address + ":" + endpointProperty.Port;

            Logger.WriteLine(update + " " + key + " " + WebOperationContext.Current.IncomingRequest.UserAgent);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            if (!resetEvents.ContainsKey(key))
            {
                AutoResetEvent a = new AutoResetEvent(false);
                resetEvents[key] = a;
            }
            //else
            //{
            //    resetEvents[key].Set();
            //}
            Debug.WriteLine(resetEvents.Count);
            if (!update)
            {
                resetEvents[key].Reset();

                string userAgent = WebOperationContext.Current.IncomingRequest.UserAgent;
                if (userAgent.IndexOf("iPhone") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlIPhone));
                else if (userAgent.IndexOf("iPad") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlIPad));
                else if (userAgent.IndexOf("Android") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlAndroid));
                else if (userAgent.IndexOf("Windows Phone") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlWP7));
                else
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlWP7));
            }
            else
            {
                Interlocked.Increment(ref numToKill);
                if (resetEvents[key].WaitOne(10000))
                {
                    Interlocked.Decrement(ref numToKill);
                    if (isShuttingDown)
                    {
                        Logger.WriteLine("Killed service " + numToKill);
                        if (numToKill == 0)
                            killResetEvent.Set();
                        return new MemoryStream(ASCIIEncoding.Default.GetBytes("kill"));
                    }
                    else
                        return new MemoryStream(ASCIIEncoding.Default.GetBytes(insideDiv));
                }
                else
                    Interlocked.Decrement(ref numToKill);
            }
            return null;
        }

        public void Event(string text)
        {
            Logger.WriteLine("Event " + text);
            if (EventCalled != null)
            {
                EventCalled(text);
            }
        }

        public void SetHtml(string html)
        {
            this.html = html;

            htmlWP7 = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("wp7"));
            htmlIPhone = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("iphone"));
            htmlIPad = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("ipad"));
            htmlAndroid = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("android"));
        }

        private string GetCSSForDevice(string device)
        {
            string css = "";
            try
            {
                Assembly _assembly = this.GetType().Assembly;
                using (Stream stream = _assembly.GetManifestResourceStream("PhoneModules.css-" + device + ".html"))
                {
                    using (StreamReader _textStreamReader = new StreamReader(stream))
                    {
                        css = _textStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception erf)
            {
                Logger.WriteLine(erf);
            }
            return css;
        }

        public void SetInsideDiv(string insideDiv)
        {
            this.insideDiv = insideDiv;
            Debug.WriteLine(resetEvents.Count);
            foreach (AutoResetEvent ev in resetEvents.Values)
            {
                ev.Set();
            }
        }

        private byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public void ShuttingDown()
        {
            isShuttingDown = true;
            if (numToKill > 0)
            {
                //numToKill = resetEvents.Count;
                Debug.WriteLine(numToKill);
                foreach (AutoResetEvent ev in resetEvents.Values)
                {
                    ev.Set();
                }

                killResetEvent.WaitOne(5000);

                Thread.Sleep(100);
            }
        }

        public delegate void EventCalledHandler(string eventText);
        public event EventCalledHandler EventCalled;
    }
}
