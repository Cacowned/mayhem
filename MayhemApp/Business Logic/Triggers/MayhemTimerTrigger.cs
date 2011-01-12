using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Runtime.Serialization;


namespace MayhemApp.Business_Logic.Triggers
{
    [Serializable]
    public class MayhemTimerTrigger: MayhemTrigger,
        ISerializable, IMayhemTriggerCommon, IMayhemConnectionItemCommon
        
    {
        private const string TAG = "[MayhemTimerTrigger} : ";
        public override event triggerActivateHandler onTriggerActivated;
        private TimerSetupWindow setupWindow;
        private int selected_hours;
        private int selected_mins;
        private int selected_secs;
        private Timer myTimer = new Timer();


        #region constructors

        /**<summary>
         * A Base-Compatible Constructor
         * Ignores the input string! 
         * </summary>
         * **/
        public MayhemTimerTrigger(string s)
            : this() { }

        public MayhemTimerTrigger()
            : base("Timer",
                    "Triggers after a certain amount of time",
                    "This trigger fires after a predefined amount of time has passed. The Trigger will automatically reset itself. Double click to configure the Trigger time period." )
        {

            selected_hours = 0;
            selected_mins = 0;
            selected_secs = 1;

            setupWindow = new TimerSetupWindow();
            setupWindow.OnSetButtonClicked += new TimerSetupWindow.SetButtonClickedHandler(setupWindow_OnSetButtonClicked);

            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);
            myTimer.Enabled = false;
        }

        #endregion

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            if (onTriggerActivated != null)
            {
                onTriggerActivated(this, new EventArgs());
            }
        }

        void setupWindow_OnSetButtonClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            selected_hours = setupWindow.selected_hours;
            selected_mins =  setupWindow.selected_minutes;
            selected_secs =  setupWindow.selected_seconds;

            double interval = (selected_hours * 3600 + selected_mins * 60 + selected_secs)*1000;

            Debug.WriteLine(TAG + "Selected interval" + interval);
            myTimer.Interval = interval ;
 

        }


        public override void OnDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.WriteLine("MayhemTimerTrigger::ONDOUBLECLICK");
            base.OnDoubleClick(sender, e);
            DimMainWindow(true);
            setupWindow.ShowDialog();
            DimMainWindow(false);
        }

        public   override void EnableTrigger()
        {

            base.EnableTrigger();

            myTimer.Enabled = true;

            myTimer.Start();

        }

        public  override void DisableTrigger()
        {

            base.DisableTrigger();
            myTimer.Stop();
            myTimer.Enabled = false;
        }

        #region MayhemTimerTrigger Serialization
        public MayhemTimerTrigger(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.selected_hours = info.GetInt32("Hours");
            this.selected_mins = info.GetInt32("Minutes");
            this.selected_secs = info.GetInt32("Seconds");

            this.setupWindow = new TimerSetupWindow();
            setupWindow.selected_hours = selected_hours;
            setupWindow.selected_minutes = selected_mins;
            setupWindow.selected_seconds = selected_secs;
            setupWindow.OnSetButtonClicked += new TimerSetupWindow.SetButtonClickedHandler(setupWindow_OnSetButtonClicked);

            myTimer = new Timer();
            myTimer.Interval =  (selected_hours * 3600 + selected_mins * 60 + selected_secs)*1000;

            myTimer.Elapsed += this.myTimer_Elapsed;

            if (this.triggerEnabled)
                myTimer.Start();

            
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Hours", this.selected_hours);
            info.AddValue("Minutes", this.selected_mins);
            info.AddValue("Seconds", this.selected_secs);
        }
        #endregion
    }
}
