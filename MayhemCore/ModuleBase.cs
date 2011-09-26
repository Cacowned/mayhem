﻿
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MayhemCore
{
    /// <summary>
    /// This class is extended by EventBase and ReactionBase
    /// </summary>
    [DataContract]
    public abstract class ModuleBase : INotifyPropertyChanged
    {
        // A reference to the connection that holds this module.
        public Connection Connection { get; set; }

        public bool Enabled { get; private set; }

        public bool IsConfiguring { get; set; }

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

        public bool HasConfig
        {
            get;
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _configString;
        public string ConfigString
        {
            get
            {
                return _configString;
            }
            set
            {
                _configString = value;
                OnPropertyChanged("ConfigString");
            }
        }

        internal void Enable_()
        {
            try
            {
                Enabled = Enable();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Name);
            }
        }

        public virtual bool Enable()
        {
            return true;
        }

        internal void Disable_()
        {
            try
            {
                Disable();
                Enabled = false;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error disabling " + Name);
            }
        }
        public virtual void Disable()
        {
        }

        public virtual void Delete()
        {
        }

        private void _Initialize()
        {
            Type configurableType = MayhemEntry.Instance.ConfigurableType;
            Type[] interfaceTypes = GetType().GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType.Equals(configurableType))
                {
                    HasConfig = true;
                    break;
                }
            }
            object[] attList = GetType().GetCustomAttributes(typeof(MayhemModuleAttribute), true);
            if (attList.Length > 0)
            {
                MayhemModuleAttribute att = attList[0] as MayhemModuleAttribute;
                this.Name = att.Name;
                this.Description = att.Name;
            }
        }

        protected ModuleBase()
        {
            _Initialize();
            try
            {
                Initialize();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc)
        {
            _Initialize();
            try
            {
                Initialize();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            SetConfigString();
        }

        protected virtual void Initialize() { }

        public override string ToString()
        {
            return Name;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public virtual void SetConfigString() { }
    }
}
