using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using WindowModules.Wpf;

namespace WindowModules
{
    [DataContract]
    [MayhemModule("Window Sequence", "Window Sequence")]
    public class WindowSequence : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        public WindowActionInfo ActionInfo
        {
            get;
            private set;
        }

        private static HashSet<int> processBlackList = new HashSet<int>();

        public WindowSequence()
        {
            ActionInfo = new WindowActionInfo();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WindowSequenceConfig(ActionInfo); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WindowSequenceConfig config = (WindowSequenceConfig)configurationControl;
            ActionInfo = config.ActionInfo;
        }

        public string GetConfigString()
        {
            return ActionInfo.WindowInfo.Title;
        }

        public override void Perform()
        {
            WindowFinder.Find(ActionInfo, new WindowFinder.WindowActionResult((hwnd) =>
                {
                    foreach (WindowAction action in ActionInfo.WindowActions)
                    {
                        action.Perform(hwnd);
                        Thread.Sleep(50);
                    }
                }));
        }
    }
}
