using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using WindowModules.Wpf;
using System.Diagnostics;

namespace WindowModules
{
    [DataContract]
    [MayhemModule("Move Window", "Move a window")]
    public class MoveWindow : ReactionBase, IWpfConfigurable
    {
        WindowActionInfo selectedWindow;

        [DataMember]
        public WindowActionInfo SelectedWindow
        {
            get
            {
                return selectedWindow;
            }
            set
            {
                selectedWindow = value;
            }
        }

        [DataMember]
        private bool shouldBalls;

        [DataMember]
        public bool shouldBallsPublic;

        [DataMember]
        public bool shouldBallsProp
        {
            get;
            set;
        }

        public MoveWindow()
        {
            selectedWindow = new WindowActionInfo();
            shouldBalls = true;
            shouldBallsPublic = true;
            shouldBallsProp = true;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new MoveWindowConfig(SelectedWindow); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            MoveWindowConfig config = (MoveWindowConfig)configurationControl;
            SelectedWindow = config.SelectedWindow;
            ConfigString = "asdf";
        }

        public override void Perform()
        {
            Process[] pArr = Process.GetProcesses();
            foreach (Process p in pArr)
            {
                try
                {
                    if (p.MainModule.FileName.ToLower().EndsWith(SelectedWindow.WindowInfo.FileName.ToLower()))
                    {
                        if (SelectedWindow.ShouldMove)
                        {
                            IntPtr handle = p.MainWindowHandle;
                            Native.RECT Rect = new Native.RECT();
                            Native.GetWindowRect(handle, ref Rect);
                            int width = Rect.right - Rect.left;
                            int height = Rect.bottom - Rect.top;

                            Native.MoveWindow(handle, SelectedWindow.MoveX, SelectedWindow.MoveY, width, height, true);
                            break;
                        }
                    }
                }
                catch { }
            }
        }
    }
}
