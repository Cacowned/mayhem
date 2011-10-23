using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Wpf
{
    public class MayDuinoAnalogEventConfig : MayduinoEventConfig
    {
        private int analogPin;
        private bool condition;
        private int eventArgs;

        public MayDuinoAnalogEventConfig(int analogPin, bool condition, int eventArgs)
        {
            this.analogPin = analogPin;
            this.condition = condition;
            this.eventArgs = eventArgs; 
        }

        public override void FillSelections()
        {
            Dictionary<string, int> pinBoxItems = new Dictionary<string, int>();
            Dictionary<string, bool> conditionItems = new Dictionary<string, bool>();

            lbl_pullup.Visibility = bx_pullup.Visibility =  System.Windows.Visibility.Collapsed;

            // fill pin values

            for (int i = 0; i < 6; i++)
            {
                string pinString = "A" + i;
                pinBoxItems[pinString] = i; 
            }

            bx_pin.ItemsSource = pinBoxItems;
            bx_pin.DisplayMemberPath = "Key";
            bx_pin.SelectedValuePath = "Value";


            conditionItems["SMALLER"] = false;
            conditionItems["GREATER"] = true;

            bx_cond.ItemsSource = conditionItems;
            bx_cond.DisplayMemberPath = "Key";
            bx_cond.SelectedValuePath = "Value";

            bx_pin.SelectedIndex = analogPin;
            bx_cond.SelectedIndex = condition ? 1 : 0;
            bx_eventVals.Text = eventArgs.ToString();

        }

        public override string Title
        {
            get
            {
                return "MayDuino Analog Pin Event";
            }
        }

    }
}
