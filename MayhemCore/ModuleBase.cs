
using System;
namespace MayhemCore
{
    /// <summary>
    /// This class is extended by ActionBase and ReactionBase
    /// </summary>
    public abstract class ModuleBase: IComparable<ModuleBase>
    {
        protected bool hasConfig = false;
        /// <summary>
        /// Whether this module has configuration settings
        /// </summary>
        public bool HasConfig {
            get {
                return hasConfig;
            }
        }

        protected bool Enabled { get; set; }

        public string Name
        {
            get;
            protected set;
        }
        
        public string Description
        {
            get;
            protected set;
        }
        
        // TODO: category?


        public virtual void Enable() {
            this.Enabled = true;
        }
        public virtual void Disable() {
            this.Enabled = false;
        }

        public ModuleBase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public override string ToString() {
            return Name;
        }

        public int CompareTo(ModuleBase obj) {
            return String.Compare(this.Name, obj.Name);
        }
    }
}
