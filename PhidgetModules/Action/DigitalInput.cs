using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Phidgets;
using PhidgetModules.Wpf;
using System.Windows;
using Phidgets.Events;

namespace PhidgetModules.Action
{
	[Serializable]
	public class DigitalInput : ActionBase, IWpf, ISerializable
	{
		// Which index do we want to be looking at?
		protected int index;

		// The interface kit we are using for the sensors
		protected InterfaceKit ifKit;

		// Toggle when it goes on, or when it goes off?
		protected bool onWhenOn;

		protected InputChangeEventHandler inputChangeHandler;

		public DigitalInput()
			: base("Phidget: Digital Input", "Triggers on a digital input") {

			index = 0;
			Setup();
		}

		protected virtual void Setup() {
			hasConfig = true;

			this.ifKit = InterfaceFactory.GetInterface();
			onWhenOn = true;

			inputChangeHandler = new InputChangeEventHandler(InputChanged);

			SetConfigString();
		}

		public void WpfConfig() {
			var window = new PhidgetDigitalInputConfig(ifKit, index, onWhenOn);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			window.ShowDialog();

			if (window.DialogResult == true) {
				this.index = window.Index;
				this.onWhenOn = window.OnWhenOn;

				SetConfigString();
			}
		}

		public void SetConfigString() {
			string type = "turns on";
			if (!onWhenOn) {
				type = "turns off";
			}

			ConfigString = String.Format("Triggers when input #{0} {1}", index, type);
		}

		// The input has changed, do the work here
		protected void InputChanged(object sender, InputChangeEventArgs e) {
			// If e.value is true, then it used to be false.
			// Trigger when appropriate
			
			// We are dealing with the right input
			if (e.Index == index) {
				// If its true and we turn on when it turns on
				// then trigger
				if (e.Value == true && onWhenOn) {
					OnActionActivated();
				}
				// otherwise, if it its off, and we trigger
				// when it turns off, then trigger
				else if (e.Value == false && !onWhenOn) {
					OnActionActivated();
				}
			}
		}


		public override void Enable() {
			base.Enable();
			ifKit.InputChange += inputChangeHandler;
		}

		public override void Disable() {
			base.Disable();

			if (ifKit != null) {
				ifKit.InputChange -= inputChangeHandler;
			}
			
		}

		#region Serialization
		public DigitalInput(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			index = info.GetInt32("Index");
			onWhenOn = info.GetBoolean("OnWhenOn");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("Index", index);
			info.AddValue("OnWhenOn", onWhenOn);
		}
		#endregion
	}
}
