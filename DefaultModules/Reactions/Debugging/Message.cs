using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;

namespace DefaultModules.Reactions.Debugging
{
    [Serializable]
    public class Message: ReactionBase, IWpf
    {
        string message = "Debug Reaction was triggered";
        public Message()
            :base("Debug: Message", "Generates debug output when triggered")
        {
            Setup();
        }

        public void Setup()
        {
            hasConfig = true;

            SetConfigString();
        }

        public override void Perform()
        {
            Debug.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), message));
        }

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

        private void SetConfigString()
        {
            ConfigString = message;
        }

        #region Serialization
        public Message(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion

        
    }
}
