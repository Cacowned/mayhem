using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using CoreAudioApi;
using MayhemCore;

namespace SystemModules
{
	[DataContract]
	[MayhemModule("Decrease Volume", "Increases the master volume level")]
	public class DecreaseVolume : ReactionBase
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

			device.AudioEndpointVolume.VolumeStepDown();
		}
	}
}
