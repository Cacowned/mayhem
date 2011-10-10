using System.Windows;
using System.Windows.Threading;
using MayhemWpf.UserControls;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Wpf
{
    /// <summary>
    /// Interaction logic for Phidget1023RFIDConfig.xaml
    /// </summary>
    public partial class Phidget1023RFIDConfig : WpfConfiguration
    {
        protected RFID Rfid;

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
            this.Rfid = InterfaceFactory.Rfid;
            TagID = tagId;

            this.DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get
            {
                return "Phidget - Rfid";
            }
        }

        public override void OnLoad()
        {
            Rfid.Tag += RfidTag;
            Rfid.TagLost += LostRfidTag;

            Rfid.Attach += RfidAttach;
            Rfid.Detach += RfidDetach;
        }

        #region Phidget Event Handlers

        //Tag event handler...we'll display the tag code in the field on the GUI
        private void RfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;
                Rfid.LED = true;
                TagID = e.Tag;
            }));
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        private void LostRfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                Rfid.LED = false;
            }));
        }

        private void RfidAttach(object sender, Phidgets.Events.AttachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReader.Visibility = Visibility.Collapsed;
            }));
        }

        private void RfidDetach(object sender, DetachEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                this.NoReader.Visibility = Visibility.Visible;
            }));
        }

        #endregion

        public override void OnClosing()
        {
            Rfid.Tag -= RfidTag;
            Rfid.TagLost -= LostRfidTag;

            Rfid.Attach -= RfidAttach;
            Rfid.Detach -= RfidDetach;
        }
    }
}
