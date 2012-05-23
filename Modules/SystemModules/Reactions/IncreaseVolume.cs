﻿using System.Runtime.InteropServices;
using CoreAudioApi;
using MayhemCore;

namespace SystemModules.Reactions
{
	[MayhemModule("Increase Volume", "Increases the master volume level")]
	public class IncreaseVolume : ReactionBase
	{
		public override void Perform()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			MMDevice device;

			try
			{
				device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			}
			catch (COMException)
			{
				ErrorLog.AddError(ErrorType.Failure, "No audio output device available.");
				return;
			}

			device.AudioEndpointVolume.VolumeStepUp();
		}
	}
}
