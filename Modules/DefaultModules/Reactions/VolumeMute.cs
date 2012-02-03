using CoreAudioApi;
using MayhemCore;
using System.Runtime.InteropServices;

namespace DefaultModules.Reactions
{
	[MayhemModule("Volume: Mute", "Mutes / unmutes the default audio device")]
	public class VolumeMute : ReactionBase
	{
		public override void Perform()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			MMDevice m_device = null;
			
			try
			{
				m_device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			}
			catch (COMException)
			{
				ErrorLog.AddError(ErrorType.Failure, "No audio output device available.");
				return;
			}

			m_device.AudioEndpointVolume.Mute = !m_device.AudioEndpointVolume.Mute;
		}
	}
}
