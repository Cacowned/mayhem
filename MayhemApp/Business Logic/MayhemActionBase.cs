using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MayhemApp.Business_Logic
{
    /**<summary>
     * Action component of MayhemConnection
     * </summary>
     */
    [Serializable]
    public abstract class MayhemActionBase : MayhemConnectionItem, ISerializable, IMayhemConnectionItemCommon
    {

        protected int ID = 0;

        public MayhemActionBase(string text, string sTitle, string hTxt)
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
        public abstract void PerformAction(MayhemTriggerBase sender);

        public virtual new void OnDoubleClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("[MayhemAction] OnDoubleClick");
        }

        public MayhemActionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
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
        }
    }
}
