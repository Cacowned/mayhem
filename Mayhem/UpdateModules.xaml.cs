using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows;
using NuGet;

namespace Mayhem
{
	public partial class UpdateModules : Window
	{
		private List<IPackage> packages;

		public UpdateModules(List<IPackage> packages)
		{
			this.packages = packages;

			InitializeComponent();
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Dispatcher.Invoke((Action)delegate
				{
					stackPanelDefaultButtons.Visibility = Visibility.Hidden;
					buttonClose.IsEnabled = false;
					buttonClose.Visibility = Visibility.Visible;
				});

				// Create a package manager to install and resolve dependencies
				var packageManager = MayhemNuget.PackageManager;
				packageManager.Logger = new Logger(this);

				packageManager.Logger.Log(MessageLevel.Info, "Starting update.");

				// Install the package
				ThreadPool.QueueUserWorkItem(o =>
				{
					try
					{
						foreach (IPackage package in packages)
						{
							Debug.WriteLine("Updating " + package.GetFullName());
							packageManager.UpdatePackage(package, true);
						}

						packageManager.Logger.Log(MessageLevel.Info, "Finished updating. You may now close this window.");

						Dispatcher.Invoke((Action)delegate
						{
							buttonClose.IsEnabled = true;
						});
					}
					catch (WebException exception)
					{
						packageManager.Logger.Log(MessageLevel.Error, "Could not connect to server.");
					}
				});
			}
			catch (Exception ex)
			{
				// TODO: This should be a better message
				MessageBox.Show(ex.Message);
				Debug.WriteLine(ex.Message);

				DialogResult = false;
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		public class Logger : ILogger
		{
			private readonly UpdateModules parent;

			public Logger(UpdateModules parent)
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
