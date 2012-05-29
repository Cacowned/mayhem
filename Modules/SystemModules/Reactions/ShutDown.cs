using System.Diagnostics;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SystemModules.Wpf;

namespace SystemModules.Reactions
{
    [DataContract]
    [MayhemModule("Power: Shut Down", "Shuts the computer Down")]
    public class Shutdown : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private bool forceShutDown;

        public override void Perform()
        {
            if (forceShutDown)
            {
                Process.Start("shutdown", "/s /f /t 0");
            }
            else
            {
                Process.Start("shutdown", "/s /t 0");
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new ShutDownWarning(forceShutDown); }
        }

        public string GetConfigString()
        {
            if (forceShutDown)
                return "Enforced Shutdown";
            else
                return "Safe Shutdown";
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
             forceShutDown = ((ShutDownWarning)configurationControl).ForceShutDown;
        }
    }
}
