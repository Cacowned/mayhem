/*
 * 
 * VideoConfig.cs
 * 
 * Config for the Video Reaction. Extends PictureConfig. 
 * 
 * Derives from PictureConfig.cs, so check out the xaml there 
 * 
 * 
 */
using System.Collections.Generic;
using System.Windows.Controls;
using MayhemOpenCVWrapper;
using VisionModules.Reactions;

namespace VisionModules.Wpf
{
    public class VideoConfig : PictureConfig
    {
        private CheckBox compress;

        public VideoRecordingMode RecordingMode
        {
            get;
            private set;
        }

        private const int VideoDurationS = Camera.LoopDuration / 1000;

        /// <summary>
        /// Content of the Checkbox items, for selection of the video recording timing modes. 
        /// </summary>
        private readonly Dictionary<string, VideoRecordingMode> checkboxItems;
        private ComboBox cbxTimeOffset;

        public bool CompressVideo
        {
            get
            {
                if (compress.IsChecked != null)
                {
                    return (bool)compress.IsChecked;
                }

                return false;
            }
        }

        public VideoConfig(string location, string prefix, VideoRecordingMode recordingMode, int deviceIdx)
            : base(location, prefix, 0, deviceIdx)
        {
            checkboxItems = new Dictionary<string, VideoRecordingMode>();
            checkboxItems.Add(VideoDurationS / 2 + "s before until " + VideoDurationS / 2 + "s after the event (default)", VideoRecordingMode.MidEvent);
            checkboxItems.Add(VideoDurationS + "s before the event until the event occurred", VideoRecordingMode.PreEvent);
            checkboxItems.Add("instant of event until " + VideoDurationS + "s after the event", VideoRecordingMode.PostEvent);

            RecordingMode = recordingMode;
            Init();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            switch (RecordingMode)
            {
                case VideoRecordingMode.MidEvent:
                    cbxTimeOffset.SelectedIndex = 0;
                    break;
                case VideoRecordingMode.PreEvent:
                    cbxTimeOffset.SelectedIndex = 1;
                    break;
                case VideoRecordingMode.PostEvent:
                    cbxTimeOffset.SelectedIndex = 2;
                    break;
            }
        }

        /// <summary>
        /// Hide the old Init() method. It should get called during construction, though. 
        /// </summary>
        public void Init()
        {
            cbxTimeOffset = new ComboBox();
            slider_panel.Children.Remove(slider_capture_offset);

            cbxTimeOffset.ItemsSource = checkboxItems;
            cbxTimeOffset.SelectedValuePath = "Value";
            cbxTimeOffset.DisplayMemberPath = "Key";
            cbxTimeOffset.Width = 300;
            cbxTimeOffset.SelectedIndex = 0;
            slider_panel.Children.Add(cbxTimeOffset);
            cbxTimeOffset.SelectionChanged += cbx_time_offset_SelectionChanged;

            lbl_slider_title.Content = "Select the time frame of the " + VideoDurationS + "s video recording:";
            lbl_img_save.Content = "Click the button below to choose the location of saved videos:";
            img_save_button.Content = "Browse";

            compress = new CheckBox();
            compress.Content = "Compress Video";
            compress.Margin = new System.Windows.Thickness(5, 0, 0, 0);

            slider_panel.Children.Add(compress);
        }

        private void cbx_time_offset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordingMode = (VideoRecordingMode)cbxTimeOffset.SelectedValue;
        }

        public override string Title
        {
            get { return "Video"; }
        }
    }
}
