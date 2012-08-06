using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SKYPE4COMLib;
using SkypeModules.Resources;
using SkypeModules.Wpf;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// A class that will resume the call with the user identified by the predefined Skype ID.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Unhold Call", "Resumes the call with the user identified by the selected Skype ID")]
    public class UnholdCall : SkypeIDWpfBase, IWpfConfigurable
    {
        public override void Perform()
        {
            bool isCallActive = false;
            bool isOnHold = false;

            try
            {
                foreach (Call call in skype.ActiveCalls)
                {
                    if (call.PartnerHandle.Equals(skypeID))
                    {
                        isCallActive = true;

                        if (call.Status.Equals(TCallStatus.clsLocalHold) || call.Status.Equals(TCallStatus.clsOnHold))
                        {
                            isOnHold = true;
                            call.Resume();

                            break;
                        }
                    }
                }

                if (!isCallActive)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallNotActive, skypeID));
                }
                else
                    if (!isOnHold)
                    {
                        // A call is initializated between the user and the selected skype ID but the call is not on hold.
                        ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallNotOnHold, skypeID));
                    }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallCantResume, skypeID));
                Logger.Write(ex);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeIDConfig(skypeID, Strings.UnholdCall_Title); }
        }
    }
}
