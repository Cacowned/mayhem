using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace MayhemApp
{
    /**<summary>
      This class represents the connection between Action and Trigger
    </summary> **/
    [Serializable]
    public class MayhemConnection : ISerializable
    {
        // this global list of connections keeps track of all created or deleted connections
        // the connection states of the connections in this list are serialized when the application terminates
        public static List<MayhemConnection> ALL_CONNECTIONS = new List<MayhemConnection>();


        private const string TAG = "[MayhemConnection] : ";
        public MayhemTrigger trigger {get; set;}
        public MayhemAction action { get; set; }
        public RunControl runControl {get; set;}
        public bool ConnectionEnabled = false;

        public static BitmapImage triggerImg = new BitmapImage(new Uri("Images/bluebutton.png", UriKind.Relative)); //(BitmapImage) App.Current.TryFindResource("bluebutton");
        public static BitmapImage actionImg = new BitmapImage(new Uri("Images/redbutton.png", UriKind.Relative));


        public MayhemConnection()
        {
            Debug.WriteLine(TAG + "Added a new Mayhem Connection!!!!!!!!!!");
            MayhemConnection.ALL_CONNECTIONS.Add(this);
        }

        public MayhemConnection(MayhemTrigger t, MayhemAction a ) : this()
        {

            trigger = t;
            t.onTriggerActivated += this.trigger_activated;
            t.associatedConnection = this;
            action = a;
            a.associatedConnection = this;
            ConnectionEnabled = false;

            runControl = new RunControl(this, t, a);
            runControl.OnTrashButtonClicked += new RunControl.TrashButtonClickHandler(runControl_OnTrashButtonClicked);

        }

        /**<summary>
         * Removes this connection from the global list of connections
         * Also, switches it off
         * </summary>
         */ 
        public void runControl_OnTrashButtonClicked(object sender, EventArgs e)
        {

            //throw new NotImplementedException();
            MayhemConnection.ALL_CONNECTIONS.Remove(this);
            this.DisableConnection();
            
        }

        /**<summary>
         * This methods gets called when a trigger is activated.
         * It notifies the action to fire and also also starts any associated eye-candy.
         * </summary>
         * */
        private void trigger_activated(object sender, EventArgs e)
        {
            Debug.WriteLine(TAG + "trigger_activated");

            // animate runcontrol to show that action has fired

            if (runControl != null)
            {

                // This needs to be started via the control's dispatcher, as triggers can be activated from other threads
                runControl.onOffCtrl.Dispatcher.BeginInvoke(
                    new Action
                        (
                            delegate()
                            {
                                runControl.onOffCtrl.RunTriggerFiredAnimation();
                            }
                        )
                   , null);
            }
            // start the associated action

            if (ConnectionEnabled)
                action.PerformAction( (MayhemTrigger) sender);
        }

        /**
         * <summary>
         * Gets called when the on/off switch changes to ON
         * </summary>
         **/
        public void EnableConnection()
        {
            ConnectionEnabled = true;
            Debug.WriteLine(TAG + "EnableConnection");
            trigger.EnableTrigger();
        }

        /**
         * <summary>
         * Gets called when the on/off switch changes to OFF
         * </summary>
         **/
        public void DisableConnection()
        {
            ConnectionEnabled = false;
            Debug.WriteLine(TAG + "DisableConnection");
            trigger.DisableTrigger();
        }


        public MayhemConnection(SerializationInfo info, StreamingContext context) : this()
        {
            Debug.WriteLine(TAG + "(SerializationInfo info, StreamingContext context)");
            Debug.WriteLine("=========================================");
            Debug.WriteLine("Object Types " + info.GetValue("Trigger", typeof(object)).GetType());

            this.trigger =  info.GetValue("Trigger", typeof(object)) as MayhemTrigger;
            this.action =   info.GetValue("Action", typeof(object)) as MayhemAction;
        
            this.ConnectionEnabled = info.GetBoolean("ConnectionEnabled");

            // bind the action and trigger
            this.trigger.onTriggerActivated += this.trigger_activated;
            this.trigger.associatedConnection = this;
            this.action.associatedConnection = this;

            // build the associated runcontrol
            // runControl = new RunControl(this, trigger, action);
            // runControl.OnTrashButtonClicked += new RunControl.TrashButtonClickHandler(runControl_OnTrashButtonClicked);



        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           // throw new NotImplementedException();

            info.AddValue("Trigger", this.trigger);
            info.AddValue("Action", this.action);
            info.AddValue("ConnectionEnabled", this.ConnectionEnabled);

        }
    }

    /**<summary>
     *  Common properties of Mayhem events and actions
     * </summary>*/
    public interface IMayhemConnectionItemCommon
    {
        void OnDoubleClick(object sender, MouseEventArgs e);
    }


    /**<summary>
     * A connection item is the base class for the entities representing a connectio between trigger and action
     *  (i.e.) MayhemTrigger, MayhemAction
     * </summary>
     * */
    [Serializable]
    public  class MayhemConnectionItem : ISerializable, IMayhemConnectionItemCommon
    {
       

        //public delegate void Fire(object sender, EventArgs e);
        public MayhemConnection associatedConnection {get; set;}
        public string description;
        public string helpText = "A longer help text";
        public string subTitle = "My subtitle";

        // data template data 
        public MayhemButton template_data;

        public MayhemConnectionItem() { }
        public MayhemConnectionItem(string text) { }

        // a connection Item usually (but not always) has a configuration Window

        public Window setup_window = null;

        

        
        public virtual void OnDoubleClick(object sender, MouseEventArgs e) 
        {
            Debug.WriteLine("[MayhemConnectionItem] OnDoubleClick");
        }

       

        public MayhemConnectionItem(SerializationInfo info, StreamingContext context)
        {
            this.description = info.GetString("Description");
            this.subTitle = info.GetString("SubTitle");
            this.helpText = info.GetString("HelpText");
            this.associatedConnection = (MayhemConnection) info.GetValue("AssociatedConnection", typeof(MayhemConnection));
            // UserControl not set!!!
        }

        protected void DimMainWindow(bool dim)
        {
            WindowCollection wc = Application.Current.Windows;
            Debug.WriteLine("Number of current Windows: " + wc.Count);

            MainWindow mainW = null;

            foreach (Window w in wc)
            {
                Debug.WriteLine("Name? " + w.Name);

                if (w.Name == "TheMainWindow")
                {
                    mainW = w as MainWindow;
                }
            }

            if (mainW != null)
            {
                if (dim)
                {
                    Panel.SetZIndex(mainW.DimRectangle, 99);
                    mainW.DimRectangle.Visibility = Visibility.Visible;
                }
                else
                {
                    Panel.SetZIndex(mainW.DimRectangle, 0);
                    mainW.DimRectangle.Visibility = Visibility.Collapsed;
                }
            }

        }

        /** <summary>
         *  Packaged values for serialzation for all MayhemConnectionItem(s) are stored here
         *  Warning:
         *   -- override with "new" keyword
         *   -- be sure to call the base getObjectData
         * </summary>
         * */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();
            info.AddValue("Description", description);
            info.AddValue("AssociatedConnection", associatedConnection);
            info.AddValue("SubTitle", subTitle);
            info.AddValue("HelpText", helpText);      
        }
    }

    public interface IMayhemTriggerCommon
    {
       // void OnDoubleClick(object sender, MouseEventArgs e);
        void EnableTrigger();
        void DisableTrigger();
    }

    [Serializable]
    public class MayhemTrigger : MayhemConnectionItem, ISerializable, IMayhemTriggerCommon, IMayhemConnectionItemCommon
    {
        /*
         *  This class is the trigger component of a MayhemConnection
         * 
         * */

        //private static string 

        public delegate void triggerActivateHandler(object sender, EventArgs e);
        public virtual event triggerActivateHandler onTriggerActivated;
        public bool triggerEnabled = false;


        public MayhemTrigger(string text, string sTitle, string hTxt)
        {
            description = text;
            subTitle = sTitle;
            helpText = hTxt;
            BitmapImage redImg = new BitmapImage(new Uri("../Images/redbutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/button-glow.png", UriKind.Relative)); //"/MayhemApp;component/Images/twitter-official.png"
            template_data = new MayhemButton(description, this.subTitle, this.helpText, redImg, glowImg, MayhemButton.MayhemButtonType.TRIGGER);
            template_data.connectionItem = this;
            template_data.OnDoubleClick += new MayhemButton.DoubleClickHandler(this.OnDoubleClick);
        }


        

        /** <summary>Implement this method to enable the trigger.</summary>
         * 
         **/
        public virtual void EnableTrigger()
        {
            triggerEnabled = true;

        }


        /** <summary>Implement this method do disable the trigger.</summary>
        **/
        public virtual void DisableTrigger()
        {
            triggerEnabled = false;
        }


        public virtual new  void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[MayhemTrigger] OnDoubleClick");
        }

        #region ISerializable for MayhemTrigger
        public MayhemTrigger(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            this.onTriggerActivated = (triggerActivateHandler)info.GetValue("TriggerActivateHandler", typeof(triggerActivateHandler));
            this.triggerEnabled = info.GetBoolean("TriggerEnabled");
            BitmapImage redImg = new BitmapImage(new Uri("../Images/redbutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/button-glow.png", UriKind.Relative));
            this.template_data = new MayhemButton(this.description,  this.subTitle, this.helpText, redImg, glowImg, MayhemButton.MayhemButtonType.TRIGGER);
            template_data.connectionItem = this;
            template_data.OnDoubleClick += new MayhemButton.DoubleClickHandler(this.OnDoubleClick);


        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            base.GetObjectData(info, context);
            info.AddValue("TriggerActivateHandler", onTriggerActivated);
            info.AddValue("TriggerEnabled", triggerEnabled);
        }
        #endregion 
    }

    /**<summary>
     * Action component of MayhemConnection
     * </summary>
     */
    [Serializable]
    public class MayhemAction : MayhemConnectionItem, ISerializable, IMayhemConnectionItemCommon
    {
     
        protected int ID = 0;

        public MayhemAction(string text, string sTitle, string hTxt)
        {
            description = text;
            subTitle = sTitle;
            helpText = hTxt;
            BitmapImage blueImg = new BitmapImage(new Uri("../Images/bluebutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/action-glow.png", UriKind.Relative)); 
            template_data = new MayhemButton(description, this.subTitle, this.helpText, blueImg, glowImg, MayhemButton.MayhemButtonType.ACTION);
            template_data.connectionItem = this;
            template_data.OnDoubleClick += new MayhemButton.DoubleClickHandler(this.OnDoubleClick);
           
        }

        /**<summary>
         *  performs the "business end" of Mayhem Action
         * </summary>
         */
        public virtual void PerformAction(MayhemTrigger sender)
        {
           
        }

        public virtual new void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[MayhemAction] OnDoubleClick");
        }

        public MayhemAction(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            BitmapImage blueImg = new BitmapImage(new Uri("../Images/bluebutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/action-glow.png", UriKind.Relative)); 
            this.template_data = new MayhemButton(this.description, blueImg, glowImg, MayhemButton.MayhemButtonType.ACTION);
            template_data.connectionItem = this;
            template_data.OnDoubleClick += new MayhemButton.DoubleClickHandler(this.OnDoubleClick);

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            //throw new NotImplementedException();
        }
    }
}
