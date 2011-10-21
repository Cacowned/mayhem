using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using MayhemWpf.UserControls;
using VisionModules.Helpers;
using MayhemOpenCVWrapper;
using System.Runtime.CompilerServices;
using MayhemOpenCVWrapper.LowLevel;
using System.Drawing;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("HTTP Image Server", "Runs a simple http server and updates the image when triggered")]
    public class ImageServer : ReactionBase
    {
        ImagerBase cam; 
        /// <summary>
        /// Port of the Server 
        /// </summary>
        [DataMember]
        private int port;

        [DataMember]
        private int cameraIndex; 

        private HTTPImageServer server;
        private DummyCameraImageListener dummy ; 

        protected override void OnLoadDefaults()
        {
            port = 8080;
            cameraIndex = 0;
        }

        protected override void OnAfterLoad()
        {
            
            // get a camera
            CameraDriver cDriver = CameraDriver.Instance;

            // camera listener
            dummy = new DummyCameraImageListener();

            if (cDriver.CamerasAvailable.Count > 0 && cDriver.CamerasAvailable.Count - 1 >= cameraIndex)
            {
                cam = cDriver.CamerasAvailable[cameraIndex];
            }
            else
            {
                cam = new DummyCamera() ;
            }

           
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            dummy.RegisterForImages(cam);
            cam.StartFrameGrabbing();
            if (server == null)
                server = HTTPImageServer.ServerForCamera(cam);
            server.StartServer();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            dummy.UnregisterForImages(cam);
            server.StopServer();
            cam.TryStopFrameGrabbing();
        }

        public override void  Perform()
        {
 	        //update server with current camera bitmap
            Bitmap currentImage = cam.ImageAsBitmap();
            currentImage.Save("mayhem--preserve.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            server.UpdateImage(currentImage);
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
