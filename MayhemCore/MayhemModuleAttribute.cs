using System;

namespace MayhemCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MayhemModuleAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public MayhemModuleAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
