using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Runtime.Serialization;

namespace MayhemApp.Business_Logic
{
    [Serializable]
    class MayhemDebugAction : MayhemAction, ISerializable
    {


        public MayhemDebugAction(SerializationInfo info, StreamingContext context) : base( info,  context){}

        public MayhemDebugAction(String s ) : this() { }

        public MayhemDebugAction()
            : base(  "Debug Action",
                     "Generates debug output when triggered",
                     "This action generates debug output when triggered. Mainly for devloper use.")
        {
            
           
        }

        public override void PerformAction(MayhemTrigger sender)
        {
            Debug.WriteLine("MayhemDebugAction " + ID + " got triggered!");
        }

       
    }

    [Serializable]
    class MayhemDebugPopupAction : MayhemAction, ISerializable
    {

        /**<summary>
         * 
         * Makes a base-compatible constructor.
         * Ignores the input string!
         * </summary>
         **/

        public MayhemDebugPopupAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public MayhemDebugPopupAction(string s) : this(){}

        public MayhemDebugPopupAction()
            : base(     "Popup Action",
                        "Popup Window appears",
                        "This Action shows a popup window when triggered. Mainly used for testing." )
        {

        }

        public override void PerformAction(MayhemTrigger sender)
        {
            //Debug.WriteLine("MayhemDebugAction " + ID + " got triggered!");

            MessageBox.Show("I Got Triggered!"); 
        }

        /*
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }*/
    }

}
