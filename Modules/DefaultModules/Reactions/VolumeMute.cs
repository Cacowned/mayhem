using CoreAudioApi;
using MayhemCore;

namespace DefaultModules.Reactions
{
	[MayhemModule("Volume: Mute", "Mutes / unmutes the default audio device")]
	public class VolumeMute : ReactionBase
	{
		public override void Perform()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			MMDevice m_device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			m_device.AudioEndpointVolume.Mute = !m_device.AudioEndpointVolume.Mute;
		}
	}
}
