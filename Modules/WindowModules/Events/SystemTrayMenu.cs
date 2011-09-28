using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using WindowModules.Wpf;

namespace WindowModules.Events
{
    [DataContract]
    [MayhemModule("System Tray Menu", "Add a menu into the system tray")]
    public class SystemTrayMenu : EventBase, IWpfConfigurable
    {
        static object locker = new object();
        static NotifyIcon notifyIcon = null;

        private SynchronizationContext context;

        MenuItem menuItem;

        [DataMember]
        private string text;

        protected override void Initialize()
        {
            context = SynchronizationContext.Current;
        }

        void SetNotifyIconStuff(object state)
        {
            if (notifyIcon == null)
            {
                notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenu = new ContextMenu();
                Uri iconUri = new Uri("pack://application:,,,/Game.ico", UriKind.RelativeOrAbsolute);
                Stream iconStream = System.Windows.Application.GetResourceStream(iconUri).Stream;
                if (iconStream != null)
                {
                    notifyIcon.Icon = new System.Drawing.Icon(iconStream);
                }
                notifyIcon.Text = "Mayhem";
                notifyIcon.MouseUp += new MouseEventHandler(notifyIcon_MouseUp);
            }
            if(!notifyIcon.Visible)
                notifyIcon.Visible = true;

            menuItem = new MenuItem(text, OnClick);
            notifyIcon.ContextMenu.MenuItems.Add(menuItem);
        }

        void RemoveFromContextMenu(object state)
        {
            notifyIcon.ContextMenu.MenuItems.Remove(menuItem);
            menuItem = null;
            if (notifyIcon.ContextMenu.MenuItems.Count == 0)
            {
                notifyIcon.Visible = false;
            }
        }

        protected override bool OnEnable()
        {
            context.Post(SetNotifyIconStuff, null);
        
            return true;
        }

        protected override void OnDisable()
        {
            context.Post(RemoveFromContextMenu, null);
        }

        private void OnClick(object o, EventArgs e)
        {
            base.Trigger();
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
            SystemTrayMenuConfig config = configurationControl as SystemTrayMenuConfig;
            text = config.Text;
        }

        #endregion
    }
}
