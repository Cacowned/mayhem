﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace MayhemCore
{
	/// <summary>
	/// Helper methods for Event and Reaction lists
	/// </summary>
	/// <typeparam name="T">EventBase or ReactionBase</typeparam>
	/// <typeparam name="V">The interface type that modules must implement</typeparam>
	public abstract class ModuleList<T, V> : List<T> where T : ModuleBase
	{
        public List<Type> types;
		public ModuleList() 
        {
            types = new List<Type>();
		}

        public void ScanModules(string path) 
        {
            // Load up all the types of things that we want in the application root
            FindTypes(path);
            //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules"));
            //FindTypes(Path.Combine(Application.StartupPath, "modules"));
            //FindTypes(Application.StartupPath);
        }

		/// <summary>
		/// Find all the types that exist in DLLs specified by path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		protected void FindTypes(string path)
        {
			if (String.IsNullOrEmpty(path))
            {
				throw new ArgumentNullException("The given path is null or empty");
			}

            // Clear this of all the modules
            this.Clear();

			string[] pluginFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

			// Go through each dll file
			for (int i = 0; i < pluginFiles.Length; i++) 
            {
				// load it
				string pathFile = Path.Combine(path, pluginFiles[i]);
                try
                {
                    Assembly assembly = Assembly.LoadFrom(pathFile);

                    if (assembly != null)
                    {
                        // Go through all the public classes in the assembly
                        foreach (var type in assembly.GetExportedTypes())
                        {
                            // If it's parent class is the type we want
                            // and it implements the correct moduleType
                            if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                            {
                                // Create an instance
                                T reaction = (T)Activator.CreateInstance(type);
                                bool hasConfig = reaction.HasConfig;
                                if (!hasConfig || (hasConfig && type.GetInterfaces().Contains(typeof(V))))
                                {
                                    // Add it to our final list
                                    this.Add(reaction);
                                    types.Add(type);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Error opening file: \n" + e);
                }
			}
			this.Sort();
		}
	}
}
