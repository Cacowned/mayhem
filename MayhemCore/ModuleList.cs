using System;
using System.Collections.Generic;
using System.Reflection;

namespace MayhemCore
{
	/// <summary>
	/// Helper methods for Event and Reaction lists
	/// </summary>
	/// <typeparam name="T">EventBase or ReactionBase</typeparam>
	internal abstract class ModuleList<T> : BindingCollection<ModuleType>
	{
		private readonly List<Type> allTypes;

		protected ModuleList()
		{
			allTypes = new List<Type>();
		}

		internal void TryAdd(Assembly assembly)
		{
			try
			{
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
				// Swallow
			}
		}

		internal Type[] GetAllTypesInModules()
		{
			return allTypes.ToArray();
		}
	}
}
