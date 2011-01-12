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
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for OnOffControl.xaml
    /// </summary>
    public partial class OnOffControl : UserControl
    {


        private enum sliderButtonState {
            ON,
            OFF
        };

        private sliderButtonState sbState = sliderButtonState.OFF;
 

       

        public delegate void SliderMovedEventHandler(object sender, EventArgs e);
        public event SliderMovedEventHandler OnSliderOff;
        public event SliderMovedEventHandler OnSliderOn;

        public OnOffControl()
        {
            InitializeComponent();
        }

        /**<summary>
         * Sets the control to "ON" position
         * 
         * </summary>
         * */
        public void Set_On(bool fireEvent)
        {
            if (sbState == sliderButtonState.OFF)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation da = new DoubleAnimation(43, new Duration(TimeSpan.FromMilliseconds(500)));
                Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(da);
                buttonSlider.BeginStoryboard(sb);
                Debug.WriteLine("[onOffSlider_mouseLeftUp] animation started");
                sbState = sliderButtonState.ON;

                // TODO activate event when animation has finished, not immediately

                // fire event to listener
                if (OnSliderOn != null)
                    OnSliderOn(this, new EventArgs());
            }
        }

        public  void RunTriggerFiredAnimation()
        {
            Storyboard sb = (Storyboard)FindResource("triggerActivated");
            if (sb != null)
            {
                sb.Begin(ActivatedEffect);
            }
        }

        /**<summary>
         * Sets the control to "OFF" position
         * 
         * </summary>
         * */
        public void Set_Off(bool fireEvent)
        {
            if (sbState == sliderButtonState.ON)
            {
                Storyboard sb = new Storyboard();
                DoubleAnimation da = new DoubleAnimation(7, new Duration(TimeSpan.FromMilliseconds(500)));
                Storyboard.SetTargetProperty(da, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(da);
                buttonSlider.BeginStoryboard(sb);
                Debug.WriteLine("[onOffSlider_mouseLeftUp] animation started");
                sbState = sliderButtonState.OFF;

                if (fireEvent)
                {
                    // fire event to listener
                    if (OnSliderOff != null)
                        OnSliderOff(this, new EventArgs());
                }
            }  
        }



        private void onOffSlider_mouseLeftUp(object sender, MouseButtonEventArgs e)
        {

            if (sbState == sliderButtonState.ON)
            {
                Set_Off(true);            
            }
            else
            {
                Set_On(true);
            }

        }

        private void buttonSlider_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
