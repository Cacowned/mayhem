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
using System.Diagnostics;
using MayhemApp.Business_Logic;

namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for RunControl.xaml
    /// </summary>
    public partial  class RunControl : UserControl
    {

        private const string TAG = "[RunControl] : ";


        public delegate void TrashButtonClickHandler(object sender, EventArgs e);
        public event TrashButtonClickHandler OnTrashButtonClicked;
        

        public MayhemConnection connection { get; set; }

        public RunControl() { }
        
        public RunControl(MayhemConnection c, MayhemTriggerBase t, MayhemActionBase a)  
            : this (c, t.template_data, a.template_data) {}
        
        public RunControl(MayhemConnection c, MayhemButton t,  MayhemButton a)
        {
            InitializeComponent();

            // let the connection and the control know each other
            connection = c;
            c.runControl = this;
           

            // data bindings
            triggerItem.Img.Source = t.Img;
            triggerItem.name.Text = t.Text;
            triggerItem.DataContext = t;

            actionItem.Img.Source = a.Img;
            actionItem.name.Text = a.Text;
            actionItem.DataContext = a;

            // TODO SET THIS After Init
            //Canvas.SetLeft(onOffCtrl.buttonSlider, 43);

            this.onOffCtrl.OnSliderOn += new OnOffControl.SliderMovedEventHandler(onOffCtrl_OnSliderOn);
            this.onOffCtrl.OnSliderOff += new OnOffControl.SliderMovedEventHandler(onOffCtrl_OnSliderOff);
            this.myTrashButton.OnButtonClick += new TrashButton.ButtonClickHandler(myTrashButton_OnButtonClick);

            // set up the slider to display correctly
            if (c.ConnectionEnabled)
            {
                this.onOffCtrl.Set_On(false);
            }
            else
            {
                this.onOffCtrl.Set_Off(false);
            }

            // register the trashbutton event with the connection


        }

        void myTrashButton_OnButtonClick(object sender, EventArgs e)
        {

            Debug.WriteLine(TAG + "myTrashButton_OnButtonClick");


            if (OnTrashButtonClicked != null)
                OnTrashButtonClicked(this, new EventArgs());

         

        }

        void onOffCtrl_OnSliderOff(object sender, EventArgs e)
        {
        
            Debug.WriteLine(TAG + "onOffCtrl_OnSliderOff");

            // notify the mayhem connection

            connection.DisableConnection();

        }

        void onOffCtrl_OnSliderOn(object sender, EventArgs e)
        {
            Debug.WriteLine(TAG + "onOffCtrl_OnSliderON");

            //notify the Mayhem connection
            connection.EnableConnection();


        }

        public void forceOn()
        {
            Canvas.SetLeft(onOffCtrl.buttonSlider, 43);
            this.InvalidateVisual();
        }

        public void forceOff()
        {
            Canvas.SetLeft(onOffCtrl.buttonSlider,7);
            this.InvalidateVisual();
        }




        /*
        public RunControl(MayhemTrigger t, MayhemAction a)
        {
            triggerButton = t.userControl;
            actionButton = a.userControl;
        } */

        /* * Broken ... fix!
        public RunControl(MayhemConnection c)
        {

            triggerItem.Img.Source = c.trigger.userControl.Img;
            triggerItem.name.Text = c.trigger.userControl.Text;
            actionItem.Img.Source = c.action.userControl.Img;
            actionItem.name.Text = c.action.userControl.Text;

            this.onOffCtrl.OnSliderOn += new OnOffControl.SliderMovedEventHandler(onOffCtrl_OnSliderOn);
            this.onOffCtrl.OnSliderOff += new OnOffControl.SliderMovedEventHandler(onOffCtrl_OnSliderOff);
        }*/

        private void action_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateVisual();
        }
    }
}
