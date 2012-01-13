using System;
using System.Collections.Generic;
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
		}

		private void Setup_Click(object sender, RoutedEventArgs e)
		{
			var packageManager = MayhemNuget.PackageManager;
			packageManager.Logger = new Logger(this);

			packageManager.Logger.Log(MessageLevel.Info, "Starting setup.");

			try
			{
				Dispatcher.Invoke((Action)delegate
				{
					buttonSetup.IsEnabled = false;
				});

				// Create a package manager to install and resolve dependencies

				// Install the package
				ThreadPool.QueueUserWorkItem(o =>
				{
					try
					{
						packageManager.InstallPackage("DefaultModules");

						packageManager.Logger.Log(MessageLevel.Info, "Set up successfully. Click OK to start Mayhem.");

						success = true;
					}
					catch (WebException exception)
					{
						packageManager.Logger.Log(MessageLevel.Error, "Unable to connect to the Mayhem servers to set up.");
						packageManager.Logger.Log(MessageLevel.Error, "Please connect to the internet and try again.");
						success = false;
					}
				});
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

				buttonClose.IsEnabled = true;
				buttonClose.Visibility = Visibility.Visible;

				if (!success)
				{
					buttonClose.Content = "Close";
				}
			});
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = success;
		}

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
