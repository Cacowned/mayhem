using System.Windows;
using System.Windows.Threading;
using Phidgets;
using Phidgets.Events;
using MayhemWpf.UserControls;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1023RFIDConfig.xaml
    /// </summary>
    public partial class Phidget1023RFIDConfig : IWpfConfiguration
    {
        protected RFID rfid;

        public string TagID
        {
            get { return (string)GetValue(TagIDProperty); }
            set { SetValue(TagIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagIDProperty =
            DependencyProperty.Register("TagID", typeof(string), typeof(Phidget1023RFIDConfig), new UIPropertyMetadata(string.Empty));


        public Phidget1023RFIDConfig(string tagId)
        {
            this.rfid = InterfaceFactory.GetRFID();
            TagID = tagId;

            InitializeComponent();
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void RfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                {
                    rfid.LED = true;
                    TagID = e.Tag;
                }));
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void LostRfidTag(object sender, TagEventArgs e)
        {
            rfid.LED = false;
        }

        public override void OnLoad()
        {
            rfid.Tag += RfidTag;
            rfid.TagLost += LostRfidTag;
        }

        public override void OnClosing()
        {
            rfid.Tag -= RfidTag;
            rfid.TagLost -= LostRfidTag;
        }

        public override bool OnSave()
        {
            if (TagID == string.Empty)
            {
                MessageBox.Show("You must configure a tag.");
                return false;
            }
            return true;
        }

        public override string Title
        {
            get
            {
                return "Phidget - RFID";
            }
        }
    }
}
