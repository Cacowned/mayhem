using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace MayhemApp.Dialogs
{
    /**<summary>
     * I chose to subclass TwitterActionSetupWindow because these two dialogs, for the most part
     * share the same widget set.
     * </summary>
     */
    class UDPTriggerSetupWindow : TwitterActionSetupWindow
    {

        private string titleText_ = "Configure: UDP Trigger";
        public override string titleText { get { return titleText_; } }
        private string subTitleText_ = "This action fires when the specified message is received on the socket";
        public override string subTitleText {  get { return subTitleText_; } }
        private string helpText_ = "Please specify the message (limit 125 characters) to listen for and the port nr to listen on:";
        public override string helpText {  get { return helpText_; } }
        public  override string imageSource {  get { return imageSource_; } }
        private string imageSource_ = "/MayhemApp;component/Images/networkIcon.png";

        public TextBox portBox = new TextBox();

        // TODO clean the following up, it is a bit messy
        public  delegate void SetButtonClickedHandler_(object sender, TwitterActionSetTextEventArgs e);
        public  new event SetButtonClickedHandler_ OnSetButtonClicked;

        public class UDPTriggerEventArgs : TwitterActionSetTextEventArgs
        {
            public string message = null;
            public int port = -1;

            public UDPTriggerEventArgs(string msg, int prt)
                : base()
            {
                message = msg;
                port = prt;
            }

        }

        public UDPTriggerSetupWindow()
            : base()
        {
            // add a textbox for port selection programmatically 
            // don't know if this is better than just creating a new .xaml file

            Label l = new Label();
            l.Content = "Port Nr (>1023):";
            l.Foreground =  new SolidColorBrush(Colors.WhiteSmoke);
            additional_items.Children.Add(l);
            l.Margin = new System.Windows.Thickness(0,0,10,0);
            l.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tb = portBox;
            tb.Text = "1024";
            tb.MaxHeight = 20;
            tb.MinWidth = 100;
            tb.Margin = new System.Windows.Thickness(10, 0, 10, 0);
            additional_items.Children.Add(tb);

            icon.Width = 150;

            EntryBox.Text = "bazinga!";

            base.OnSetButtonClicked += new SetButtonClickedHandler(UDPTriggerSetupWindow_OnSetButtonClicked);

            
        }

        void UDPTriggerSetupWindow_OnSetButtonClicked(object sender, TwitterActionSetupWindow.TwitterActionSetTextEventArgs e)
        {
            //throw new NotImplementedException();
            int port_nr = 0;
            if (int.TryParse(portBox.Text, out port_nr))
            {
                if (OnSetButtonClicked != null)
                {
                    OnSetButtonClicked(this, new UDPTriggerEventArgs(tweet_text, port_nr));
                }
            }
        }

       

    }
}
