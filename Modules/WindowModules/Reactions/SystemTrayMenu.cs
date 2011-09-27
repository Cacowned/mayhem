using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using System.IO;
using WindowModules.Wpf;
using System.Reflection;
using System.Windows.Threading;
using System.Threading;

namespace WindowModules.Reactions
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

        public SystemTrayMenu()
        {
            
        }

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

        public override bool Enable()
        {
            context.Post(SetNotifyIconStuff, null);
        
            return true;
        }

        public override void Disable()
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
