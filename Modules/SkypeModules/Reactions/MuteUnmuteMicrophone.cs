using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Reactions
{
    /// <summary>
    /// A class that will mute/unmute the microphone for the current Skype call.
    /// </summary>
    [MayhemModule("Skype: Mute/Unmute Microphone", "Mutes/unmutes the microphone for the current call")]
    public class MuteUnmuteMicrophone : SkypeReactionBase
    {
        public override void Perform()
        {
            try
            {
                if (skype.ActiveCalls.Count == 0)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Error_CantMuteUnmuteNoCall);
                }
                else
                {
                    ((ISkype)skype).Mute = !((ISkype)skype).Mute; // I must cast because it's an ambiguity between ISkype and ISkype_Events_Event.
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Error_CantMuteUnmute);
                Logger.Write(ex);
            }
        }
    }
}
