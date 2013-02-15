using System;
using System.Globalization;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Http Get Request", "Launches an HTTP GET request to the specified Url")]
    public class HttpGet : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string Url
        {
            get;
            set;
        }

        public override void Perform()
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(Url);
                request.Timeout = 2222;
                System.Net.WebResponse response = request.GetResponse();
                System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream());
                ErrorLog.AddError(ErrorType.Message, stream.ReadToEnd().Trim());
            }
            catch
            { 

            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new OpenUrlConfig(Url); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as OpenUrlConfig;

            Url = config.Url;
        }

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.OpenUrl_ConfigString, Url);
        }
    }
}