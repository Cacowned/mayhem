using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DisplayWindowModuleWpf
{
    /// <summary>
    /// Interaction logic for MsgWindow.xaml
    /// </summary>
    public partial class MessagWindow : Window
    {
        private bool isFadeOutStoryboardCompleted = false;
        private bool isFadeOutStoryboardStarted = false;
        private Storyboard fadeOutStoryboard = new Storyboard();
        private System.Windows.Forms.Timer timer;
        private int seconds;
        
        public MessagWindow(String message, int sec)
        {
            InitializeComponent();

            this.textBlock.Text = message;
            this.seconds = sec;

            SetWindowToBottomRightOfScreen();

            fadeOutStoryboard = (Storyboard)this.TryFindResource("FadeOutStoryboard");
            fadeOutStoryboard.Completed += new EventHandler(FadeOutStoryboard_Completed);
        }

        private void FadeOutStoryboard_Completed(object sender, EventArgs e)
        {
            isFadeOutStoryboardCompleted = true;
            this.Close();
        } 

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetWindowToBottomRightOfScreen()
        {
            Left = SystemParameters.WorkArea.Width - this.Width;
            Top = SystemParameters.WorkArea.Height - this.Height;
        }

        private void MyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isFadeOutStoryboardCompleted && !isFadeOutStoryboardStarted)
            {
                //if we start the storyboard we don't want to start it again if the closing event is raised again
                isFadeOutStoryboardStarted = true;
                fadeOutStoryboard.Begin();
                e.Cancel = true;
            }
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;

            timer = new System.Windows.Forms.Timer();
            timer.Interval = seconds * 1000; //transforming seconds in miliseconds
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        //is triggered when we should close the window, so we stop the timer
        public void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Dispose();

            this.Close();
        }
    }
}
