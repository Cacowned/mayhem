
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using MayhemCore.ModuleTypes;
using System.Reflection;
using System.Diagnostics;

namespace MayhemCore
{

    /// <summary>
    /// This class is extended by EventBase and ReactionBase
    /// </summary>
    [DataContract]
    public abstract class ModuleBase : INotifyPropertyChanged
    {
        // A reference to the connection that holds this module.
        internal Connection Connection
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get;
            private set;
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
        internal string ConfigString
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

        protected ModuleBase()
        {
            try
            {
                Initialize_();
                OnBeforeLoad();
                OnLoadDefaults();
                OnAfterLoad();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
            }
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            object[] attList = this.GetType().GetCustomAttributes(typeof(DataContractAttribute), true);
            if (attList.Length > 0)
            {
                Debug.WriteLine(context.State);
                Initialize_();
                try
                {
                    OnBeforeLoad();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            object[] attList = this.GetType().GetCustomAttributes(typeof(DataContractAttribute), true);
            if (attList.Length > 0)
            {
                try
                {
                    OnLoadFromSaved();
                    OnAfterLoad();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error loading " + Name);
                }
                SetConfigString();
            }
        }

        protected virtual void OnBeforeLoad() { }
        protected virtual void OnLoadDefaults() { }
        protected virtual void OnLoadFromSaved() { }
        protected virtual void OnAfterLoad() { }

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

        internal void Enable(EnablingEventArgs e)
        {
            try
            {
                OnEnabling(e);
                if (!e.Cancel)
                    IsEnabled = true;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Name);
            }
        }

        protected virtual void OnEnabling(EnablingEventArgs e)
        {
        }

        internal void Disable(DisabledEventArgs e)
        {
            try
            {
                OnDisabled(e);
                IsEnabled = false;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Error disabling " + Name);
            }
        }

        protected virtual void OnDisabled(DisabledEventArgs e)
        {
        }

        internal void Delete()
        {
            OnDeleted();
        }

        protected virtual void OnDeleted()
        {
        }

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
