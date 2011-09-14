using System;
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
    public abstract class ModuleList<T> : List<ModuleType>
	{
        List<Type> allTypes = new List<Type>();
        public ModuleList() 
        {
		}

        public bool ScanModules(string path) 
        {
            // Load up all the types of things that we want in the application root
            return FindTypes(path);
        }

		/// <summary>
		/// Find all the types that exist in DLLs specified by path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		protected bool FindTypes(string path)
        {
			if (String.IsNullOrEmpty(path))
            {
				throw new ArgumentNullException(path, "The given path is null or empty");
			}

            // Clear this of all the modules
            this.Clear();

            if (!Directory.Exists(path))
                return false;

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
                        foreach (Type type in assembly.GetExportedTypes())
                        {
                            // If it's parent class is the type we want
                            // and it implements the correct moduleType
                            if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                            {
                                object [] attList = type.GetCustomAttributes(typeof(MayhemModule), true);
                                if (attList.Length > 0)
                                {
                                    MayhemModule att = attList[0] as MayhemModule;
                                    // Add it to our final list
                                    this.Add(new ModuleType(type, att.Name, att.Description));
                                }
                                else
                                {
                                    ///TODO: Do something to tell the developer the module has an error
                                   // throw new Exception("Module does not have MayhemModule attribute set:\n" + type.FullName);
                                }
                            }
                            allTypes.Add(type);
                            //Logger.WriteLine(type);
                        }
                    }
                }
                catch // (Exception e)
                {
                  //  Trace.WriteLine("Error opening file: \n" + e);
                }
			}
            this.Sort();
            return true;
		}

        public Type[] GetAllTypesInModules()
        {
            //List<Type> types = new List<Type>();
            //foreach (ModuleType moduleType in this)
            //{
            //    types.Add(moduleType.Type);
            //}
            return allTypes.ToArray();
        }
	}
}
