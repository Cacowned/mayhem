﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MayhemCore
{
	/// <summary>
	/// Helper methods for Action and Reaction lists
	/// </summary>
	/// <typeparam name="T">ActionBase or ReactionBase</typeparam>
	/// <typeparam name="V">The interface type that modules must implement</typeparam>
	[Serializable]
	public abstract class ModuleList<T, V> : List<T> where T : ModuleBase
	{
		public ModuleList() {
            RescanModules();
		}

        public void RescanModules() {
            // Load up all the types of things that we want in the application root
            FindTypes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules"));
            //FindTypes(Path.Combine(Application.StartupPath, "modules"));
            //FindTypes(Application.StartupPath);
        }

		/// <summary>
		/// Find all the types that exist in DLLs specified by path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		protected void FindTypes(string path) {
			if (String.IsNullOrEmpty(path)) {
				throw new ArgumentNullException("The given path is null or empty");
			}

            // Clear this of all the modules
            this.Clear();

			string[] pluginFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

			// Go through each dll file
			for (int i = 0; i < pluginFiles.Length; i++) {
				// load it
				string path2 = Path.Combine(path, pluginFiles[i]);
				Assembly ass = Assembly.LoadFrom(path2);

				if (ass != null) {
					// Go through all the public classes in the assembly
					foreach (var type in ass.GetExportedTypes()) {
						// If it's parent class is the type we want
						// and it implements the correct moduleType
						if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract) {
							// Create an instance
							T reaction = (T)Activator.CreateInstance(type);
							bool hasConfig = reaction.HasConfig;
							if (!hasConfig || (hasConfig && type.GetInterfaces().Contains(typeof(V)))) {
								// Add it to our final list
								this.Add(reaction);
							}
						}
					}
				}
			}
			this.Sort();
		}
	}
}
