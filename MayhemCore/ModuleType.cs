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
}
