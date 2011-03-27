using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class SmsMessage : ReactionBase, IWpf
    {
        //list of US provider gateways: http://hacknmod.com/hack/email-to-text-messages-for-att-verizon-t-mobile-sprint-virgin-more/

        protected string to;
        protected string msg;
        protected string carrierString;

        protected Dictionary<string, string> carriers;

        public SmsMessage()
            : base("SMS Message", "Sends a text message when triggered.")
        {

            to = "";
            msg = "Mayhem just triggered!";

            Setup();
        }

        public void Setup()
        {
            hasConfig = true;

            carriers = new Dictionary<string, string>();

            carriers.Add("ATT", "@txt.att.net");
            carriers.Add("Verizon", "@vtext.com");
            carriers.Add("T-Mobile", "@tmomail.net");
            carriers.Add("Sprint", "@messaging.sprintpcs.com");

            SetConfigString();
        }

        public void SetConfigString()
        {
            ConfigString = String.Format("To: {0}{1}\nMessage: {2}", to, carrierString, msg);
        }

        public override void Perform()
        {
            try
            {
                // Create the Outlook application.
                Outlook.Application oApp = new Outlook.Application();

                // Get the NameSpace and Logon information.
                Outlook.NameSpace oNS = oApp.GetNamespace("mapi");

                // Log on by using a dialog box to choose the profile.
                oNS.Logon(Missing.Value, Missing.Value, true, true);

                // Alternate logon method that uses a specific profile.
                // TODO: If you use this logon method, 
                //  change the profile name to an appropriate value.
                //oNS.Logon("YourValidProfile", Missing.Value, false, true); 

                // Create a new mail item.
                Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);


                oMsg.Body = msg;

                // Add a recipient.
                Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;
                // TODO: Change the recipient in the next line if necessary.
                Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(to + carrierString);
                oRecip.Resolve();

                // Send.
                oMsg.Send();

                // Log off.
                oNS.Logoff();

                // Clean up.
                oRecip = null;
                oRecips = null;
                oMsg = null;
                oNS = null;
                oApp = null;
            }

             // Simple error handling.
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public void WpfConfig()
        {
            var window = new SmsMessageConfig(to, msg, carriers);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                to = window.to;
                msg = window.msg;
                carrierString = window.carrierString;

                SetConfigString();
            }
        }

        #region Serialization
        public SmsMessage(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            to = info.GetString("To");
            msg = info.GetString("Message");
            carrierString = info.GetString("CarrierString");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("To", to);
            info.AddValue("Message", msg);
            info.AddValue("CarrierString", carrierString);
        }
        #endregion
    }
}
