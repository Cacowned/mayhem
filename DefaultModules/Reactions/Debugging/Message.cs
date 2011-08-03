using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;

namespace DefaultModules.Reactions.Debugging
{
    public class Message : ReactionBase, IWpf
    {
        string message = "Debug Reaction was triggered";
        public Message()
            : base("Debug: Message", "Generates debug output when triggered")
        {
            hasConfig = true;

            SetConfigString();
        }

        
        
        public override void Perform()
        {
            Debug.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message));

            UserControl asdf = new DebugMessageConfig(message);
        }
        /*
        public void WpfConfig()
        {
            var window = new DebugMessageConfig(message);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                message = window.message;

                SetConfigString();
            }
        }
        */

        public void OnSaved(UserControl configurationControl)
        {
        }

        private void SetConfigString()
        {
            ConfigString = message;
        }

        public UserControl ConfigurationControl
        {
            get { return new DebugMessageConfig(message); }
        }
    }
}
