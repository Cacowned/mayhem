using System;
using MayhemCore;
using SKYPE4COMLib;
using SkypeModules.Resources;

namespace SkypeModules.Events
{
    /// <summary>
    /// An event that will be triggered when an SMS is received.
    /// </summary>
    [MayhemModule("Skype: Receive SMS", "Triggers when an SMS is received")]
    public class SmsReceived : SkypeEventBase
    {
        private _ISkypeEvents_SmsMessageStatusChangedEventHandler smsMessageStatusChangedEventHandler;

        protected override void OnAfterLoad()
        {
            smsMessageStatusChangedEventHandler = Skype_SmsMessageStatusChanged;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);

            if (!e.Cancel)
            {
                skype.SmsMessageStatusChanged += smsMessageStatusChangedEventHandler;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (skype != null)
            {
                skype.SmsMessageStatusChanged -= smsMessageStatusChangedEventHandler;
            }
        }

        private void Skype_SmsMessageStatusChanged(SmsMessage pMessage, TSmsMessageStatus Status)
        {
            try
            {
                if (Status.Equals(TSmsMessageStatus.smsMessageStatusReceived))
                {
                    Trigger();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.Error_EventCantBeTriggered, Strings.ReceiveSms_Title));
                Logger.Write(ex);
            }
        }
    }
}
