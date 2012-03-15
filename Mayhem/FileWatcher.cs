using System;
using System.IO;


namespace Mayhem
{

	public class FileWatcher
	{
		private FileSystemWatcher watcher;

		private DateTime lastUpdated;

		private Action hook;

		public FileWatcher(string location, Action hook)
		{
			this.hook = hook;

			watcher = new FileSystemWatcher(location);
			watcher.IncludeSubdirectories = true;
			watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
			watcher.Changed += Hook;
			watcher.Created += Hook;
			watcher.Deleted += Hook;
			watcher.Renamed += Hook;
			watcher.EnableRaisingEvents = true;

			lastUpdated = DateTime.Now;
		}

		private void Hook(object sender, FileSystemEventArgs e)
		{
			var nowTime = DateTime.Now;

			if ((nowTime - lastUpdated).TotalMilliseconds > 30)
			{
				lastUpdated = nowTime;
				hook();
			}
		}

		public void Enable()
		{
			watcher.EnableRaisingEvents = true;
		}

		public void Disable()
		{
			watcher.EnableRaisingEvents = false;
		}
	}
}
