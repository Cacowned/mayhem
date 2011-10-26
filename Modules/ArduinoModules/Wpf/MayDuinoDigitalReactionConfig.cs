﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Wpf
{
    class MayDuinoDigitalReactionConfig : MayduinoReactionConfig
    {
        private int digitalPin;
        private bool outState;

        public MayDuinoDigitalReactionConfig(int digitalPin, bool outState)
        {
            // TODO: Complete member initialization
            this.digitalPin = digitalPin;
            this.outState = outState;
        }
        public override void FillSelections()
        {
            Dictionary<string, int> pinBoxItems = new Dictionary<string, int>();
            Dictionary<string, bool> conditionItems = new Dictionary<string, bool>();
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
            conditionItems["HIGH"] = true;
            conditionItems["LOW"] = false;

            bx_cond.ItemsSource = conditionItems;
            bx_cond.DisplayMemberPath = "Key";
            bx_cond.SelectedValuePath = "Value";


            bx_pin.SelectedIndex = digitalPin - 2;
            bx_cond.SelectedIndex = outState ? 0 : 1;
        }

        public override string Title
        {
            get
            {
                return "MayDuino Digital Pin Reaction";
            }
        }
    }
}