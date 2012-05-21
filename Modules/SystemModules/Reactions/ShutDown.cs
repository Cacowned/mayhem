using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SystemModules.Wpf;

namespace SystemModules.Reactions
{
    [DataContract]
    [MayhemModule("Shut Down", "Shuts the computer Down")]
    public class Shutdown : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private bool forceShutDown
        {
            get;
            set; 
        }
        public override void Perform()
        {
            Application.Exit(); //experienced instances of unsaved settings before shutdown crashing mayhem after startup. Adding this to save settings.
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
            return null;
        }
        public void OnSaved(WpfConfiguration configurationControl)
        {
             forceShutDown = ((ShutDownWarning)configurationControl)._forceShutDown;
        }
    }
}
