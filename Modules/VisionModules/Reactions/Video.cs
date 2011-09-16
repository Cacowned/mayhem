/*
 * Video.cs
 * 
 * This vision module records an avi video of the camera before or after an event has fired.
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 *
 * Parts of the code make use of functions from the AviFile library by Corrina John
 * http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Video", "Records an avi video of the camera scene before or after an event has fired")]
    public class Video : ReactionBase, IWpfConfigurable
    {

        public override void Perform()
        {
            throw new NotImplementedException();
        }

        public MayhemWpf.UserControls.IWpfConfiguration ConfigurationControl
        {
            get { throw new NotImplementedException(); }
        }

        public void OnSaved(MayhemWpf.UserControls.IWpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }
    }
}
