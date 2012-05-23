using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CoreAudioApi;
using MayhemCore;

namespace SystemModules
{
	[MayhemModule("Mute Volume", "Mutes / unmutes the default audio device")]
	public class MuteVolume : ReactionBase
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

			device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
		}
	}
}
