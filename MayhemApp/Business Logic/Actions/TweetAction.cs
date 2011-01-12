using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemApp.Business_Logic.Twitter;
using MayhemApp.Dialogs;
using System.Runtime.Serialization;
using System.Windows;
using System.Diagnostics;


namespace MayhemApp.Business_Logic.Actions
{
    /**<summary>
     *  This class represents a Twitter Tweet Action. 
     * </summary>
     * */
    [Serializable]
    public class TweetAction : MayhemActionBase, ISerializable, IMayhemConnectionItemCommon
    {

        public string tweet_text = "#Mayhem just got triggered";

        public int times_triggered = 0;

        // TODO -- make this more generic and associate a UI dialog with the Action in the base class
        private new TwitterActionSetupWindow setup_window = new TwitterActionSetupWindow();

        // TODO: some setup Window


        /**<summary>
         * 
         * Makes a base-compatible constructor.
         * Ignores the input string!
         * </summary>
         **/
        public TweetAction(string s) : this() { }

        public TweetAction()
            : base(     "Twitter Update",
                        "Updates Twitter status with a text",
                        "This action updates the Twitter status of the Twitter account associated with Mayhem. Double click to configure the Twitter update message")
        {
            setup_window.OnSetButtonClicked += new TwitterActionSetupWindow.SetButtonClickedHandler(setup_window_OnSetButtonClicked);
        }

        void setup_window_OnSetButtonClicked(object sender, TwitterActionSetupWindow.TwitterActionSetTextEventArgs e)
        {
            tweet_text =  setup_window.tweet_text;
        }

        #region MayhemAction overrides
        /** <summary>
         *  Event following UI widget double click
         *  Implements override in MayhemConnectionCommon Interface
         * </summary>
         */ 
        public override void OnDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO -- this could be implemented in the base class
           // setup_window.Show();
           
           DimMainWindow(true);
           setup_window.ShowDialog();
           DimMainWindow(false);
        }
      
        /** <summary>
         *  This is the actual action that gets executed
         * </summary>
         * 
         */ 
        public override void PerformAction(MayhemTriggerBase sender)
        {
            MayhemTwitter t = MayhemTwitter.Instance;
            t.SendTweet("#Mayhem [" + DateTime.Now.Millisecond.ToString().Substring(0,3)+"] " + tweet_text);
        }


        #endregion

        #region MayhemTimerTrigger Serialization

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Tweet_Text", this.tweet_text);
        }

         public TweetAction(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.tweet_text = info.GetString("Tweet_Text");

            // configure the window
            this.setup_window = new TwitterActionSetupWindow();
            this.setup_window.tweet_text = tweet_text;
            this.setup_window.EntryBox.Text = tweet_text;
            this.setup_window.OnTextEntered();
        }

        #endregion
    }
}
