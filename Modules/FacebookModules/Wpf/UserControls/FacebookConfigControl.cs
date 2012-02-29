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

        public virtual bool CanSave
        {
            get;
            set;
        }

        public virtual string GetErrorString() { return string.Empty; }

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
