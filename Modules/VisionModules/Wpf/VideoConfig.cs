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
using System;

namespace VisionModules.Wpf
{
    public class VideoConfig : PictureConfig
    {
    
        private CheckBox compress;

        public VIDEO_RECORDING_MODE RecordingMode
        {
            get;
            private set;
        }

        private const int video_duration_s = Camera.kLoopDuration / 1000; 

        /// <summary>
        /// Content of the Checkbox items, for selection of the video recording timing modes. 
        /// </summary>
        private  Dictionary<string, VIDEO_RECORDING_MODE> checkbox_items =
            new Dictionary<string, VIDEO_RECORDING_MODE>()
            {
                {video_duration_s/2+"s before until " + video_duration_s/2 + "s after the event (default)", VIDEO_RECORDING_MODE.MID_EVENT},
                {video_duration_s+"s before the event until the event occurred", VIDEO_RECORDING_MODE.PRE_EVENT},
                {"instant of event until " +video_duration_s+"s after the event", VIDEO_RECORDING_MODE.POST_EVENT}
            };
        private ComboBox cbx_time_offset;

        public bool compress_video
        {
            get 
            {
                    if (compress.IsChecked != null)
                    {
                        return (bool)compress.IsChecked;
                    }
                    else
                    {
                        return false; 
                    }
            }
        }

        public VideoConfig(string location, string prefix, VIDEO_RECORDING_MODE recordingMode, int deviceIdx)
            : base(location, prefix, 0, deviceIdx)
            
        {
            RecordingMode = recordingMode; 
            Init();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            switch (RecordingMode)
            {
                case VIDEO_RECORDING_MODE.MID_EVENT:
                    cbx_time_offset.SelectedIndex = 0;
                    break;
                case VIDEO_RECORDING_MODE.PRE_EVENT:
                    cbx_time_offset.SelectedIndex = 1;
                    break;
                case VIDEO_RECORDING_MODE.POST_EVENT:
                    cbx_time_offset.SelectedIndex = 2;
                    break;
            }
        }

        /// <summary>
        /// Hide the old Init() method. It should get called during construction, though. 
        /// </summary>
        public void Init()
        {
            cbx_time_offset = new ComboBox();
            slider_panel.Children.Remove(slider_capture_offset);
         
            cbx_time_offset.ItemsSource = checkbox_items;
            cbx_time_offset.SelectedValuePath = "Value";
            cbx_time_offset.DisplayMemberPath = "Key";
            cbx_time_offset.Width = 300;
            cbx_time_offset.SelectedIndex = 0;
            slider_panel.Children.Add(cbx_time_offset);
            cbx_time_offset.SelectionChanged += new SelectionChangedEventHandler(cbx_time_offset_SelectionChanged);
            
            lbl_slider_title.Content = "Select the time frame of the "+video_duration_s+"s video recording:";
            lbl_img_save.Content = "Click the button below to choose the location of saved videos:";
            img_save_button.Content = "Browse";
            
            compress = new CheckBox();
            compress.Content = "Compress Video";
            compress.Margin = new System.Windows.Thickness(5, 0, 0, 0);

            slider_panel.Children.Add(compress);
        }

        void cbx_time_offset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordingMode = (VIDEO_RECORDING_MODE) cbx_time_offset.SelectedValue;
        }

        public override string Title
        {
            get { return "Video"; }
        }
    }
}
