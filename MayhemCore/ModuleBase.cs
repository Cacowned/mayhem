
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using MayhemCore.ModuleTypes;

namespace MayhemCore
{

    /// <summary>
    /// This class is extended by EventBase and ReactionBase
    /// </summary>
    [DataContract]
    public abstract class ModuleBase : INotifyPropertyChanged
    {
        // A reference to the connection that holds this module.
        public Connection Connection 
        {
            get;
            internal set; 
        }

        public bool IsEnabled 
        {
            get;
            private set; 
        }

        public bool IsConfiguring 
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public string Description
        {
            get;
            internal set;
        }

        internal bool HasConfig
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
            private set
            {
                _configString = value;
                OnPropertyChanged("ConfigString");
            }
        }

        internal void Enable()
        {
            try
            {
                IsEnabled = OnEnable();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Name);
            }
        }

        protected virtual bool OnEnable()
        {
            return true;
        }

        internal void Disable()
        {
            try
            {
                OnDisable();
                IsEnabled = false;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error disabling " + Name);
            }
        }

        protected virtual void OnDisable()
        {
        }

        internal void Delete()
        {
            OnDelete();
        }

        protected virtual void OnDelete()
        {
        }

        private void Initialize_()
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
            Initialize_();
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
            Initialize_();
            try
            {
                Initialize();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
            }
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

        internal void SetConfigString() 
        {
            if (this is IConfigurable)
            {
                ConfigString = ((IConfigurable)this).GetConfigString();
            }
        }
    }
}
