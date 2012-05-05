﻿using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;
using System;

namespace PhidgetModules
{
	[DataContract]
	public abstract class SensorEventBase : EventBase
	{
		// The interface kit we are using for the sensors
		private InterfaceKit ifKit;

		// Which index do we want to be looking at?
		[DataMember]
		protected int Index
		{
			get;
			set;
		}
		
		protected override void OnLoadDefaults()
		{
			// Default to first index
			Index = 0;
		}

		protected abstract void SensorChange(object sender, SensorChangeEventArgs ex);

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (!e.WasConfiguring)
			{
				try
				{
					ifKit = PhidgetManager.Get<InterfaceKit>();
				}
				catch (InvalidOperationException)
				{
					ErrorLog.AddError(ErrorType.Failure, "The Phidget interface kit is not attached");
					e.Cancel = true;
					return;
				}
			}
			ifKit.SensorChange += SensorChange;
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			ifKit.SensorChange -= SensorChange;
			if (!e.IsConfiguring)
			{
				PhidgetManager.Release<InterfaceKit>(ref ifKit);
			}
		}
	}
}
