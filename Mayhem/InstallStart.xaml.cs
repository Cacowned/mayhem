using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows;
using NuGet;

namespace Mayhem
{
	public partial class InstallStart : Window
	{
		private bool success;

		public InstallStart()
		{
			InitializeComponent();
			success = false;
		}

		private void Setup_Click(object sender, RoutedEventArgs e)
		{
			var packageManager = MayhemNuget.PackageManager;
			/*
			var repo = packageManager.SourceRepository;
			var progressProvider = repo as IProgressProvider;
			if (progressProvider != null)
			{
				progressProvider.ProgressAvailable += OnProgressAvailable;
			}
			 */

			packageManager.Logger = new Logger(this);

			packageManager.Logger.Log(MessageLevel.Info, "Starting setup.");

			// Install the package
			ThreadPool.QueueUserWorkItem(o =>
			{
				try
				{
					Dispatcher.Invoke((Action)delegate
					{
						buttonSetup.IsEnabled = false;
					});

					// Create a package manager to install and resolve dependencies
					packageManager.InstallPackage("DefaultModules");

					packageManager.Logger.Log(MessageLevel.Info, "Set up successfully. Click OK to start Mayhem.");

					success = true;
				}
				catch (WebException exception)
				{
					packageManager.Logger.Log(MessageLevel.Error, "Unable to connect to the Mayhem servers to set up.");
					packageManager.Logger.Log(MessageLevel.Error, "Please make sure you are connected to the internet and try again.");
					Debug.WriteLine(exception.Message);
					success = false;
				}
				catch (Exception ex)
				{
					// TODO: This should be a better message
					// MessageBox.Show(ex.Message);
					packageManager.Logger.Log(MessageLevel.Error, "An unknown error occurred.");
					Debug.WriteLine(ex.Message);
					success = false;
				}
				// toggle the buttons so we can do something now.
				Dispatcher.Invoke((Action)delegate
				{
					buttonSetup.Visibility = Visibility.Hidden;

					if (!success)
					{
						buttonClose.Content = "Close";
					}

					buttonClose.IsEnabled = true;
					buttonClose.Visibility = Visibility.Visible;


				});
			});
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = success;
		}

		/*
		public void OnProgressAvailable(object sender, ProgressEventArgs e)
		{
			Dispatcher.Invoke((Action)delegate
			{
				progress.Value = e.PercentComplete;
			});
		}
		 */

		public class Logger : ILogger
		{
			private readonly InstallStart parent;

			public Logger(InstallStart parent)
			{
				this.parent = parent;
			}

			public void Log(MessageLevel level, string message, params object[] args)
			{
				Debug.WriteLine(message, args);
				parent.Dispatcher.Invoke((Action)delegate
				{
					parent.listBox1.Items.Add(string.Format(message, args));
					parent.listBox1.SelectedItem = parent.listBox1.Items.GetItemAt(parent.listBox1.Items.Count - 1);
					parent.listBox1.ScrollIntoView(parent.listBox1.SelectedItem);
				});
			}
		}
	}
}
