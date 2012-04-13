using System.Runtime.Serialization;
using System.Threading;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using WindowModules.Wpf;

namespace WindowModules.Reactions
{
	[DataContract]
	[MayhemModule("Window Sequence", "Manipulates an application window with a sequence of actions")]
	public class WindowSequence : ReactionBase, IWpfConfigurable
	{
		[DataMember]
		public WindowActionInfo ActionInfo
		{
			get;
			private set;
		}

		protected override void OnLoadDefaults()
		{
			ActionInfo = new WindowActionInfo();
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new WindowSequenceConfig(ActionInfo); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			WindowSequenceConfig config = (WindowSequenceConfig)configurationControl;
			ActionInfo = config.ActionInfo;
		}

		public string GetConfigString()
		{
			return ActionInfo.WindowInfo.Title;
		}

		public override void Perform()
		{
			int num = WindowFinder.Find(ActionInfo, hwnd =>
				{
					foreach (IWindowAction action in ActionInfo.WindowActions)
					{
						action.Perform(hwnd);
						Thread.Sleep(50);
					}
				}
			);

			// If we can't find the window, then num appears to be non-zero.
			if (num != 0)
			{
				ErrorLog.AddError(ErrorType.Failure, string.Format("The Window '{0}' is not avaliable", ActionInfo.WindowInfo.Title));
			}
		}
	}
}
