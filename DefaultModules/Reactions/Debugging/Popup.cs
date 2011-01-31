using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;
using System.Windows;
using System.Runtime.Serialization;

namespace DefaultModules.Reactions.Debugging
{
    [Serializable]
    public class Popup : ReactionBase
    {
        public Popup()
            : base("Debug: Popup", "Generates a small popup window when triggered")
        {

        }
        public override void Perform()
        {
            MessageBox.Show("Triggered!");
        }

        #region Serialization
        public Popup(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
