using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using MayhemWpf.UserControls;
using VisionModules.Helpers;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("HTTP Image Server", "Runs a simple http server and updates the image when triggered")]
    public class ImageServer : ReactionBase
    {
        /// <summary>
        /// Port of the Server 
        /// </summary>
        [DataMember]
        private int port;

        private HTTPImageServer server;

        protected override void OnLoadDefaults()
        {
            port = 8080;
           
        }

        protected override void OnAfterLoad()
        {
            server = new HTTPImageServer(port);
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            server.StartServer();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            server.StopServer();
        }

        public override void  Perform()
        {
 	        //update server with current camera bitmap
        }

        /*
        public WpfConfiguration  ConfigurationControl
        {
	        get { throw new NotImplementedException(); }
        }

        public void  OnSaved(WpfConfiguration configurationControl)
        {
 	        throw new NotImplementedException();
        }

        public string  GetConfigString()
        {
 	        throw new NotImplementedException();
        }*/
    }
}
