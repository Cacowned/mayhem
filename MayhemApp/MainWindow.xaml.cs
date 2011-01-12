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
using System.Collections.ObjectModel;
using System.Diagnostics;
using MayhemApp.Business_Logic;
using MayhemApp.Business_Logic.Triggers;
using System.Runtime.Serialization;
using MayhemApp.Business_Logic.Actions;
using MayhemOpenCVWrapper;
using MayhemApp.Low_Level;


using System.Windows.Forms;
using System.Threading;
using MayhemApp.Widgets;

// using Twitterizer;

//using oAuthExample;


namespace MayhemApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly string TAG = "[MainWindow] : ";

        ObservableCollection<object> actionList_ = new ObservableCollection<object>();
      
        ObservableCollection<object> triggerList_ = new ObservableCollection<object>();
       
        ObservableCollection<object> runList_ = new ObservableCollection<object>();

        ObservableCollection<object> actionBuilder_ = new ObservableCollection<object>();
        ObservableCollection<object> triggerBuilder_ = new ObservableCollection<object>();



        ///////////////////////////////////////////////// category list controller


        MayhemInterfaceCategoryListController trigger_categories = null;
        MayhemInterfaceCategoryListController action_categories = null;

        /////////////////////////////////////////////



        
        // empty fields for create mahyem bar

        private MayhemButton dragTriggerButton;
        private MayhemButton dragActionButton;

        private MayhemInterfacePaginationController actionListController = new MayhemInterfacePaginationController();
        private MayhemInterfacePaginationController triggerListController = new MayhemInterfacePaginationController();
        private MayhemInterfacePaginationController runListController = new MayhemInterfacePaginationController();



        // Configuration Screen

        private Dialogs.MainConfigDialog mainConfigWindow = new Dialogs.MainConfigDialog();


        public MainWindow()
        {
            InitializeComponent();
            InterceptKeys.Instantiate();


            #region testing

            ///////// MayhemImageUpdater Test

            MayhemImageUpdater.Instance.EnumerateDevices();
            MayhemImageUpdater.Instance.InitCaptureDevice(0, 320, 240);

            ////////////////





            ////////// Media Player Tests

            //MPlayer m = new MPlayer();

           // m.TestPlayFile();
            // MPlayer.instance.TestPlayFile();

         



            ////////////





            // key interceptor, can be used by triggers
            

            /////// Test of InterceptKeys

            //InterceptKeys.OnInterceptKeyDown += new InterceptKeys.KeyDownHandler(InterceptKeys_OnInterceptKeyDown);
            //InterceptKeys.OnInterceptKeyUp += new InterceptKeys.KeyUpHandler(InterceptKeys_OnInterceptKeyUp);

            //////////

            //HookManager.KeyDown += new System.Windows.Forms.KeyEventHandler(HookManager_KeyDown); 

            // HookManager.KeyPress += new KeyPressEventHandler(HookManager_KeyPress);

            #endregion

            


     

            runListController.displayItems = runList_;

            runListController.pageLabel = runListLabel;
        

            DragDropHelper.ItemDropped += new EventHandler<DragDropEventArgs>(DragDropHelper_ItemDropped);


            this.configButton.OnButtonClick += new ConfigButton.ButtonClickHandler(configButton_OnButtonClick);


            // add configuration items to config window
            mainConfigWindow.configItemsBox.Items.Add(new CameraConfigItem());
            mainConfigWindow.configItemsBox.Items.Add(new TwitterConfigItem());

            #region action / trigger lists
            trigger_categories = new MayhemInterfaceCategoryListController(triggerList, t_listNavigationControl,  "Trigger Types");
            action_categories = new MayhemInterfaceCategoryListController(actionList, a_listNavigationControl, "Action Types");

            actionList.ItemsSource = action_categories.current_displayItems;
            actionList.DataContext = action_categories;


            triggerList.ItemsSource = trigger_categories.current_displayItems;
            triggerList.DataContext = trigger_categories;
            #endregion

            actionBuilder.ItemsSource = actionBuilder_;
            triggerBuilder.ItemsSource = triggerBuilder_;

            actionBuilder.IsEnabled = true;
            triggerBuilder.IsEnabled = true;

            // set the version label

            Mayhem_Version_Label.Content = "Mayhem Build " + this.GetType().Assembly.GetName().Version.ToString();

        }

        void InterceptKeys_OnInterceptKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(e.KeyCode);
        }

        void InterceptKeys_OnInterceptKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(e.KeyCode);
        }

        
       
        /**<summary>
         * Configuration Button Clicked
         * </summary>
         */ 
        void configButton_OnButtonClick(object sender, EventArgs e)
        {
            Debug.WriteLine("[MainWindow.xaml] : configuButton clicked");

            mainConfigWindow.Show();

        }

        /**<summary>
         * Implementation of Drag&Drop logic in main window. 
         * </summary>
         * */
         void DragDropHelper_ItemDropped(object sender, DragDropEventArgs e)
        {
            Debug.WriteLine("[MainWindow] : DragDropHelper_ItemDropped"); 

            MayhemButton p = e.Content as MayhemButton;
            if (p == null) return;


            if (action_categories.ContainsItem(p) && !actionBuilder_.Contains(p))
            {
                actionBuilder.IsEnabled = true;
                //actionBuilder_.Remove(dragActionButton);
                actionBuilder_.Add(p);
            }

            else if (trigger_categories.ContainsItem(p) && !triggerBuilder_.Contains(p))
            {
                triggerBuilder.IsEnabled = true;
                //triggerBuilder_.Remove(dragTriggerButton);
                triggerBuilder_.Add(p);
            }

            else if (actionBuilder_.Contains(p))
            {
                // remove from action builder
                actionBuilder_.Remove(p);
            }
            else if (triggerBuilder_.Contains(p))
            {
                // remove from action builder
                triggerBuilder_.Remove(p);
            }
             // do the really interesting stuff when we both slots are filled


             // check if we do have a connection

            if (
                triggerBuilder_.Count > 1 &&  ((MayhemButton) triggerBuilder_[1]).buttonType == MayhemButton.MayhemButtonType.TRIGGER &&
                actionBuilder_.Count > 1 &&  ((MayhemButton) actionBuilder_[1]).buttonType == MayhemButton.MayhemButtonType.ACTION )             
            {
                Debug.WriteLine(TAG + "two connection items in connection builder");

                // TODO BUILD CONNECTION CODE <--> storyboards: shrink the action/trigger buttons, re-expand the drag buttons

                

                MayhemButton t = (MayhemButton) triggerBuilder_[1];
                MayhemButton a = (MayhemButton) actionBuilder_[1];


                // TODO PLAY SHRINK ANIMATION

              






                




                // build a connection

                if (t.connectionItem != null && a.connectionItem != null)
                {
                    MayhemConnection c = new MayhemConnection((MayhemTrigger) t.connectionItem,
                                                              (MayhemAction) a.connectionItem);
                    RunControl r = c.runControl;
                    runListController.AddItem(r);
                }


                /// remove items from trigger/action builder
                /// 

                triggerBuilder_.Remove(t);
                actionBuilder_.Remove(a);

                /// clone the live item in the categories list
                /// 

                Type triggerType = t.connectionItem.GetType();
                var new_trigger = Activator.CreateInstance(triggerType, new object[] { t.connectionItem.description });

                Type actionType = a.connectionItem.GetType();
                var new_action = Activator.CreateInstance(actionType, new object[] { a.connectionItem.description });

                if (new_trigger is MayhemTrigger)
                {
                    trigger_categories.ReplaceItem(t, new_trigger);
                }

                if (new_action is MayhemAction)
                {
                    action_categories.ReplaceItem(a, new_action);
                }
                                      
            }


        }

         private void Window_Loaded(object sender, RoutedEventArgs e)
         {
             Debug.WriteLine("WINDOW LOADED");

             runListController.userControl = runList;

             //////// Uncomment to delete settings
             //Properties.Settings.Default.RunlistSettings = string.Empty;
             //Properties.Settings.Default.Save();
             /////////////////////////

             LoadButtons();


     
         }

         private void LoadButtons()
         {
                                                      
             BitmapImage actionImg = new BitmapImage(new Uri("Images/bluebutton.png", UriKind.Relative)); //(BitmapImage) App.Current.TryFindResource("bluebutton");
             BitmapImage triggerImg = new BitmapImage(new Uri("Images/redbutton.png", UriKind.Relative));

             BitmapImage dragTriggerImg = new BitmapImage(new Uri("../Images/trigger-empty.png", UriKind.Relative));
             BitmapImage dragActionImg = new BitmapImage(new Uri("../Images/action-empty.png", UriKind.Relative));


             /// Create top level Categories ////////////////////

             LibraryListItem time_category = new LibraryListItem("Time Triggers", triggerImg, null);
             LibraryListItem webcam_category = new LibraryListItem("Webcam Triggers", triggerImg, null);
             LibraryListItem input_events_category = new LibraryListItem("Input Triggers", triggerImg, null);
             LibraryListItem network_events_category = new LibraryListItem("Serial/Network Port Triggers", triggerImg, null);

             trigger_categories.AddDisplayItem(time_category);
             trigger_categories.AddDisplayItem(webcam_category);
             trigger_categories.AddDisplayItem(input_events_category);
             trigger_categories.AddDisplayItem(network_events_category);

             LibraryListItem debug_category = new LibraryListItem("Debug Actions", actionImg, null);
             LibraryListItem record_category = new LibraryListItem("Recording Actions", actionImg, null);
             LibraryListItem playback_category = new LibraryListItem("Playback Actions", actionImg, null);
             LibraryListItem social_category = new LibraryListItem("Social Network Actions", actionImg, null);

             action_categories.AddDisplayItem(debug_category);
             action_categories.AddDisplayItem(record_category);
             action_categories.AddDisplayItem(playback_category);
             action_categories.AddDisplayItem(social_category);

             ////////////////////////////////////////////////////

          

             /// Add subpages /////////////////////////////////////////////

       
             trigger_categories.AddSubPageToItem(time_category,time_category.Label);
             trigger_categories.AddSubPageToItem(webcam_category, webcam_category.Label);
             trigger_categories.AddSubPageToItem(input_events_category, input_events_category.Label);
             trigger_categories.AddSubPageToItem(network_events_category, network_events_category.Label);

             action_categories.AddSubPageToItem(debug_category, time_category.Label);
             action_categories.AddSubPageToItem(record_category, webcam_category.Label);
             action_categories.AddSubPageToItem(playback_category, input_events_category.Label);
             action_categories.AddSubPageToItem(social_category, social_category.Label);
            // action_categories.AddSubPageToItem(
                       
             //////////////////////////////
             

             /////////////////// Populate list with triggers ///////////////////////////////
             //
             //
              MayhemTimerTrigger timerTrig = new MayhemTimerTrigger();
              trigger_categories.current_page.subPages[time_category].Add_Item(timerTrig.template_data);

              MayhemMotionDetectionTrigger motionDetectionTrigger = new MayhemMotionDetectionTrigger();
              trigger_categories.current_page.subPages[webcam_category].Add_Item(motionDetectionTrigger.template_data);

              MayhemKeyboardEventTrigger keyboardEventTrigger = new MayhemKeyboardEventTrigger();
              trigger_categories.current_page.subPages[input_events_category].Add_Item(keyboardEventTrigger.template_data);

              MayhemUDPTrigger UDPTrigger = new MayhemUDPTrigger();
              trigger_categories.current_page.subPages[network_events_category].Add_Item(UDPTrigger.template_data);

             //////////////// Populate with Actions
             //
             //

              MayhemDebugPopupAction popupAction = new MayhemDebugPopupAction();
              action_categories.current_page.subPages[debug_category].Add_Item(popupAction.template_data);

              MayhemDebugAction debugAction = new MayhemDebugAction();
              action_categories.current_page.subPages[debug_category].Add_Item(debugAction.template_data);

              MayhemPlaySoundAction playSoundA = new MayhemPlaySoundAction();
              action_categories.current_page.subPages[playback_category].Add_Item(playSoundA.template_data);

              MayhemSnapshotAction snapshotA = new MayhemSnapshotAction();
              action_categories.current_page.subPages[record_category].Add_Item(snapshotA.template_data);

              MayhemTweetAction tweetAction = new MayhemTweetAction();
              action_categories.current_page.subPages[social_category].Add_Item(tweetAction.template_data);

             //////////////////////////////////////////////////////////////////////////////////





            // MayhemTrigger t2 = new MayhemTrigger("*Outside Temp");
            // triggerListController.AddItem(t2.userControl);

            // MayhemTrigger t3 = new MayhemTrigger("*Twitter Activity");
            // triggerListController.AddItem(t3.userControl);

            // MayhemTrigger t4 = new MayhemTrigger("*Inbox Size");
            // triggerListController.AddItem(t4.userControl);

            // MayhemTrigger t5 = new MayhemTrigger("*Face Detected");
            // triggerListController.AddItem(t5.userControl);

             

            // //some demo actions

            // MayhemAction a = new MayhemAction("*Take Snapshot");
            // actionListController.AddItem(a.userControl);

            // MayhemAction a2 = new MayhemAction("*Run Python Script");
            // actionListController.AddItem(a2.userControl);

            ///* MayhemAction a3 = new MayhemAction("*Play Sound");
            // actionListController.AddItem(a3.userControl); */

            // MayhemAction a4 = new MayhemAction("*Send Text");
            // actionListController.AddItem(a4.userControl);

            //// MayhemAction a5 = new MayhemAction("*Post Tweet");
            //// actionListController.AddItem(a5.userControl);

            // MayhemAction snapshot = new MayhemSnapshotAction();
            // actionListController.AddItem(snapshot.userControl);

            

            // // add keyboard event trigger

            // MayhemUDPTrigger UDPTrigger = new MayhemUDPTrigger();
            // triggerListController.AddItem(UDPTrigger.userControl);
         

            // MayhemKeyboardEventTrigger keyboardTrigger = new MayhemKeyboardEventTrigger();
            // triggerListController.AddItem(keyboardTrigger.userControl);

            // // add timer trigger

            // MayhemTimerTrigger timerTrig = new MayhemTimerTrigger();
            // triggerListController.AddItem(timerTrig.userControl);


            // // add motion trigger
            // MayhemMotionDetectionTrigger motionTrig = new MayhemMotionDetectionTrigger();
            // triggerListController.AddItem(motionTrig.userControl);

            // // add a sample action
            // MayhemDebugAction debugAction = new MayhemDebugAction();
            // actionListController.AddItem(debugAction.userControl);

             

            // // another sample action
            // MayhemDebugPopupAction popupAction = new MayhemDebugPopupAction();
            // actionListController.AddItem(popupAction.userControl);

            // // add twitter action

            // MayhemTweetAction tweetAction = new MayhemTweetAction();
            // actionListController.AddItem(tweetAction.userControl);


            // MayhemPlaySoundAction playSoundAction = new MayhemPlaySoundAction();
            // actionListController.AddItem(playSoundAction.userControl);

           
             

            


             actionBuilder.ItemsSource = actionBuilder_;
             triggerBuilder.ItemsSource = triggerBuilder_;

            // // add the bitmaps for drag/drop action
           dragTriggerImg = new BitmapImage(new Uri("../Images/trigger-empty.png", UriKind.Relative));
             dragActionImg = new BitmapImage(new Uri("../Images/action-empty.png", UriKind.Relative));

            dragTriggerButton = new MayhemButtonPlaceHolder("", dragTriggerImg);
            triggerBuilder_.Add(dragTriggerButton);
            triggerBuilder.IsEnabled = true;



             dragActionButton = new MayhemButton("", dragActionImg);
             actionBuilder_.Add(dragActionButton);
             actionBuilder.IsEnabled = true;




             #region  Deserialization
            

             // actionBuilder.IsEnabled = false;

             //////// Uncomment to delete settings
             //Properties.Settings.Default.RunlistSettings = string.Empty;
             //Properties.Settings.Default.Save();
             /////////////////////////


             // populate the runlist

             if (Properties.Settings.Default.RunlistSettings.Length > 0)
             {

                 try
                 {
                     List<MayhemConnection> connections = Business_Logic.Base64Serialize<List<MayhemConnection>>.DeserializeFromString(Properties.Settings.Default.RunlistSettings);

                     Debug.WriteLine("Nr. Of Connections: " + connections.Count);

                     foreach (MayhemConnection c in connections)
                     {
                         


                         MayhemButton tri = c.trigger.template_data;
                         MayhemButton act = c.action.template_data;
                         RunControl r = new RunControl(c, tri, act);
                         r.OnTrashButtonClicked += new RunControl.TrashButtonClickHandler(c.runControl_OnTrashButtonClicked);
                         
                         runListController.AddItem(c.runControl);

                     }
                 }
                 catch (SerializationException e)
                 {
                     Debug.WriteLine(Tag + "(De-)SerializationException " + e);
                 }
             }

             #endregion

         }

         


         private void btn_runlist_next_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine("btn_runlist_next_Click");
             runListController.ShowNextPage();

             //runListLabel.Content = runListController.GetPagePositionString();

         }

         private void btn_runlist_prev_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine("btn_runlist_prev_Click");
             runListController.ShowPrevPage();

             //runListLabel.Content = runListController.GetPagePositionString();
         }

         private void btn_actionList_next_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine("btn_actionList_next_Click");
             actionListController.ShowNextPage();
         }

         private void btn_actionList_prev_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine("btn_actionList_prev_Click");
             actionListController.ShowPrevPage();
         }

         private void btn_triggerList_next_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine(" btn_triggerList_next_Click");
             triggerListController.ShowNextPage();
         }

         private void btn_triggerList_prev_Click(object sender, RoutedEventArgs e)
         {
             Trace.WriteLine("btn_triggerList_prev_Click");
             triggerListController.ShowPrevPage();
         }

         private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
         {
             // stop the image updater
             MayhemImageUpdater m = MayhemImageUpdater.Instance;
             m.Stop();

             Debug.WriteLine("Saving runListController to Settings file");
           //  runListController.SaveToSettingsFile();
             string serString = Business_Logic.Base64Serialize<List<MayhemConnection>>.SerializeToString(MayhemConnection.ALL_CONNECTIONS);
             Debug.WriteLine("Serialization String " + serString);
             Properties.Settings.Default.RunlistSettings = serString;
             Properties.Settings.Default.Save();



             Debug.WriteLine("Saved runListController to Settings file");
         }

         private void Window_Closed(object sender, EventArgs e)
         {
            
             // shutdown application
             Debug.WriteLine("[MainWindow] : closed and shutting down application");
             System.Windows.Application.Current.Shutdown(0);
         }

    }
}
