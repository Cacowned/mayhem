using System;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
    [DataContract]
    [MayhemModule("Folder Change", "This event monitors changes on a given folder.")]
    public class FolderChange : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string folderToMonitor;

        [DataMember]
        private bool monitorWrite;

        [DataMember]
        private bool monitorName;

        private FileSystemWatcher fsWatcher;
        private NotifyFilters defaultFilter = NotifyFilters.DirectoryName;

        protected override void OnLoadDefaults()
        {
            folderToMonitor = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            monitorWrite = true;
        }

        protected override void OnAfterLoad()
        {
            // Ensure that an instance of fswatcher always gets created
            try
            {
                fsWatcher = new FileSystemWatcher(folderToMonitor);
            }
            catch
            {
                Logger.WriteLine("Exc: Folder doesn't seem to exist or be accesible");
                fsWatcher = new FileSystemWatcher();
                folderToMonitor = "Reconfigure!";
            }
            SetFlags();
            fsWatcher.Changed += OnChanged;
            fsWatcher.Created += OnChanged;
            fsWatcher.Deleted += OnChanged;
            fsWatcher.Renamed += OnRenamed;
            base.OnAfterLoad();
        }

        /// <summary>
        /// Set Filters the FSWatcher should monitor
        /// </summary>
        private void SetFlags()
        {
            Logger.WriteLine("");
            fsWatcher.NotifyFilter = defaultFilter;

            if (monitorWrite)
                fsWatcher.NotifyFilter |= NotifyFilters.LastWrite;
            if (monitorName)
                fsWatcher.NotifyFilter |= NotifyFilters.DirectoryName | NotifyFilters.FileName;
            fsWatcher.IncludeSubdirectories = true;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            Logger.WriteLine("");
            fsWatcher.EnableRaisingEvents = true;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            Logger.WriteLine("");
            fsWatcher.EnableRaisingEvents = false;
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {

                return new FolderChangeConfig(folderToMonitor,
                                                monitorWrite,
                                                monitorName);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Logger.WriteLine("");
            FolderChangeConfig config = configurationControl as FolderChangeConfig;
            folderToMonitor = config.FolderToMonitor;
            monitorWrite = config.MonitorWrite;
            monitorName = config.MonitorName;
            fsWatcher.Path = folderToMonitor;
            SetFlags();
        }

        public string GetConfigString()
        {
            string conf = string.Empty;

            int pathLength = folderToMonitor.Length;

            const int cutoff = 10;
            string substr;
            if (pathLength >= cutoff)
                substr = folderToMonitor.Substring(pathLength - cutoff, cutoff);
            else
                substr = folderToMonitor;
            conf += "[...]" + substr;
            if (monitorWrite)
                conf += " WRITE ";
            if (monitorName)
                conf += " NAME ";
            return conf;
        }

        /// <summary>
        /// Handler for file changes
        /// </summary>
        /// <param name="o"></param>
        /// <param name="a"></param>
        private void OnChanged(object o, FileSystemEventArgs a)
        {
            Logger.WriteLine("Args: " + a.ChangeType + " Fname " + a.Name);
            Trigger();
        }

        /// <summary>
        /// Handler for file renames
        /// </summary>
        /// <param name="o"></param>
        /// <param name="a"></param>
        private void OnRenamed(object o, RenamedEventArgs a)
        {
            Logger.WriteLine("Args: " + a.ChangeType + " " + a.Name);
            Trigger();
        }

    }
}
