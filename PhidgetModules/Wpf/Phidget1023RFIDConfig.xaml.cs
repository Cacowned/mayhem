using System.Windows;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1023RFIDConfig.xaml
    /// </summary>
    public partial class Phidget1023RFIDConfig : Window
    {
        protected RFID rfid;

        protected TagEventHandler gotTag;
        protected TagEventHandler lostTag;

        public string TagID
        {
            get { return (string)GetValue(TagIDProperty); }
            set { SetValue(TagIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagIDProperty =
            DependencyProperty.Register("TagID", typeof(string), typeof(Phidget1023RFIDConfig), new UIPropertyMetadata(string.Empty));


        public Phidget1023RFIDConfig(RFID phidget, string tagId)
        {
            this.rfid = phidget;
            TagID = tagId;

            InitializeComponent();
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);

            gotTag = new TagEventHandler(rfidTag);
            lostTag = new TagEventHandler(lostRfidTag);
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void rfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                {
                    rfid.LED = true;
                    TagID = e.Tag;
                }));
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void lostRfidTag(object sender, TagEventArgs e)
        {
            rfid.LED = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rfid.Tag += gotTag;
            rfid.TagLost += lostTag;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rfid.Tag -= gotTag;
            rfid.TagLost -= lostTag;
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (TagID == string.Empty)
            {
                MessageBox.Show("You must configure a tag.");
                return;
            }
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
