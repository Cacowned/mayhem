using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MayhemApp.Business_Logic
{
    /**<summary>
     * A connection item is the base class for the entities representing a connectio between trigger and action
     *  (i.e.) MayhemTrigger, MayhemAction
     * </summary>
     * */
    [Serializable]
    public class MayhemConnectionItem : ISerializable, IMayhemConnectionItemCommon
    {


        //public delegate void Fire(object sender, EventArgs e);
        public MayhemConnection associatedConnection { get; set; }
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
            this.associatedConnection = (MayhemConnection)info.GetValue("AssociatedConnection", typeof(MayhemConnection));
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
}
