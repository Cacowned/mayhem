using MayhemCore;
using SKYPE4COMLib;

namespace SkypeModules.Events
{
    /// <summary>
    /// A base class for defining and setting the event handler for the CallStatus event.
    /// </summary>
    public abstract class SkypeCallStatusBase : SkypeEventBase
    {
        protected _ISkypeEvents_CallStatusEventHandler callStatusEventHandler;

        protected override void OnAfterLoad()
        {
            callStatusEventHandler = Skype_CallStatus;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                skype.CallStatus += callStatusEventHandler;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (skype != null)
            {
                skype.CallStatus -= callStatusEventHandler;
            }
        }

        protected abstract void Skype_CallStatus(Call pCall, TCallStatus Status);
    }
}
