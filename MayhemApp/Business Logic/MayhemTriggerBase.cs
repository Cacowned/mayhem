using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;

namespace MayhemApp.Business_Logic
{
    [Serializable]
    public abstract class MayhemTriggerBase : MayhemConnectionItem, ISerializable, IMayhemTriggerCommon, IMayhemConnectionItemCommon
    {
        /*
         *  This class is the trigger component of a MayhemConnection
         * 
         * */
        public delegate void triggerActivateHandler(object sender, EventArgs e);
        public virtual event triggerActivateHandler onTriggerActivated;
        public bool triggerEnabled = false;


        public MayhemTriggerBase(string text, string sTitle, string hTxt)
        {
            description = text;
            subTitle = sTitle;
            helpText = hTxt;
            BitmapImage redImg = new BitmapImage(new Uri("../Images/redbutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/button-glow.png", UriKind.Relative));
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


        public virtual new void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[MayhemTrigger] OnDoubleClick");
        }

        #region ISerializable for MayhemTrigger
        public MayhemTriggerBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.onTriggerActivated = (triggerActivateHandler)info.GetValue("TriggerActivateHandler", typeof(triggerActivateHandler));
            this.triggerEnabled = info.GetBoolean("TriggerEnabled");
            BitmapImage redImg = new BitmapImage(new Uri("../Images/redbutton.png", UriKind.Relative));
            BitmapImage glowImg = new BitmapImage(new Uri("../Images/button-glow.png", UriKind.Relative));
            this.template_data = new MayhemButton(this.description, this.subTitle, this.helpText, redImg, glowImg, MayhemButton.MayhemButtonType.TRIGGER);
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
}
