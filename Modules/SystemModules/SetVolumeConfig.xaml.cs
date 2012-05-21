using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemWpf.UserControls;

namespace SystemModules
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
