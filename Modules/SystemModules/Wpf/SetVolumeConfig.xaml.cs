using MayhemWpf.UserControls;

namespace SystemModules.Wpf
{
	public partial class SetVolumeConfig : WpfConfiguration
	{
		public int Level
		{
			get;
			private set;
		}

		public SetVolumeConfig(int level)
		{
			this.Level = level;
			InitializeComponent();
		}

		public override void OnLoad()
		{
			VolumeSlider.Value = this.Level;
			CanSave = true;
		}

		public override void OnSave()
		{
			this.Level = (int)VolumeSlider.Value;
		}

		public override string Title
		{
			get
			{
				return "Set Volume";
			}
		}
	}
}
