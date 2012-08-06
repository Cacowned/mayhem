using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a Skype call is ended.
    /// </summary>
    [MayhemModule("Skype: Call Ended", "A call is ended")]
    public class CallEnded : SkypeCallStatusBase
    {
        protected override void Skype_CallStatus(Call pCall, TCallStatus Status)
        {
            try
            {
                if (Status.Equals(TCallStatus.clsFinished))
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
