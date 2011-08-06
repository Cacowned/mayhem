using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MayhemModule : Attribute
    {
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

        public MayhemModule(string name, string description)  // url is a positional parameter
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
