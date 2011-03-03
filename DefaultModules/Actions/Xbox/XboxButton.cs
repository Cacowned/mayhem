using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DefaultModules.Actions.Xbox
{
	[Serializable]
	public class XboxButton : ActionBase, ICli, IWpf, ISerializable
	{
		protected BackgroundWorker bw = new BackgroundWorker();

		protected const string TAG = "[Xbox]";

		protected GamePadButtons buttons;
		//protected GamePadState configState;
		protected PlayerIndex player = PlayerIndex.One;

		public XboxButton()
			: base("Xbox Controller: Button", "Triggers when buttons on an Xbox 360 controller are pushed") {
			var defaultButtons = new Buttons();
			defaultButtons = defaultButtons | Buttons.A;

			buttons = new GamePadButtons(defaultButtons);

			Setup();
		}

		protected void Setup() {
			hasConfig = true;

			bw.WorkerReportsProgress = true;
			bw.WorkerSupportsCancellation = true;
			bw.DoWork += new DoWorkEventHandler(WatchButtons);
			bw.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);

			SetConfigString();
		}

		public void SetConfigString() {
			ConfigString = buttons.ToString();
		}

		public override void Enable() {
			base.Enable();
			bw.RunWorkerAsync();
		}

		public override void Disable() {
			base.Disable();
			bw.CancelAsync();
		}

		protected void WatchButtons(object sender, DoWorkEventArgs e) {
			bool wasEqual = false;

			BackgroundWorker worker = sender as BackgroundWorker;

			while (true) {

				if (worker.CancellationPending == true)
					return;

				var state2 = GamePad.GetState(player).Buttons;
				bool isEqual = StateEquals(state2);
				if (wasEqual == false && isEqual == true) {
					// It doesn't matter what we return
					worker.ReportProgress(0);
				}

				wasEqual = isEqual;

				Thread.Sleep(50);
			}
		}

		protected void ProgressChanged(object sender, ProgressChangedEventArgs e) {
			Debug.WriteLine(String.Format("{0} Button Pressed", TAG));
			base.OnActionActivated();
		}


		public void CliConfig() {
			int playerNum;

			string input = "";
			do {
				Console.WriteLine(String.Format("{0} What player's controller? 1-4.", TAG));
				input = Console.ReadLine();
			}
			while (!Int32.TryParse(input, out playerNum) || !(playerNum >= 1 && playerNum <= 4));

			// TODO: Need to make sure that the player is plugged in.
			switch (playerNum) {
				case 2: player = PlayerIndex.Two;
					break;
				case 3: player = PlayerIndex.Three;
					break;
				case 4: player = PlayerIndex.Four;
					break;
				default: player = PlayerIndex.One;
					break;
			}

			do {
				Console.WriteLine(String.Format("{0} Push buttons on the controller and press enter to set.", TAG));
				Console.ReadLine();
				buttons = GamePad.GetState(player).Buttons;
			}
			while (buttons.ToString() == "{Buttons:None}");

			Console.WriteLine(String.Format("{0} Saved state {1}", TAG, buttons.ToString()));

			SetConfigString();
		}

		public void WpfConfig() {
			var window = new XboxButtonConfig(buttons);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;



			window.ShowDialog();

			if (window.DialogResult == true) {
				buttons = new GamePadButtons(window.down_buttons);

				SetConfigString();
			}
		}


		/// <summary>
		/// Checks the gamepad state against the one saved from configuration
		/// </summary>
		/// <param name="checkState"></param>
		public bool StateEquals(GamePadButtons checkState) {
			return buttons.Equals(checkState);
			//return configState.Buttons.Equals(checkState.Buttons) && configState.DPad.Equals(checkState.DPad);
		}

		#region Serialization
		public XboxButton(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			buttons = new GamePadButtons((Buttons)info.GetValue("Buttons", typeof(Buttons)));
			player = (PlayerIndex)info.GetInt32("Player");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("Player", (int)player);

			// We need to create a buttons struct from all the buttons that are pressed
			Buttons keys = new Buttons();
			MergeButtons(ref keys, this.buttons);

			/*if ((int)this.buttons.A == 1)
				keys = keys | Buttons.A;
			if ((int)this.buttons.B == 1)
				keys = keys | Buttons.B;
			if ((int)this.buttons.Back == 1)
				keys = keys | Buttons.Back;
			if ((int)this.buttons.BigButton == 1)
				keys = keys | Buttons.BigButton;
			if ((int)this.buttons.LeftShoulder == 1)
				keys = keys | Buttons.LeftShoulder;
			if ((int)this.buttons.LeftStick == 1)
				keys = keys | Buttons.LeftStick;
			if ((int)this.buttons.RightShoulder == 1)
				keys = keys | Buttons.RightShoulder;
			if ((int)this.buttons.RightStick == 1)
				keys = keys | Buttons.RightStick;
			if ((int)this.buttons.Start == 1)
				keys = keys | Buttons.Start;
			if ((int)this.buttons.X == 1)
				keys = keys | Buttons.X;
			if ((int)this.buttons.Y == 1)
				keys = keys | Buttons.Y;
			*/
			info.AddValue("Buttons", keys);
		}
		#endregion

		// Merges the states of buttons into the original struct
		public static void MergeButtons(ref Buttons original, GamePadButtons buttons) {
			if ((int)buttons.A == 1)
				original = original | Buttons.A;
			if ((int)buttons.B == 1)
				original = original | Buttons.B;
			if ((int)buttons.Back == 1)
				original = original | Buttons.Back;
			if ((int)buttons.BigButton == 1)
				original = original | Buttons.BigButton;
			if ((int)buttons.LeftShoulder == 1)
				original = original | Buttons.LeftShoulder;
			if ((int)buttons.LeftStick == 1)
				original = original | Buttons.LeftStick;
			if ((int)buttons.RightShoulder == 1)
				original = original | Buttons.RightShoulder;
			if ((int)buttons.RightStick == 1)
				original = original | Buttons.RightStick;
			if ((int)buttons.Start == 1)
				original = original | Buttons.Start;
			if ((int)buttons.X == 1)
				original = original | Buttons.X;
			if ((int)buttons.Y == 1)
				original = original | Buttons.Y;
		}
	}
}
