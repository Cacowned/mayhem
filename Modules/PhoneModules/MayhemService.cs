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
    [ServiceBehavior(Name = "MayhemService", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MayhemService : IMayhemService
    {
        private readonly Dictionary<string, string> cssDict;
        private string html;
        private string htmlWp7;
        private string htmlIPhone;
        private string htmlIPad;
        private string htmlAndroid;
        private string insideDiv;
        private bool isShuttingDown;

        private readonly ManualResetEvent killResetEvent;
        private int numToKill;

        private readonly Dictionary<string, AutoResetEvent> resetEvents;

        public MayhemService()
        {
            cssDict = new Dictionary<string, string>();
            killResetEvent = new ManualResetEvent(false);
            resetEvents = new Dictionary<string, AutoResetEvent>();
        }

        public Stream Images(string id)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/png";
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images\\" + id + ".png");
            return new FileStream(file, FileMode.Open, FileAccess.Read);
        }

        public Stream Html(bool update)
        {
            if (isShuttingDown)
                return new MemoryStream(Encoding.Default.GetBytes("kill"));
            
            if (html == null)
                return null;

            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string key = endpointProperty.Address + WebOperationContext.Current.IncomingRequest.UserAgent;

            Logger.WriteLine(update + " " + key + " " + WebOperationContext.Current.IncomingRequest.UserAgent);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";

            lock (resetEvents)
            {
                if (!resetEvents.ContainsKey(key))
                {
                    AutoResetEvent a = new AutoResetEvent(false);
                    resetEvents[key] = a;
                    update = false;
                }
            }

            Logger.WriteLine(update);
            if (!update)
            {
                resetEvents[key].Reset();

                string userAgent = WebOperationContext.Current.IncomingRequest.UserAgent;
                if (userAgent == null)
                    return new MemoryStream(Encoding.Default.GetBytes(htmlWp7));

                if (userAgent.IndexOf("iPhone") >= 0)
                    return new MemoryStream(Encoding.Default.GetBytes(htmlIPhone));

                if (userAgent.IndexOf("iPad") >= 0)
                    return new MemoryStream(Encoding.Default.GetBytes(htmlIPad));

                if (userAgent.IndexOf("Android") >= 0)
                    return new MemoryStream(Encoding.Default.GetBytes(htmlAndroid));

                if (userAgent.IndexOf("Windows Phone") >= 0)
                    return new MemoryStream(Encoding.Default.GetBytes(htmlWp7));
                
                return new MemoryStream(Encoding.Default.GetBytes(htmlWp7));
            }
            else
            {
                Interlocked.Increment(ref numToKill);
                if (resetEvents[key].WaitOne(10000))
                {
                    Interlocked.Decrement(ref numToKill);
                    if (isShuttingDown)
                    {
                        if (numToKill == 0)
                            killResetEvent.Set();

                        return new MemoryStream(Encoding.Default.GetBytes("kill"));
                    }
                    else
                        return new MemoryStream(Encoding.Default.GetBytes(insideDiv));
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

            htmlWp7 = html.Replace("%%INSERTSTYLEHERE%%", GetCssForDevice("wp7"));
            htmlIPhone = html.Replace("%%INSERTSTYLEHERE%%", GetCssForDevice("iphone"));
            htmlIPad = html.Replace("%%INSERTSTYLEHERE%%", GetCssForDevice("ipad"));
            htmlAndroid = html.Replace("%%INSERTSTYLEHERE%%", GetCssForDevice("android"));
        }

        private string GetCssForDevice(string device)
        {
            if (!cssDict.ContainsKey(device))
            {
                try
                {
                    Assembly assembly = GetType().Assembly;
                    string css;
                    using (Stream stream = assembly.GetManifestResourceStream("PhoneModules.css-" + device + ".html"))
                    {
                        using (StreamReader textStreamReader = new StreamReader(stream))
                        {
                            css = textStreamReader.ReadToEnd();
                        }
                    }

                    cssDict[device] = css;
                    return css;
                }
                catch (Exception erf)
                {
                    Logger.WriteLine(erf);
                }
            }

            return cssDict[device];
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

        public void ShuttingDown()
        {
            isShuttingDown = true;
            if (numToKill > 0)
            {
                Debug.WriteLine(numToKill);
                foreach (AutoResetEvent ev in resetEvents.Values)
                {
                    ev.Set();
                }

                killResetEvent.WaitOne(5000);
                Thread.Sleep(500);
            }
        }

        public delegate void EventCalledHandler(string eventText);

        public event EventCalledHandler EventCalled;
    }
}
