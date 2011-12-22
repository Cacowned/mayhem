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

		public string TagId
        {
			get { return (string)GetValue(TagIdProperty); }
			set { SetValue(TagIdProperty, value); }
        }

		// Using a DependencyProperty as the backing store for TagId.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TagIdProperty =
			DependencyProperty.Register("TagId", typeof(string), typeof(Phidget1023RFIDConfig), new UIPropertyMetadata(string.Empty));

		

        public Phidget1023RFIDConfig(string tagId)
        {
            DataContext = this;

            Rfid = InterfaceFactory.Rfid;
			TagId = tagId;

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

            SetAttached();
        }

        #region Phidget Event Handlers

        // Tag event handler...we'll display the tag code in the field on the GUI
        private void RfidTag(object sender, TagEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                CanSave = true;
                Rfid.LED = true;
				TagId = e.Tag;
            }));
        }

        // Tag event handler...we'll display the tag code in the field on the GUI
        private void LostRfidTag(object sender, TagEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                Rfid.LED = false;
            }));
        }

        private void RfidAttach(object sender, AttachEventArgs e)
        {
            SetAttached();
        }

        private void RfidDetach(object sender, DetachEventArgs e)
        {
            SetAttached();
        }

        private void SetAttached()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
                {
                    if (Rfid.Attached)
                    {
                        NoReader.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        NoReader.Visibility = Visibility.Visible;
                    }
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
