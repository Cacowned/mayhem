using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Wpf
{
    public class MayDuinoDigitalEventConfig : MayduinoEventConfig
    {
        private int digitalPin;
        private bool condition;
        private bool pullUp;

        public MayDuinoDigitalEventConfig(int digitalPin, bool condition, bool pullUp)
        {
            // TODO: Complete member initialization
            this.digitalPin = digitalPin;
            this.condition = condition;
            this.pullUp = pullUp;
        }

        public override void FillSelections()
        {

            Dictionary<string, int>  pinBoxItems = new Dictionary<string, int>();
            Dictionary<string, bool> conditionItems = new Dictionary<string, bool>();
            Dictionary<string, bool> pullupItems = new Dictionary<string, bool>();
            // hide activation value
            lbl_eventVals.Visibility = System.Windows.Visibility.Collapsed;
            bx_eventVals.Visibility = System.Windows.Visibility.Collapsed;

            // fill pin values
            for (int i = 2; i <= 12; i++)
            {
                string pinString = "D" + i;
                pinBoxItems[pinString] = i;
                // bx_pin.Items.Add(pinString);
            }

            bx_pin.ItemsSource = pinBoxItems;
            bx_pin.DisplayMemberPath = "Key";
            bx_pin.SelectedValuePath = "Value";


            // fill event types
            conditionItems["LOW"] = false;
            conditionItems["HIGH"] = true;

            bx_cond.ItemsSource = conditionItems;
            bx_cond.DisplayMemberPath = "Key";
            bx_cond.SelectedValuePath = "Value";

            // fill pullup box
            pullupItems["OFF"] = false;
            pullupItems["ON"] = true;
            bx_pullup.ItemsSource = pullupItems;
            bx_pullup.DisplayMemberPath = "Key";
            bx_pullup.SelectedValuePath = "Value";
           

            // show selections
            bx_pin.SelectedIndex = digitalPin-2;
            bx_cond.SelectedIndex = condition ? 1 : 0;
            bx_pullup.SelectedIndex = pullUp ? 1: 0;
        }

        public override string Title
        {
            get
            {
                return "MayDuino Digital Pin Event";
            }
        }
    }
}
