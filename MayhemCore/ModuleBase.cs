
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

        [DataMember]
		protected bool hasConfig = false;
		/// <summary>
		/// Whether this module has configuration settings
		/// </summary>
		public bool HasConfig {
			get {
				return hasConfig;
			}
		}

		public bool Enabled { get; private set; }

        [DataMember]
		public string Name {
			get;
			protected set;
		}

        [DataMember]
		public string Description {
			get;
			protected set;
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

		public virtual void Enable() {
			this.Enabled = true;
		}
		public virtual void Disable() {
			this.Enabled = false;
		}

		public ModuleBase(string name, string description) {
			this.Name = name;
			this.Description = description;

            Initialize();
		}

        [OnDeserializing]
        private void OnDeserializing(StreamingContext sc)
        {
            Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            SetConfigString();
        }

        protected virtual void Initialize() { }

		public override string ToString() {
			return Name;
		}

		public int CompareTo(ModuleBase obj) {
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
