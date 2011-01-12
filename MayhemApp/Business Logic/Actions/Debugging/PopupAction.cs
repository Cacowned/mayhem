using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;

namespace MayhemApp.Business_Logic.Actions.Debugging
{
    [Serializable]
    class PopupAction : MayhemActionBase, ISerializable
    {
        /**<summary>
         * 
         * Makes a base-compatible constructor.
         * Ignores the input string!
         * </summary>
         **/
        public PopupAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public PopupAction(string s) : this() { }

        public PopupAction()
            : base("Popup Action",
                        "Popup Window appears",
                        "This Action shows a popup window when triggered. Mainly used for testing.")
        {

        }

        public override void PerformAction(MayhemTriggerBase sender)
        {
            MessageBox.Show("I Got Triggered!");
        }
    }
}
