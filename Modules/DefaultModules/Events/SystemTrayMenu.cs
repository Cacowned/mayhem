using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("System Tray Menu", "Adds a menu into the system tray")]
    public class SystemTrayMenu : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string text;

        private static NotifyIcon notifyIcon;

        private SynchronizationContext context;

        private MenuItem menuItem;

        protected override void OnAfterLoad()
        {
            context = SynchronizationContext.Current;
        }

        private void SetNotifyIconStuff(object state)
        {
            if (notifyIcon == null)
            {
                notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenu = new ContextMenu();
                
				Icon icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
				if (icon != null)
                {
					notifyIcon.Icon = icon;
                }

                notifyIcon.Text = "Mayhem";
                notifyIcon.MouseUp += notifyIcon_MouseUp;
            }

            if (!notifyIcon.Visible)
                notifyIcon.Visible = true;

            menuItem = new MenuItem(text, OnClick);
            notifyIcon.ContextMenu.MenuItems.Add(menuItem);
        }

        private void RemoveFromContextMenu(object state)
        {
            notifyIcon.ContextMenu.MenuItems.Remove(menuItem);
            menuItem = null;
            if (notifyIcon.ContextMenu.MenuItems.Count == 0)
            {
                notifyIcon.Visible = false;
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            context.Post(SetNotifyIconStuff, null);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            context.Post(RemoveFromContextMenu, null);
        }

        private void OnClick(object o, EventArgs e)
        {
            Trigger();
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        public string GetConfigString()
        {
            return text;
        }

        #region Configuration Views

        public WpfConfiguration ConfigurationControl
        {
            get { return new SystemTrayMenuConfig(text); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SystemTrayMenuConfig;

            text = config.Text;
        }

        #endregion
    }
}
