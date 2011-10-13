using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Wpf
{
    public class MayDuinoDigitalEventConfig : MayduinoEventConfig
    {
        public override void FillSelections()
        {
            // hide activation value
            lbl_eventVals.Visibility = System.Windows.Visibility.Collapsed;
            bx_eventVals.Visibility = System.Windows.Visibility.Collapsed;

            // fill pin values
            for (int i = 2; i <= 12; i++)
            {
                string pinString = "D" + i;
                bx_pin.Items.Add(pinString);
            }

            // fill event types
            bx_cond.Items.Add("HIGH");
            bx_cond.Items.Add("LOW");

            // fill
        }

        public override string Title
        {
            get
            {
                return "MayDuino Digital Event";
            }
        }
    }
}
