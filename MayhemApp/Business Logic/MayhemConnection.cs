﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using MayhemApp.Business_Logic;

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
        public MayhemTriggerBase trigger { get; set; }
        public MayhemActionBase action { get; set; }
        public RunControl runControl { get; set; }
        public bool ConnectionEnabled = false;

        public static BitmapImage triggerImg = new BitmapImage(new Uri("Images/bluebutton.png", UriKind.Relative)); //(BitmapImage) App.Current.TryFindResource("bluebutton");
        public static BitmapImage actionImg = new BitmapImage(new Uri("Images/redbutton.png", UriKind.Relative));


        public MayhemConnection()
        {
            Debug.WriteLine(TAG + "Added a new Mayhem Connection!!!!!!!!!!");
            MayhemConnection.ALL_CONNECTIONS.Add(this);
        }

        public MayhemConnection(MayhemTriggerBase t, MayhemActionBase a)
            : this()
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
                action.PerformAction((MayhemTriggerBase)sender);
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


        public MayhemConnection(SerializationInfo info, StreamingContext context)
            : this()
        {
            Debug.WriteLine(TAG + "(SerializationInfo info, StreamingContext context)");
            Debug.WriteLine("=========================================");
            Debug.WriteLine("Object Types " + info.GetValue("Trigger", typeof(object)).GetType());

            this.trigger = info.GetValue("Trigger", typeof(object)) as MayhemTriggerBase;
            this.action = info.GetValue("Action", typeof(object)) as MayhemActionBase;

            this.ConnectionEnabled = info.GetBoolean("ConnectionEnabled");

            // bind the action and trigger
            this.trigger.onTriggerActivated += this.trigger_activated;
            this.trigger.associatedConnection = this;
            this.action.associatedConnection = this;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Trigger", this.trigger);
            info.AddValue("Action", this.action);
            info.AddValue("ConnectionEnabled", this.ConnectionEnabled);

        }
    }
}
