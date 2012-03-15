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
        internal Connection Connection
        {
            get;
            set;
        }

        /// <summary>
        /// True if the module is enabled, false otherwise
        /// </summary>
        public bool IsEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the module
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// The description of the module. This shows up in the module lists 
        /// </summary>
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

        private string configString;

        internal string ConfigString
        {
            get
            {
                return configString;
            }

            private set
            {
                configString = value;
                OnPropertyChanged("ConfigString");
            }
        }

        protected ModuleBase()
        {
            try
            {
                InitializeHelper();
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
            object[] attList = GetType().GetCustomAttributes(typeof(DataContractAttribute), true);
            if (attList.Length > 0)
            {
                InitializeHelper();
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
            object[] attList = GetType().GetCustomAttributes(typeof(DataContractAttribute), true);
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

        /// <summary>
        /// This method is called first, you should use it to instantiate any variables that are
        /// needed to load your configuration and setup. For example, if you will need to add saved
        /// items to an object, you should instantiate that object in this method so that it will
        /// exist in OnLoadDefaults and OnLoadFromSaved.
        /// This method is called on the main thread.
        /// </summary>
        protected virtual void OnBeforeLoad()
        {
        }

        /// <summary>
        /// This method is called after OnBeforeLoad when your module object is created because of
        /// standard instantiation. Use this method to set the defaults on your configuration
        /// variables.
        /// This method is called on the main thread.
        /// </summary>
        protected virtual void OnLoadDefaults()
        {
        }

        /// <summary>
        /// This method is called after OnBeforeLoad when your module object is created because of
        /// deserialization. In this method you are guaranteed that all variables marked with the
        /// DataMember attribute are set.
        /// This method is called on the main thread.
        /// </summary>
        protected virtual void OnLoadFromSaved()
        {
        }

        /// <summary>
        /// This method is called after both OnLoadDefaults and OnLoadFromSaved. This method is
        /// guaranteed that all configuration properties are set.
        /// </summary>
        protected virtual void OnAfterLoad()
        {
        }

        private void InitializeHelper()
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
                Name = att.Name;
                Description = att.Name;
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

        /// <summary>
        /// This method is called before enabling the module. The module should verify that it can
        /// be started up. If for whatever reason it cannot, then e.Cancel should be set to true.
        /// The property WasConfiguring is set to true in the case that we are re-enabling after
        /// the configuration window was closed.
        /// This method is called on a background thread.
        /// </summary>
        /// <param name="e">Enabling arguments object</param>
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

        /// <summary>
        /// This method is called when the module is disabled; on application shutdown, on
        /// connection deleted, and when the configuration for the module is opened. In the case
        /// that it is called when configuration is opened, e.IsConfiguring will be set to true.
        /// This method is called on a background thread.
        /// </summary>
        /// <param name="e">Disabled event arguments</param>
        protected virtual void OnDisabled(DisabledEventArgs e)
        {
        }

        internal void Delete()
        {
            OnDeleted();
        }

        /// <summary>
        /// This method is called when the connection containing this module is deleted.
        /// </summary>
        protected virtual void OnDeleted()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void SetConfigString()
        {
        	var configurable = this as IConfigurable;
        	if (configurable != null)
        	{
        		ConfigString = configurable.GetConfigString();
        	}
        }
    }
}
