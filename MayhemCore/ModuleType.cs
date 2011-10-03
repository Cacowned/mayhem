using System;
using System.Collections.Generic;

namespace MayhemCore
{
    internal class ModuleType
    {
        public Type Type
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }

        public ModuleType(Type type, string name, string description)
        {
            this.Type = type;
            this.Name = name;
            this.Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal class ModuleTypeComparer : IComparer<ModuleType>
    {
        public int Compare(ModuleType x, ModuleType y)
        {
            EnsureNotNull(x);
            EnsureNotNull(y);
            return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        private static void EnsureNotNull(ModuleType type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
        }
    }
}
