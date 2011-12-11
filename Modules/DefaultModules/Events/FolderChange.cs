using System;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Events
{
	[DataContract]
	[MayhemModule("Folder Change", "Triggers when a folder's contents are changed")]
	public class FolderChange : EventBase, IWpfConfigurable, IDisposable
	{
		[DataMember]
		private string folderToMonitor;

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

			fileWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;

			fileWatcher.Changed += OnChanged;
			fileWatcher.Created += OnChanged;
			fileWatcher.Deleted += OnChanged;
			fileWatcher.Renamed += OnChanged;
		}

		/// <summary>
		/// Set Filters the FSWatcher should monitor
		/// </summary>
		private void ConfigureFsMonitor()
		{
			fileWatcher.Path = folderToMonitor;
			fileWatcher.IncludeSubdirectories = monitorSubDirs;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (Directory.Exists(folderToMonitor))
			{
				ConfigureFsMonitor();
				fileWatcher.EnableRaisingEvents = true;
				
				return;
			}

			ErrorLog.AddError(ErrorType.Failure, Strings.FolderChange_FolderDoesntExist);
			e.Cancel = true;
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			fileWatcher.EnableRaisingEvents = false;
		}

		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new FolderChangeConfig(folderToMonitor, monitorSubDirs);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as FolderChangeConfig;

			folderToMonitor = config.FolderToMonitor;
			monitorSubDirs = config.SubDirectories;
		}

		public string GetConfigString()
		{
			string conf = string.Empty;
			int pathLength = folderToMonitor.Length;

			const int Cutoff = 20;

			string substr;
			if (pathLength >= Cutoff)
				substr = folderToMonitor.Substring(pathLength - Cutoff, Cutoff);
			else
				substr = folderToMonitor;
			conf += "..." + substr;

			if (monitorSubDirs)
				conf += ", Subfolders";
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

		// Returns true if we have triggered recently
		private bool TriggeredRecently()
		{
			var shouldReturn = false;

			var time = DateTime.Now;
			if ((time - lastTriggeredDate).TotalMilliseconds <= 50)
			{
				shouldReturn = true;
			}

			lastTriggeredDate = DateTime.Now;

			return shouldReturn;
		}

		public void Dispose()
		{
			fileWatcher.Dispose();
		}
	}
}
