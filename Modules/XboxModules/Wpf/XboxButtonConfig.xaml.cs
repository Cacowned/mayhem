using System;
using System.Windows;
using MayhemWpf.UserControls;
using Microsoft.Xna.Framework.Input;
using XboxModules.Events;

namespace XboxModules.Wpf
{
    /// <summary>
    /// Interaction logic for XboxButtonConfig.xaml
    /// </summary>
    public partial class XboxButtonConfig : WpfConfiguration
    {
        public Buttons ButtonsToSave { get; private set; }

        // TODO: Make this configurable
        private Buttons downButtons;

        // Contains the string the interface binds to
        public string ButtonsDownText
        {
            get { return (string)GetValue(ButtonsDownTextProperty); }
            set { SetValue(ButtonsDownTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonsDownTextProperty =
            DependencyProperty.Register("ButtonsDownText", typeof(string), typeof(XboxButtonConfig), new UIPropertyMetadata(string.Empty));

        public XboxButtonConfig(Buttons buttons)
        {
            ButtonsToSave = buttons;

            DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Xbox Controller: Button"; }
        }

        public override void OnLoad()
        {
            XboxButton.IsConfigOpen = true;

            ButtonEvents buttons = ButtonEvents.Instance;
            buttons.AddRef();
            buttons.OnButtonDown += Instance_OnButtonDown;
            buttons.OnButtonUp += Instance_OnButtonUp;
            buttons.OnStatusChanged += Instance_OnStatusChanged;

            UpdateButtonsDown();

			Instance_OnStatusChanged(buttons.Status);
        }

        private void Instance_OnButtonDown(Buttons button)
        {
            // If this button isn't down already
            if (!downButtons.HasFlag(button))
            {
                // and we have no buttons down
                if (downButtons == 0)
                {
                    // then this is the first hit of a new combination. 
                    // clear out the buttons we were going to save
                    ButtonsToSave = 0;
                }

                // Add the button
                downButtons |= button;
                ButtonsToSave |= button;

                // And update the string
                UpdateButtonsDown();
            }
        }

        private void Instance_OnButtonUp(Buttons button)
        {
            // remove button from buttons_down
            downButtons &= ~button;
        }

        private void UpdateButtonsDown()
        {
            Dispatcher.Invoke((Action)delegate
            {
                ButtonsDownText = ButtonsToSave.ButtonString();
                CanSave = ButtonsToSave != 0;
            });
        }

        public override void OnClosing()
        {
            ButtonEvents buttons = ButtonEvents.Instance;
            buttons.RemoveRef();
            buttons.OnButtonDown -= Instance_OnButtonDown;
            buttons.OnButtonUp -= Instance_OnButtonUp;

            XboxButton.IsConfigOpen = false;
        }

        private void Instance_OnStatusChanged(ControllerStatus status)
        {
            Dispatcher.Invoke((Action)delegate
            {
                if (status == ControllerStatus.Detached)
                {
                    textInvalid.Visibility = Visibility.Visible;
                }

                if (status == ControllerStatus.Attached)
                {
                    textInvalid.Visibility = Visibility.Collapsed;
                }
            });
        }
    }
}
