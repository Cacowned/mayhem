using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;

namespace MayhemCore
{
    /// <summary>
    /// Helper methods for Action and Reaction lists
    /// </summary>
    /// <typeparam name="V">The interface type that modules must implement</typeparam>
    public abstract class ModuleList<T, V> : List<T>
    {
        public ModuleList() {
            FindTypes(Application.StartupPath);
        }
        /// <summary>
        /// Find all the types that exist in dlls specified by path
        /// </summary>
        /// <typeparam name="P">The interface type that the class must implement</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        protected void FindTypes(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("The given path is null or empty");
            }

            string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

            // Go through each dll file
            for (int i = 0; i < pluginFiles.Length; i++)
            {

                string fileName = Path.GetFileNameWithoutExtension(pluginFiles[i]);
                // load it
                Assembly ass = Assembly.Load(fileName);
                if (ass != null)
                {
                    // Go through all the public classes in the assembly
                    foreach (var type in ass.GetExportedTypes())
                    {
                        //bool hasConfig = (bool)type.GetProperty("HasConfig");
                        // If it's parent class is the type we want
                        // and it implements the correct moduleType
                        if (type.BaseType == typeof(T) && 
                            type.GetProperty("HasConfig").Equals(true) &&
                            type.GetInterfaces().Contains(typeof(V)))
                        {
                            
                            // Create an instance
                            T reaction = (T)Activator.CreateInstance(type);
                            // Add it to our final list
                            this.Add(reaction);
                        }
                    }
                }
            }
        }
    }
}
