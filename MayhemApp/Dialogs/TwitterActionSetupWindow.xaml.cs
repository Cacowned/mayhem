using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MayhemApp.Dialogs
{
    /// <summary>
    /// Interaction logic for TwitterActionSetupWindow.xaml
    /// </summary>
    public  partial class TwitterActionSetupWindow : Window
    {

        public string tweet_text = "";



        private string titleText_ = "Configure: Twitter Update";
        public virtual  string titleText { get { return titleText_; }}

        private string subTitleText_ = "This action posts a twitter update when the associated trigger fires.";
        public virtual  string subTitleText {get { return subTitleText_; }}

        private string helpText_ = "Please specify the update to be posted on twitter when this action is triggered. For technical reasons, mayhem Twitter updates are limited to 125 characters.";
        public virtual string helpText { get { return helpText_; } }

        private string imageSource_ = "/MayhemApp;component/Images/twitter-official.png";
        public virtual string imageSource { get { return imageSource_; } }



        public class TwitterActionSetTextEventArgs : RoutedEventArgs
        {


            public string tweet_text = null;


            public TwitterActionSetTextEventArgs(string tweet)
                : base()
            {
                tweet_text = tweet;
            }

            public TwitterActionSetTextEventArgs() : base() { }

        }


        public delegate void SetButtonClickedHandler(object sender, TwitterActionSetTextEventArgs e);
        public virtual event SetButtonClickedHandler OnSetButtonClicked;

        public TwitterActionSetupWindow()
        {
            InitializeComponent();

            this.DataContext = this;

           
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            this.Hide();
        }

        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // notify listener of event args
            if (OnSetButtonClicked != null)
            {
                OnSetButtonClicked(this, new TwitterActionSetTextEventArgs(this.tweet_text));
            }
        }


        public void OnTextEntered()
        {
            int length = EntryBox.Text.Length;
            int charsRemaining = 125 - length;

            if (charsRemaining >= 0)
            {
                charsRemainingLabel.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
                EntryBox.Foreground = new SolidColorBrush(Colors.Black);
                SaveButton.IsEnabled = true;
                this.tweet_text = EntryBox.Text;
            }
            else if (charsRemaining < 0 && charsRemaining >= -10)
            {
                charsRemainingLabel.Foreground = new SolidColorBrush(Colors.Tomato);
                EntryBox.Foreground = new SolidColorBrush(Colors.Tomato);
                SaveButton.IsEnabled = false;
            }



            charsRemainingLabel.Content = charsRemaining.ToString();
        }

        private void EntryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextEntered();

        }
    }
}
