using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace MayhemCore
{
    /// <summary>
    /// Helper methods for Action and Reaction lists
    /// </summary>
    public abstract class ModuleList
    {

        /// <summary>
        /// Find all the types that exist in dlls specified by path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        protected List<T> FindTypes<T>(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("The given path is null or empty");
            }

            var typeList = new List<T>();

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
                    foreach (var t in ass.GetExportedTypes())
                    {
                        // If it is the type we want
                        if (t.BaseType == typeof(T))
                        {
                            // Create an instance
                            T reaction = (T)Activator.CreateInstance(t);
                            // Add it to our final list
                            typeList.Add(reaction);
                        }
                    }
                }
            }
            return typeList;
        }
    }
}
