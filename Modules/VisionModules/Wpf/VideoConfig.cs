/*
 * 
 * VideoConfig.cs
 * 
 * Config for the Video Reaction
 * 
 * Derives from PictureConfig.cs, so check out the xaml there 
 * 
 * 
 */
using System.Collections.Generic;
using System.Windows.Controls;
using MayhemOpenCVWrapper;

namespace VisionModules.Wpf
{
    public class VideoConfig : PictureConfig
    {

       
        private enum VIDEO_RECORDING_MODE
        {
            PRE_EVENT = 0,                                        // record 30s prior to the event
            POST_EVENT = Camera.LOOP_DURATION / 1000,             // record 30s after event
            MID_EVENT = (Camera.LOOP_DURATION / 1000) / 2         // record 15s before and 15s after the event
        }

        private CheckBox compress;

        private const int video_duration_s = Camera.LOOP_DURATION / 1000; 

        private  Dictionary<string, VIDEO_RECORDING_MODE> checkbox_items =
            new Dictionary<string, VIDEO_RECORDING_MODE>()
            {
                {video_duration_s/2+"s before until " + video_duration_s/2 + "s after the event (default)", VIDEO_RECORDING_MODE.MID_EVENT},
                {video_duration_s+"s before the event until the event occurred", VIDEO_RECORDING_MODE.PRE_EVENT},
                {"instant of event until " +video_duration_s+"s after the event", VIDEO_RECORDING_MODE.POST_EVENT}
            };
        private ComboBox cbx_time_offset;


        public int temporal_offset
        {
            get
            {
                return (int) cbx_time_offset.SelectedValue;
            }
        }

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

        public  VideoConfig(string location, string prefix,  double capture_offset_time, int deviceIdx) : base(location, prefix,  capture_offset_time, deviceIdx)
            : base(location, prefix, capture_offset_time)
        {      
            Init();
        }

        /// <summary>
        /// Hide the old Init() method. It should get called during construction, though. 
        /// </summary>
        public new void Init()
        {
            cbx_time_offset = new ComboBox();

            slider_panel.Children.Remove(slider_capture_offset);

            

            cbx_time_offset.ItemsSource = checkbox_items;
            cbx_time_offset.SelectedValuePath = "Value";
            cbx_time_offset.DisplayMemberPath = "Key";
            cbx_time_offset.Width = 300;
            cbx_time_offset.SelectedIndex = 0;
            slider_panel.Children.Add(cbx_time_offset);
            
            /**** old code using the slider (may be revived at some point) 
            slider_capture_offset.Minimum = 0;
            slider_capture_offset.Maximum = max_duration;
            // changes to the duration selection
            slider_capture_offset.SelectionStart = 0;
            slider_capture_offset.SelectionEnd = max_duration;*/

            lbl_slider_title.Content = "Select the time frame of the "+video_duration_s+"s video recording:";
            lbl_img_save.Content = "Click the button below to choose the location of saved videos:";
            img_save_button.Content = "Video Save Location";
            
            compress = new CheckBox();
            compress.Content = "Compress Video";
            compress.Margin = new System.Windows.Thickness(5, 0, 0, 0);

            slider_panel.Children.Add(compress);
        }

        public override string Title
        {
            get { return "Video"; }
        }

    }
}
