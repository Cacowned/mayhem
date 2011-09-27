using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using WindowModules.Wpf;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowModules
{
    [DataContract]
    [MayhemModule("Window Sequence", "Window Sequence")]
    public class WindowSequence : ReactionBase, IWpfConfigurable
    {
        static readonly ulong TARGETWINDOW = Native.WS_BORDER | Native.WS_VISIBLE;

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
