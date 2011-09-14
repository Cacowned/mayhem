using System;

namespace MayhemCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MayhemModule : Attribute
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

        public MayhemModule(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
