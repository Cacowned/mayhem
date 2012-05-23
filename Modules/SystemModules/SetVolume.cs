using System.Runtime.Serialization;
using CoreAudioApi;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Runtime.InteropServices;

namespace SystemModules
{
	[DataContract]
	[MayhemModule("Set Volume", "Sets the master volume level")]
	public class SetVolume : ReactionBase, IWpfConfigurable
	{
		[DataMember]
		private int level;

		protected override void OnLoadDefaults()
		{
			level = 50;
		}

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
			device.AudioEndpointVolume.MasterVolumeLevelScalar = level / 100.0f;
		}

		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SetVolumeConfig(level);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = (SetVolumeConfig)configurationControl;
			this.level = config.Level;
		}

		public string GetConfigString()
		{
			return string.Format("Set the volume to {0}%", level);
		}
	}
}
