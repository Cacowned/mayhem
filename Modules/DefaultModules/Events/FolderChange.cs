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
        private bool monitorName;

        [DataMember]
        private bool monitorSubDirs;

        private FileSystemWatcher fileWatcher;

        private FileSystemEventArgs lastArgsCreated;
        private DateTime lastCreatedDate;

        protected override void OnLoadDefaults()
        {
            folderToMonitor = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        protected override void OnAfterLoad()
        {
            // Ensure that an instance of fswatcher always gets created
            try
            {
                fileWatcher = new FileSystemWatcher(folderToMonitor);
            }
            catch
            {
                Logger.WriteLine("Exc: Folder doesn't seem to exist or be accesible");
                fileWatcher = new FileSystemWatcher();
            }

            fileWatcher.Changed += OnChanged;
            fileWatcher.Created += OnChanged;
            fileWatcher.Deleted += OnChanged;
            fileWatcher.Renamed += OnRenamed;
            ConfigureFSMonitor();
        }

        /// <summary>
        /// Set Filters the FSWatcher should monitor
        /// </summary>
        private void ConfigureFSMonitor()
        {
            fileWatcher.Path = folderToMonitor;
            fileWatcher.IncludeSubdirectories = monitorSubDirs;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new FolderChangeConfig(folderToMonitor, monitorName, monitorSubDirs);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            FolderChangeConfig config = configurationControl as FolderChangeConfig;
            folderToMonitor = config.FolderToMonitor;
            monitorName = config.MonitorName;
            monitorSubDirs = config.SubDirectories;
            ConfigureFSMonitor();
        }

        public string GetConfigString()
        {
            string conf = string.Empty;
            int pathLength = folderToMonitor.Length;
            int cutoff = 10;
            string substr;
            if (pathLength >= cutoff)
                substr = folderToMonitor.Substring(pathLength - cutoff, cutoff);
            else
                substr = folderToMonitor;
            conf += "[...]" + substr;
            if (monitorName)
                conf += " NAME ";
            if (monitorSubDirs)
                conf += " SUBDIRS ";
            return conf;
        }

        /// <summary>
        /// Handler for file changes
        /// </summary>
        /// <param name="o"></param>
        /// <param name="a"></param>
        private void OnChanged(object o, FileSystemEventArgs a)
        {
            Logger.WriteLine(a.FullPath);

            // filter out repeadted triggers when a file is created
            if (a.ChangeType == WatcherChangeTypes.Created)
            {
                lastArgsCreated = a;
                lastCreatedDate = DateTime.Now;
            }
            else if (a.ChangeType == WatcherChangeTypes.Changed)
            {
                DateTime now = DateTime.Now;
                TimeSpan ts = now - lastCreatedDate;

                // don't fire any event --> the change event is caused by the previous "created" event
                if (a.FullPath == lastArgsCreated.FullPath || ts.TotalMilliseconds <= 10)
                    return;
            }

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
            if (monitorName)
                Trigger();
        }
    }
}
