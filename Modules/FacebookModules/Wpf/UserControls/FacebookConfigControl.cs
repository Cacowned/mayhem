using System.Windows.Controls;

namespace FacebookModules.Wpf.UserControls
{
    public class FacebookConfigControl : UserControl
    {
        public virtual string Title
        {
            get
            {
                return "Facebook Config";
            }
        }
        public virtual void OnLoad() { }

        public virtual void OnSave() { }

        public virtual string GetErrorString() { return string.Empty; }
    }
}
