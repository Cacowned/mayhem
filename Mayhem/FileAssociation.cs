using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace Mayhem
{
	public class FileAssociation
	{
		// Associate file extension with progID, description, icon and application
		public static void Associate(string extension,
									 string progId, string description, string icon, string application)
		{
			RegistryKey key = Registry.ClassesRoot.CreateSubKey(extension);
			key.SetValue("", progId);

			key = Registry.ClassesRoot.CreateSubKey(progId);
			key.SetValue("", description);
			RegistryKey iconKey = key.CreateSubKey("DefaultIcon");
			iconKey.SetValue("", icon);

			key = key.CreateSubKey("shell");
			key = key.CreateSubKey("open");
			key = key.CreateSubKey("command");
			key.SetValue("", application + " %1");
		}

		// Return true if extension already associated in registry
		public static bool IsAssociated(string extension)
		{
			return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
		}

		[DllImport("Kernel32.dll")]
		private static extern uint GetShortPathName(string lpszLongPath,
													[Out] StringBuilder lpszShortPath, uint cchBuffer);

		// Return short path format of a file name
		private static string ToShortPathName(string longName)
		{
			var s = new StringBuilder(1000);
			var iSize = (uint)s.Capacity;
			uint iRet = GetShortPathName(longName, s, iSize);
			return s.ToString();
		}
	}
}