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
            this.rfid = InterfaceFactory.Rfid;
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
            rfid.Tag += RfidTag;
            rfid.TagLost += LostRfidTag;

            rfid.Attach += RfidAttach;
            rfid.Detach += RfidDetach;
        }

        #region Phidget Event Handlers

        //Tag event handler...we'll display the tag code in the field on the GUI
        private void RfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;
                rfid.LED = true;
                TagID = e.Tag;
            }));
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        private void LostRfidTag(object sender, TagEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                rfid.LED = false;
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
            rfid.Tag -= RfidTag;
            rfid.TagLost -= LostRfidTag;

            rfid.Attach -= RfidAttach;
            rfid.Detach -= RfidDetach;
        }
    }
}
