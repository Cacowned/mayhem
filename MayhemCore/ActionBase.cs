
using System;
using System.Runtime.Serialization;
namespace MayhemCore
{
	public delegate void ActionActivateHandler(object sender, EventArgs e);

	/// <summary>
	/// Base class for all action modules
	/// </summary>
	public abstract class ActionBase : ModuleBase, ISerializable
	{
		/// <summary>
		/// Event that triggers when the action is activated
		/// </summary>
		public event ActionActivateHandler ActionActivated;

		public ActionBase(string name, string description)
			: base(name, description) {
		}

		/// <summary>
		/// Event trigger for when the action is activated. This shouldn't
		/// need to be overridden, just attached to
		/// </summary>
		protected virtual void OnActionActivated() {
			ActionActivateHandler handler = ActionActivated;
			if (handler != null) {
				handler(this, null);
			}
		}


		#region Serialization
		public ActionBase(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			ActionActivated = (ActionActivateHandler)info.GetValue("ActionActivated", typeof(ActionActivateHandler));
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("ActionActivated", ActionActivated);
		}

		#endregion
	}
}
