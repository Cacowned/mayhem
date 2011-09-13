using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    public class ModuleType : IComparable
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

        public int CompareTo(object obj)
        {
            if (obj is ModuleType)
            {
                return Name.CompareTo(((ModuleType)obj).Name);
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
