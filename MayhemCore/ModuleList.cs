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
    public abstract class ModuleList<T> : List<T>
    {
        public ModuleList() {
            FindTypes(Application.StartupPath);
        }
        /// <summary>
        /// Find all the types that exist in dlls specified by path
        /// </summary>
        /// <typeparam name="T"></typeparam>
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
                        // If it's parent class is the type we want
                        if (type.BaseType == typeof(T))
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
