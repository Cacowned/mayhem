using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Input;
using MayhemApp.Dialogs;
using MayhemApp.Low_Level;
using System.Diagnostics;

namespace MayhemApp.Business_Logic.Triggers
{
    [Serializable]
    class MayhemKeyboardEventTrigger : MayhemTrigger, IMayhemTriggerCommon, IMayhemConnectionItemCommon, ISerializable
    {

         public const string TAG = "[MayhemKeyboardEventTrigger]";

         private HashSet<System.Windows.Forms.Keys> monitor_keys_down = null;

         private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
         private InterceptKeys.KeyDownHandler keyDownHandler = null;
         private InterceptKeys.KeyUpHandler keyUpHandler = null;


         public override event triggerActivateHandler onTriggerActivated;

         #region Constructors

         public MayhemKeyboardEventTrigger(string s)
            : this() { }

         public MayhemKeyboardEventTrigger()
             : base("Keyboard Event",
                    "This trigger fires with a predefined keyboard event",
                    "Define a keyboard event to make this trigger fire. This trigger also functions when Mayhem is running in the background. Double click to configure.")
         {
             setup_window = new KeyboardEventTriggerSetupWindow();

             ((KeyboardEventTriggerSetupWindow)setup_window).OnKeyCombinationUpdated += new KeyboardEventTriggerSetupWindow.KeyCombinationUpdatedHandler(MayhemKeyboardEventTrigger_OnKeyCombinationUpdated);


             keyDownHandler = new InterceptKeys.KeyDownHandler(Intercept_key_down);
             keyUpHandler = new InterceptKeys.KeyUpHandler(Intercept_key_up);

         
         }

        #endregion

         #region business logic
         private void Intercept_key_down(object sender, System.Windows.Forms.KeyEventArgs e)
         {
             Debug.WriteLine(TAG + " dn " + e.KeyCode);

             keys_down.Add(e.KeyCode);

             if (Keysets_Equal())
             {
                 Debug.WriteLine(TAG + "key sets match, will fire the trigger if activated");
                 if (this.triggerEnabled)
                 {
                     Debug.WriteLine(TAG + "trigger enabled, testing fire condition");
                     if (onTriggerActivated != null)
                     {
                         onTriggerActivated(this, new EventArgs());
                     }
                 }

             }

         }

         private bool Keysets_Equal()
         {
             Debug.WriteLine(TAG + "Keyset Lengths " + monitor_keys_down.Count + " " + keys_down.Count);

             if (keys_down.Count == monitor_keys_down.Count)
             {
                 foreach (System.Windows.Forms.Keys k in monitor_keys_down)
                 {
                     bool foundEqiv = false;
                     foreach (System.Windows.Forms.Keys l in keys_down)
                     {
                         if (l == k)
                         { foundEqiv = true; break; }
                     }

                     if (foundEqiv == false)
                         return false;
                 }
                 return true;
             }
             else
             {
                 return false;
             }
         }



         private void Intercept_key_up(object sender, System.Windows.Forms.KeyEventArgs e)
         {
             Debug.WriteLine(TAG + " up " + e.KeyCode);

             keys_down.Remove(e.KeyCode);

         }


         void MayhemKeyboardEventTrigger_OnKeyCombinationUpdated(object sender, KeyboardEventTriggerSetupWindow.KeyCombinationUpdatedArgs e)
         {
            // throw new NotImplementedException();

             monitor_keys_down = e.key_combination;

         }

        #endregion

         #region MayhemTrigger Methods

         public override void EnableTrigger()
         {
             Debug.WriteLine(TAG + "EnableTrigger()");
             base.EnableTrigger();

             this.triggerEnabled = true;
             InterceptKeys.OnInterceptKeyDown += keyDownHandler;
             InterceptKeys.OnInterceptKeyUp += keyUpHandler;
         }

         public override void DisableTrigger()
         {
             Debug.WriteLine(TAG + "DisableTrigger()");
             base.DisableTrigger();

             this.triggerEnabled = false;
             InterceptKeys.OnInterceptKeyDown -= keyDownHandler;
             InterceptKeys.OnInterceptKeyUp -= keyUpHandler;
         }

         public override void OnDoubleClick(object sender, MouseEventArgs e)
         {
             DimMainWindow(true);
             setup_window.ShowDialog();
             DimMainWindow(false);
         }

        #endregion

        #region Serialization


         /** <summary>
          *  De-Serialization constructor
          * </summary> 
          * */
         public MayhemKeyboardEventTrigger(SerializationInfo info, StreamingContext context)
            : base(info, context)

            {
                setup_window = new KeyboardEventTriggerSetupWindow();

                


                monitor_keys_down = new HashSet<System.Windows.Forms.Keys>();

                string[] key_strings = null;
                try
                {
                    key_strings = info.GetValue("monitor_keys", typeof(object)) as string[];
                }
                catch (SerializationException ex)
                {
                    Debug.WriteLine(TAG + "Serialization Exception !!!!!\n" + ex);
                }

                if (key_strings == null)
                {
                    Debug.WriteLine(TAG + "monitor keys is either null or deserialization is broken!!");
                }
                else
                {
                    // disable key scanning of the window
                    ((KeyboardEventTriggerSetupWindow)setup_window).scanning_keys = false;

                    for (int i = 0; i < key_strings.Length; i++)
                    {
                        System.Windows.Forms.Keys k = (System.Windows.Forms.Keys) Enum.Parse(typeof(System.Windows.Forms.Keys), key_strings[i]);
                        monitor_keys_down.Add(k);
                    }

                    foreach (System.Windows.Forms.Keys k in monitor_keys_down)
                    {
                        ((KeyboardEventTriggerSetupWindow) setup_window).Deserialize_AddKey(k);
                    }
                }
            }

        /** <summary>
         *  Packaged values for serialzation are stored here
         *  Warning:
         *   -- override with "new" keyword
         *   -- be sure to call the base getObjectData
         * </summary>
         * */
         public new void GetObjectData(SerializationInfo info, StreamingContext context) 
         {
             //throw new NotImplementedException();

             base.GetObjectData(info, context);

             string[] keyStrings = new string[monitor_keys_down.Count];
             System.Windows.Forms.Keys[] keys = monitor_keys_down.ToArray();

             for (int i = 0; i < keys.Length; i++)
             {
                 keyStrings[i] = keys[i].ToString();
             }

             info.AddValue("monitor_keys", keyStrings);
         }
    #endregion
    }
}
