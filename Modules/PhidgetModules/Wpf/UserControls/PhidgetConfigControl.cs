using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PhidgetModules.Wpf.UserControls
{
    public abstract class PhidgetConfigControl : UserControl
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
    }
}
