using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PhoneModules.Controls
{
    public class PhoneUIElement : UserControl
    {
        public virtual string Text { get; set; }
        public PhoneLayoutElement LayoutInfo;
    }
}
