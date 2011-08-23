
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MayhemCore
{
    /// <summary>
    /// This class is extended by EventBase and ReactionBase
    /// </summary>
    [DataContract]
    public abstract class ModuleBase : IComparable<ModuleBase>, INotifyPropertyChanged
    {
        // A reference to the connection that holds this module.
        public Connection connection;

        public bool Enabled { get; private set; }

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

        public virtual void Enable()
        {
            this.Enabled = true;
        }
        public virtual void Disable()
        {
            this.Enabled = false;
        }

        public virtual void Delete()
        {
        }

        private void _Initialize()
        {
            Type configurableType = Mayhem.Instance.ConfigurableType;
            Type[] interfaceTypes = GetType().GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (interfaceType.Equals(configurableType))
                {
                    HasConfig = true;
                    break;
                }
            }
            object[] attList = GetType().GetCustomAttributes(typeof(MayhemModule), true);
            if (attList.Length > 0)
            {
                MayhemModule att = attList[0] as MayhemModule;
                this.Name = att.Name;
                this.Description = att.Name;
            }
        }

        public ModuleBase()
        {
            _Initialize();
            Initialize();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc)
        {
            _Initialize();
            Initialize();
            SetConfigString();  
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
         
        }

        protected virtual void Initialize() { }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(ModuleBase obj)
        {
            return String.Compare(this.Name, obj.Name);
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
