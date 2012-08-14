using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a Skype call is put on hold.
    /// </summary>
    [MayhemModule("Skype: Call On Hold", "Triggers when a call is put on hold")]
    public class CallOnHold : SkypeCallStatusBase
    {
        protected override void Skype_CallStatus(Call pCall, TCallStatus Status)
        {
            try
            {
                if (Status.Equals(TCallStatus.clsOnHold) || Status.Equals(TCallStatus.clsLocalHold))
                {
                    Trigger();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_EventCantBeTriggered, Strings.OnHoldCall_Title));
                Logger.Write(ex);
            }
        }
    }
}
