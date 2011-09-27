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

        private WindowActionInfo actionInfo;

        [DataMember]
        public WindowActionInfo ActionInfo
        {
            get
            {
                return actionInfo;
            }
            set
            {
                actionInfo = value;
            }
        }

        private static HashSet<int> processBlackList = new HashSet<int>();

        public WindowSequence()
        {
            actionInfo = new WindowActionInfo();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new WindowSequenceConfig(actionInfo); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WindowSequenceConfig config = (WindowSequenceConfig)configurationControl;
            actionInfo = config.ActionInfo;
        }

        public string GetConfigString()
        {
            return actionInfo.WindowInfo.Title;
        }

        public override void Perform()
        {
            WindowFinder.Find(actionInfo, new WindowFinder.WindowActionResult((hwnd) =>
                {
                    foreach (WindowAction action in actionInfo.WindowActions)
                    {
                        action.Perform(hwnd);
                        Thread.Sleep(50);
                    }
                }));
        }
    }
}
