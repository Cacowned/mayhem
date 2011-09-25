using System.Windows;
using MayhemWpf.UserControls;
using Microsoft.Xna.Framework.Input;
using XboxModules.Events;
using System;

namespace XboxModules.Wpf
{
    /// <summary>
    /// Interaction logic for XboxButtonConfig.xaml
    /// </summary>
    public partial class XboxButtonConfig : IWpfConfiguration
    {
        public Buttons ButtonsToSave;

        // TODO: Make this configurable
        private Buttons down_buttons;

        // Contains the string the interface binds to
        public string ButtonsDownText
        {
            get { return (string)GetValue(ButtonsDownTextProperty); }
            set { SetValue(ButtonsDownTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonsDownTextProperty =
            DependencyProperty.Register("ButtonsDownText", typeof(string), typeof(XboxButtonConfig), new UIPropertyMetadata(string.Empty));

        private bool shouldCheckValidity = false;

        public XboxButtonConfig(Buttons buttons)
        {
            ButtonsToSave = buttons;

            this.DataContext = this;
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Xbox Button"; }
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

            shouldCheckValidity = true;
        }

        private void Instance_OnButtonDown(Buttons button)
        {
            // If this button isn't down already
            if (!down_buttons.HasFlag(button))
            {
                // and we have no buttons down
                if (down_buttons == 0)
                {
                    // then this is the first hit of a new combination. 
                    // clear out the buttons we were going to save
                    ButtonsToSave = 0;
                }
                // Add the button
                down_buttons |= button;
                ButtonsToSave |= button;

                // And update the string
                UpdateButtonsDown();
            }
            //down_buttons |= button;
        }

        private void Instance_OnButtonUp(Buttons button)
        {
            // remove button from buttons_down
            down_buttons &= ~button;
        }

        private void UpdateButtonsDown()
        {
            Dispatcher.Invoke((Action)delegate
            {
                ButtonsDownText = ButtonsToSave.ButtonString();
                CanSave = ButtonsToSave != 0;

                if (shouldCheckValidity)
                {
                    textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
                }

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
                    //CanSave = false;

                    textInvalid.Visibility = Visibility.Visible;
                }
                if (status == ControllerStatus.Attached)
                {
                    //CanSave = true;
                    textInvalid.Visibility = Visibility.Collapsed;
                }
            });

            //textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
