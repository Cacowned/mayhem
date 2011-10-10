using System.Windows.Controls;

namespace PhidgetModules.Wpf.UserControls
{
    public class PhidgetConfigControl : UserControl
    {
        public virtual string Title
        {
            get
            {
                return "Phidget Config";
            }
        }

        public virtual void OnLoad() { }
        public virtual void OnSave() { }

        public virtual string CheckValidity() { return ""; }

        public delegate void Revalidate();
        public event Revalidate OnRevalidate;

        protected void Validate()
        {
            if (OnRevalidate != null)
            {
                OnRevalidate();
            }
        }
    }
}
