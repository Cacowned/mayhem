using System;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using DefaultModules.Resources;

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

        private DateTime lastTriggeredDate;

        protected override void OnLoadDefaults()
        {
            folderToMonitor = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        protected override void OnAfterLoad()
        {
            // Ensure that an instance of fswatcher always gets created
            fileWatcher = new FileSystemWatcher();

            fileWatcher.Changed += OnChanged;
            fileWatcher.Created += OnChanged;
            fileWatcher.Deleted += OnChanged;
            fileWatcher.Renamed += OnRenamed;
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
            try
            {
                ConfigureFSMonitor();
                fileWatcher.EnableRaisingEvents = true;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.FolderChange_FolderDoesntExist);
                e.Cancel = true;
            }
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
            conf += "..." + substr;
            if (monitorName)
                conf += ", Renames";
            if (monitorSubDirs)
                conf += ", Subdirs";
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
            Logger.WriteLine("Args: " + a.ChangeType + " Fname " + a.Name);
            
            if (!TriggeredRecently())
            {
                Trigger();
            }
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
            {
                if (!TriggeredRecently())
                {
                    Trigger();
                }
            }
        }

        // Returns true if we have triggered recently
        private bool TriggeredRecently()
        {
            var shouldReturn = false;

            // Don't trigger if we have triggered within the last 10 milliseconds
            if (lastTriggeredDate != null)
            {
                var time = DateTime.Now;
                if ((time - lastTriggeredDate).TotalMilliseconds <= 50)
                {
                    shouldReturn = true;
                }
            }

            lastTriggeredDate = DateTime.Now;

            return shouldReturn;
        }
    }
}
