using System;

namespace MayhemCore
{
    /// <summary>
    /// This attribute defines a class as being a module for Mayhem
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MayhemModuleAttribute : Attribute
    {
        /// <summary>
        /// The name of the module
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The description of the module
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new mayhem module attribute
        /// </summary>
        /// <param name="name">The name of the module</param>
        /// <param name="description">The description of the module</param>
        public MayhemModuleAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
