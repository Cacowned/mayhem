using System;
using System.Linq;
using System.Windows;
using NuGet;
using System.IO;


namespace MayhemWpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!e.Args.Any())
            {
                return;
            }

            try
            {
                // Get the package file
                string packageFile = e.Args[0];
                // Modules path
                string installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules");

                var zipFile = new ZipPackage(packageFile);
                // Create a repository pointing to the local feed
                var repository = new DataServicePackageRepository(new Uri("http://localhost:58108/nuget/"));

                //repository.ProgressAvailable += new EventHandler<ProgressEventArgs>(repository_ProgressAvailable);

                // Create a package manager to install and resolve dependencies
                var packageManager = new PackageManager(repository, installPath);
                //packageManager.Logger = new Logger();

                // Install the package
                packageManager.InstallPackage(zipFile, ignoreDependencies: false);
            }
            catch (Exception ex)
            {
                // Fail
                Console.WriteLine(ex.Message);
            }
        }
        
        void repository_ProgressAvailable(object sender, ProgressEventArgs e)
        {
            Console.WriteLine("Progress!! {0}%", e.PercentComplete);
        }

        public class Logger : ILogger
        {

            public void Log(MessageLevel level, string message, params object[] args)
            {
                if (level == MessageLevel.Info)
                {
                    Console.WriteLine(message, args);
                }
            }
        }
    }
}
