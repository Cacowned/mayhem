using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;
using OOutlook = Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;

namespace DefaultModules.Actions.Office.Outlook
{
    [Serializable]
    public class OutlookMail : ActionBase, ISerializable
    {
        protected OOutlook.Application outlook;
        protected OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

        public OutlookMail()
            : base("Outlook New Mail", "Triggers when a new email is received") {

            SetUp();

        }

        protected void SetUp() {

            outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
            mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail); 
        }

        private void GotMail() {
            base.OnActionActivated();
        }


        public override void Enable() {
            base.Enable();

            outlook.NewMail  += mailEvent;

        }

        public override void Disable() {
            base.Disable();

            outlook.NewMail -= mailEvent;
        }

        #region Serialization
        public OutlookMail(SerializationInfo info, StreamingContext context)
            : base(info, context) {

            SetUp();
        }
        #endregion
    }
}
