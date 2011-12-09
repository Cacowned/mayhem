using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;

namespace MayhemCore
{
    /// <summary>
    /// Helper methods for Event and Reaction lists
    /// </summary>
    /// <typeparam name="T">EventBase or ReactionBase</typeparam>
    internal abstract class ModuleList<T> : BindingCollection<ModuleType>
    {
        private readonly List<Type> allTypes;

        private string Location;

        private DateTime lastUpdated;

        protected ModuleList()
        {
            allTypes = new List<Type>();
        }

        internal bool ScanModules(string path)
        {
            Location = path;

            FileSystemWatcher fileWatcher = new FileSystemWatcher(Location);
            fileWatcher.IncludeSubdirectories = true;
            fileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileWatcher.Changed += UpdateDependencies;
            fileWatcher.Created += UpdateDependencies;
            fileWatcher.Deleted += UpdateDependencies;
            fileWatcher.Renamed += UpdateDependencies;
            fileWatcher.EnableRaisingEvents = true;

            lastUpdated = DateTime.Now;
            // Load up all the types of things that we want in the application root
            return FindTypes(Location);
        }

        private void UpdateDependencies(object source, FileSystemEventArgs e)
        {
            var nowTime = DateTime.Now;

            if ((nowTime - lastUpdated).TotalMilliseconds > 30)
            {
                FindTypes(Location);
                lastUpdated = nowTime;
            }
        }

        /// <summary>
        /// Find all the types that exist in DLLs specified by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool FindTypes(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(path, "The given path is null or empty");
            }

            // Clear this of all the modules
            Clear();


            if (!Directory.Exists(path))
                return false;

            string[] pluginFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

            // Go through each dll file
            foreach (string pluginFile in pluginFiles)
            {
                // load it
                string pathFile = Path.Combine(path, pluginFile);
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
                                object[] attList = type.GetCustomAttributes(typeof(MayhemModuleAttribute), true);
                                if (attList.Length > 0)
                                {
                                    MayhemModuleAttribute att = attList[0] as MayhemModuleAttribute;

                                    // Add it to our final list
                                    Add(new ModuleType(type, att.Name, att.Description));
                                }
                                else
                                {
                                    // TODO: Do something to tell the developer the module has an error
                                    // throw new Exception("Module does not have MayhemModuleAttribute attribute set:\n" + type.FullName);
                                }
                            }

                            allTypes.Add(type);
                        }
                    }
                }
                catch
                {
                }
            }

            //Sort(new ModuleTypeComparer());
            return true;
        }

        internal Type[] GetAllTypesInModules()
        {
            return allTypes.ToArray();
        }
    }
}
