using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MayhemWpf.UserControls
{
    public class IWpfConfiguration : UserControl
    {
        public virtual string Title { get { return "Title"; } }
        public virtual bool OnSave() { return true; }
        public virtual void OnCancel() { }
        public virtual void OnLoad() { }
        public virtual void OnClosing() { }
        public delegate void ConfigCanSaveHandler(bool canSave);
        public event ConfigCanSaveHandler CanSavedChanged;
        
        // When the configuration windows open, you can't save without changing fields.
        private bool canSave = false;
        public bool CanSave
        {
            get
            {
                return canSave;
            }
            set
            {
                // Only do this if the flag is being switched
                if (value != canSave)
                {
                    canSave = value;

                    if (CanSavedChanged != null)
                    {
                        CanSavedChanged(canSave);
                    }
                }
            }
        }
    }
}
