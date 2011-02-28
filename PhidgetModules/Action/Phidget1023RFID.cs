﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;
using MayhemCore.ModuleTypes;
using PhidgetModules.Wpf;
using System.Windows;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1023RFID : ActionBase, IWpf, ISerializable
	{
		protected static RFID rfid;

		protected TagEventHandler gotTag;

		protected TagEventHandler lostTag;

		// This is the tag we are watching for
		protected string ourTag;

		public Phidget1023RFID()
			: base("Phidget-1023: RFID", "Triggers with a certain RFID Tag") {
			Setup();
		}

		protected virtual void Setup() {
			hasConfig = true;

			if (rfid == null) {
				rfid = new RFID();
				rfid.open();
				//rfid.Attach += new AttachEventHandler(rfid_Attach);
				//rfid.Detach += new DetachEventHandler(rfid_Detach);
				//rfid.Error += new ErrorEventHandler(rfid_Error);

				//rfid.Tag += new TagEventHandler(rfid_Tag);
				//rfid.TagLost += new TagEventHandler(rfid_TagLost);
			}

			gotTag = new TagEventHandler(rfidTag);
			lostTag = new TagEventHandler(lostRfidTag);
			SetConfigString();
		}


		public void WpfConfig() {

			var window = new Phidget1023RFIDConfig(rfid, ourTag);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			window.ShowDialog();

			if (window.DialogResult == true) {
				ourTag = window.TagID;

				SetConfigString();
			}
		}

		protected void SetConfigString() {
			ConfigString = String.Format("RFID Tag ID {0}", ourTag);
		}


		//Tag event handler...we'll display the tag code in the field on the GUI
		void rfidTag(object sender, TagEventArgs e) {
			if (e.Tag == ourTag) {
				OnActionActivated();
				rfid.LED = true;
			}
		}

		//Tag event handler...we'll display the tag code in the field on the GUI
		void lostRfidTag(object sender, TagEventArgs e) {
			rfid.LED = false;
		}

		public override void Enable() {
			base.Enable();
			rfid.Tag += gotTag;
			rfid.TagLost += lostTag;
		}

		public override void Disable() {
			base.Disable();
			rfid.Tag -= gotTag;
			rfid.TagLost -= lostTag;
		}

		#region Serialization

		public Phidget1023RFID(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			ourTag = info.GetString("TagID");
			Setup();

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("TagID", ourTag);
			// Save the index
		}
		#endregion
	}
}
