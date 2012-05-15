using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using MayhemWpf.UserControls;
using DisplayWindowModuleWpf.Wpf;
using System.Globalization;
using DisplayWindowModule.Resources;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace DisplayWindowModuleWpf.Reactions
{
    [DataContract]
    [MayhemModule("Display Window", "Displays a message")]
    public class DisplayWindow : ReactionBase, IWpfConfigurable
    {
        private void CreateWindow()
        {
            try
            {
                MessagWindow msgWindow = new MessagWindow(Message, Seconds);
                msgWindow.ShowDialog();               
            }
            catch 
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.DispayWindow_CantCreateWindow);
            } 
        }        

        public override void Perform()
        {
            try
            {
                Thread thread = new Thread(CreateWindow);
                thread.SetApartmentState(ApartmentState.STA); //set the thread to STA 
                thread.Start();

                thread.Join();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.DisplayWindow_CantPerformReaction);
            }            
        }

        [DataMember]
        public string Message
        {
            get;
            set;
        }

        [DataMember]
        public int Seconds
        {
            get;
            private set;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new DisplayWindowConfig(Message, Seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as DisplayWindowConfig;

            if (config == null)
                return;

            Message = config.Message;
            Seconds = config.Seconds;
        }

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DisplayWindow_ConfigString, Message);
        }
    }
}
