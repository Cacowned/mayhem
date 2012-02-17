using System;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using NuGet;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

namespace Mayhem
{
	public class MayhemNuget
	{

		private static bool? canWrite = null;

		public static string WriteDir
		{
			get
			{
				string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\");
				string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem");

				if (canWrite == null)
				{
					try
					{
						string filePath = Path.Combine(localPath, "tempFile.tmp");
						// Attempt to create a temporary file. 
						using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
						{
							fs.WriteByte(0xff);
						}

						// Delete temporary file if it was successfully created. 
						if (File.Exists(filePath))
						{
							File.Delete(filePath);
							canWrite = true;
						}
						else
						{
							canWrite = false;
						}
					}
					catch (Exception)
					{
						canWrite = false;
					}
				}

				if (canWrite.Value)
				{
					return localPath;
				}
				else
				{
					if (!Directory.Exists(appData))
					{
						Directory.CreateDirectory(appData);
					}
					return appData;
				}
			}
		}

		public static IPackageRepository Repository
		{
			get
			{
				// Create a repository pointing to the feed
				IPackageRepository repository;
				if (Environment.GetCommandLineArgs().Contains("-localrepo"))
				{
					repository = new LocalPackageRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\MayhemPackages\Packages"));
				}
				else
				{
					repository = new DataServicePackageRepository(new Uri("http://makemayhem.com/www/nuget/"));
				}

				return repository;
			}
		}

		public static string InstallPath
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..");
			}
		}

		public static string AppDataFolder
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem");
			}
		}

		public static string SettingsPath
		{
			get
			{
				return Path.Combine(AppDataFolder, "settings.xml");
			}
		}

		private static PackageManager manager;

		public static PackageManager PackageManager
		{
			get
			{
				if (manager == null)
					manager = new PackageManager(MayhemNuget.Repository, MayhemNuget.InstallPath);

				return manager;
			}
		}
	}
}
