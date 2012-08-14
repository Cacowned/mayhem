using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a Skype call is received.
    /// </summary>
    [MayhemModule("Skype: Call Received", "Triggers when a call is received")]
    public class CallReceived : SkypeCallStatusBase
    {
        protected override void Skype_CallStatus(Call pCall, TCallStatus Status)
        {
            try
            {
                if (Status.Equals(TCallStatus.clsRinging) && (pCall.Type.Equals(TCallType.cltIncomingP2P) || pCall.Type.Equals(TCallType.cltIncomingPSTN)))
                {
                    Trigger();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_EventCantBeTriggered, Strings.ReceiveCall_Title));
                Logger.Write(ex);
            }
        }
    }
}
