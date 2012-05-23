using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using DisplayWindowModule.Resources;
using DisplayWindowModuleWpf.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DisplayWindowModuleWpf.Reactions
{
    [DataContract]
    [MayhemModule("Display Window", "Displays a message")]
    public class DisplayWindow : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string message;

        [DataMember]
        private int seconds;

        private void CreateWindow()
        {
            try
            {
                MessagWindow msgWindow = new MessagWindow(message, seconds);
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

        public WpfConfiguration ConfigurationControl
        {
            get { return new DisplayWindowConfig(message, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as DisplayWindowConfig;

            if (config == null)
                return;

            message = config.Message;
            seconds = config.Seconds;
        }

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DisplayWindow_ConfigString, message);
        }
    }
}
