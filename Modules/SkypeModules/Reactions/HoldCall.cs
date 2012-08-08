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
    /// A class that will put on hold the call with the user identified by the predefined Skype ID.
    /// </summary>
    [DataContract]
    [MayhemModule("Skype: Hold Call", "Puts the call with the user identified by the selected Skype ID")]
    public class HoldCall : SkypeIDWpfBase, IWpfConfigurable
    {
        public override void Perform()
        {
            bool inProgress = false;
            bool isCallActive = false;

            try
            {
                if (VerifySkypeIDValidity(skypeID))
                {
                    foreach (Call call in skype.ActiveCalls)
                    {
                        if (call.PartnerHandle.Equals(skypeID))
                        {
                            isCallActive = true;

                            if (call.Status.Equals(TCallStatus.clsInProgress))
                            {
                                inProgress = true;
                                call.Hold();

                                break;
                            }
                        }
                    }

                    if (!isCallActive)
                    {
                        ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallNotActive, skypeID));
                    }
                    else if (!inProgress)
                    {
                        // A call is initializated between the user and the selected Skype ID but the call is not in progress.
                        ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallNotInProgress, skypeID));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_CallCantPutOnHold, skypeID));
                Logger.Write(ex);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new SkypeIDConfig(skypeID, Strings.HoldCall_Title); }
        }
    }
}
