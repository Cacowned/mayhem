
using System;
using System.Runtime.Serialization;
using System.Windows;

namespace MayhemCore
{
	/// <summary>
	/// This class is extended by ActionBase and ReactionBase
	/// </summary>
	public abstract class ModuleBase : DependencyObject, IComparable<ModuleBase>, ISerializable
	{
		// A reference to the connection that holds this module.
		public Connection connection;

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

		public string Name {
			get;
			protected set;
		}

		public string Description {
			get;
			protected set;
		}

		public string ConfigString {
			get { return (string)GetValue(ConfigStringProperty); }
			set { SetValue(ConfigStringProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ConfigString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ConfigStringProperty =
			DependencyProperty.Register("ConfigString", typeof(string), typeof(ModuleBase), new UIPropertyMetadata(string.Empty));


		// TODO: category?


		public virtual void Enable() {
			this.Enabled = true;
		}
		public virtual void Disable() {
			this.Enabled = false;
		}

		public ModuleBase(string name, string description) {
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
		public ModuleBase(SerializationInfo info, StreamingContext context) {
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
