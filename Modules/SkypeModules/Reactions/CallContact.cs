using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SkypeModules.Resources;
using SkypeModules.Wpf;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// A class that will make a call to the predefined contact.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Call Contact", "Makes a call to the predefined contact")]
    public class CallContact : SkypeIDWpfBase, IWpfConfigurable
    {
        public override void Perform()
        {
            try
            {
                if (VerifySkypeIDValidity(skypeID))
                {
                    skype.PlaceCall(skypeID);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallingContact, skypeID));
                Logger.Write(ex);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeIDConfig(skypeID, Strings.CallContact_Title); }
        }
    }
}
