using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using KinectModules.Wpf;
using System.Runtime.Serialization;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Media;

namespace KinectModules
{
    [DataContract]
    [MayhemModule("Kinect Event", "Triggers when a Kinect gesture is detected")]
    public class KinectSkeletalGesture : EventBase, IWpfConfigurable
    {

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            {
                return new KinectEventConfig();
            }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            throw new NotImplementedException();
        }

        public string GetConfigString()
        {
            throw new NotImplementedException();
        }

        

    }
}
