
using System;
using System.Runtime.Serialization;
namespace MayhemCore
{
    /// <summary>
    /// This class is extended by ActionBase and ReactionBase
    /// </summary>
    public abstract class ModuleBase: IComparable<ModuleBase>, ISerializable
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

        public virtual string ConfigString
        {
            get { return String.Empty; }
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

        #region Serialization
        public ModuleBase(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Description = info.GetString("Description");
            hasConfig = info.GetBoolean("HasConfig");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Name", Name);
            info.AddValue("Description", Description);
            info.AddValue("HasConfig", HasConfig);
        }
        #endregion
    }
}
