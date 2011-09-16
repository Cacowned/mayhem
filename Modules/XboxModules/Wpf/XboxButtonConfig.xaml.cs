using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XboxModules.Events;
using MayhemWpf.UserControls;

namespace XboxModules.Wpf
{
    /// <summary>
    /// Interaction logic for XboxButtonConfig.xaml
    /// </summary>
    public partial class XboxButtonConfig : IWpfConfiguration
    {
        // TODO: Make this configurable
        protected PlayerIndex player = PlayerIndex.One;

        ButtonEvents.ButtonDownHandler down_handler;
        ButtonEvents.ButtonUpHandler up_handler;

        private Buttons down_buttons;
        public Buttons SaveButtons;


        public string ButtonsDown
        {
            get { return (string)GetValue(ButtonsDownProperty); }
            set { SetValue(ButtonsDownProperty, value); }
        }

        public static readonly DependencyProperty ButtonsDownProperty =
            DependencyProperty.Register("ButtonsDown", typeof(string), typeof(XboxButtonConfig), new UIPropertyMetadata(string.Empty));

        public XboxButtonConfig(Buttons buttons)
        {
            this.DataContext = this;
            InitializeComponent();

            SaveButtons = buttons;

            down_handler = new ButtonEvents.ButtonDownHandler(Instance_OnButtonDown);
            up_handler = new ButtonEvents.ButtonUpHandler(Instance_OnButtonUp);
        }

        public override void OnLoad()
        {
            ButtonEvents.Instance.OnButtonDown += down_handler;
            ButtonEvents.Instance.OnButtonUp += up_handler;

            ButtonEvents.Instance.AddRef();

            UpdateButtonsDown(SaveButtons);
        }

        void Instance_OnButtonDown(Buttons button)
        {
            if (!down_buttons.HasFlag(button))
            {
                if (down_buttons == 0)
                {
                    SaveButtons = 0;
                }
                down_buttons |= button;
                SaveButtons |= button;

                UpdateButtonsDown(SaveButtons);
            }
            down_buttons |= button;
        }

        void Instance_OnButtonUp(Buttons button)
        {
            // remove button from buttons_down
            down_buttons &= ~button;
        }

        void UpdateButtonsDown(Buttons buttons)
        {
            Dispatcher.Invoke((Action)delegate
            {
                ButtonsDown = buttons.ButtonString();
                CanSave = buttons != 0;
            });
        }

        public override bool OnSave()
        {
            ///TODO: Should this be here?
            //bwButtons.CancelAsync();
            if (SaveButtons == 0)
            {
                MessageBox.Show("You must push at least one button");
                return false;
            }
            return true;
        }

        public override void OnClosing()
        {
            ButtonEvents.Instance.RemoveRef();
            base.OnClosing();
        }

        public override string Title
        {
            get { return "Xbox Button"; }
        }
    }
}
