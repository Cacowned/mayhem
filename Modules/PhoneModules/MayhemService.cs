using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.Reflection;

namespace PhoneModules
{
    [ServiceContract(Name = "IMayhemService")]
    public interface IMayhemService
    {
        [OperationContract]
        [WebGet(UriTemplate = "Event/{text}", BodyStyle=WebMessageBodyStyle.Bare)]
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
    }

    [ServiceBehavior(Name = "MayhemService", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MayhemService : IMayhemService
    {
        string formData = null;
        string nextData = null;
        string html = null;
        string htmlWP7 = null;
        string htmlIOS = null;
        string htmlAndroid = null;
        string insideDiv = null;
        object locker = new object();

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        Dictionary<string, AutoResetEvent> resetEvents = new Dictionary<string, AutoResetEvent>();

        public Stream Images(string id)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\" + id + ".png");
            return new FileStream(file, FileMode.Open, FileAccess.Read);
        }

        public Stream Html(bool update)
        {
            if (html == null)
                return null;

            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            Debug.WriteLine(update + " " + WebOperationContext.Current.IncomingRequest.UserAgent);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            if (!resetEvents.ContainsKey(endpointProperty.Address))
            {
                AutoResetEvent a = new AutoResetEvent(false);
                resetEvents[endpointProperty.Address] = a;
            }
            if (!update)
            {
                resetEvents[endpointProperty.Address].Reset();

                string userAgent = WebOperationContext.Current.IncomingRequest.UserAgent;
                if(userAgent.IndexOf("iPhone") >= 0 || userAgent.IndexOf("iPad") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlIOS));
                else if (userAgent.IndexOf("Android") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlAndroid));
                else if (userAgent.IndexOf("Windows Phone") >= 0)
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlWP7));
                else
                    return new MemoryStream(ASCIIEncoding.Default.GetBytes(htmlWP7));
            }
            else if (resetEvents[endpointProperty.Address].WaitOne(5000))
            {
                return new MemoryStream(ASCIIEncoding.Default.GetBytes(insideDiv));
            }
            return null;
        }
        public void Event(string text)
        {
            Debug.WriteLine("Event " + text);
            if (EventCalled != null)
            {
                EventCalled(text);
            }
        }

        public void SetHtml(string html)
        {
            this.html = html;

            htmlWP7 = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("wp7"));
            htmlIOS = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("ios"));
            htmlAndroid = html.Replace("%%INSERTSTYLEHERE%%", GetCSSForDevice("android"));
        }

        string GetCSSForDevice(string device)
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
                Debug.WriteLine(erf);
            }
            return css;
        }

        public void SetInsideDiv(string insideDiv)
        {
            this.insideDiv = insideDiv;
            foreach (AutoResetEvent events in resetEvents.Values)
            {
                events.Set();
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

        public delegate void EventCalledHandler(string eventText);
        public event EventCalledHandler EventCalled;
    }
}
