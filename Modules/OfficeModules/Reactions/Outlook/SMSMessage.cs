using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using OfficeModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using OOutlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace OutlookModules.Reactions.Outlook
{
    [DataContract]
    [MayhemModule("SMS Message", "Sends a text message when triggered")]
    public class SmsMessage : ReactionBase, IWpfConfigurable
    {
        //list of US provider gateways: http://hacknmod.com/hack/email-to-text-messages-for-att-verizon-t-mobile-sprint-virgin-more/
        #region Configuration
        private string To
        {
            get;
            set;
        }

        private string Message
        {
            get;
            set;
        }

        private string CarrierString
        {
            get;
            set;
        }

        #endregion
        protected Dictionary<string, string> carriers;

        protected override void Initialize()
        {
            base.Initialize();

            carriers = new Dictionary<string, string>();

            carriers.Add("ATT", "@txt.att.net");
            carriers.Add("Verizon", "@vtext.com");
            carriers.Add("T-Mobile", "@tmomail.net");
            carriers.Add("Sprint", "@messaging.sprintpcs.com");

            To = "";
            Message = "";
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("To: {0}{1}\nMessage: {2}", To, CarrierString, Message);
        }

        public override void Perform()
        {
            try
            {
                // Create the Outlook application.
                OOutlook.Application oApp = new OOutlook.Application();

                // Get the NameSpace and Logon information.
                OOutlook.NameSpace oNS = oApp.GetNamespace("mapi");

                // Log on by using a dialog box to choose the profile.
                oNS.Logon(Missing.Value, Missing.Value, true, true);

                // Alternate logon method that uses a specific profile.
                // TODO: If you use this logon method, 
                //  change the profile name to an appropriate value.
                //oNS.Logon("YourValidProfile", Missing.Value, false, true); 

                // Create a new mail item.
                OOutlook.MailItem oMsg = (OOutlook.MailItem)oApp.CreateItem(OOutlook.OlItemType.olMailItem);


                oMsg.Body = Message;

                // Add a recipient.
                OOutlook.Recipients oRecips = (OOutlook.Recipients)oMsg.Recipients;
                // TODO: Change the recipient in the next line if necessary.
                OOutlook.Recipient oRecip = (OOutlook.Recipient)oRecips.Add(To + CarrierString);
                oRecip.Resolve();

                // Send. 

                ((Microsoft.Office.Interop.Outlook._MailItem)oMsg).Send();

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

        public IWpfConfiguration ConfigurationControl
        {
            get { return new SmsMessageConfig(To, Message, carriers); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            To = ((SmsMessageConfig)configurationControl).To;
            Message = ((SmsMessageConfig)configurationControl).Message;
            CarrierString = ((SmsMessageConfig)configurationControl).CarrierString;
            SetConfigString();
        }
    }
}
