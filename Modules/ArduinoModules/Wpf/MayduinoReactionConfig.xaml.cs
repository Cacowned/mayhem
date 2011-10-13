using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemWpf.UserControls;
using MayhemSerial;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Interaction logic for MayduinoEvent.xaml
    /// </summary>

    public partial class MayduinoReactionConfig : WpfConfiguration
    {
        
        public int Pin
        {
            get { return (int) bx_pin.SelectedValue; }
        }

        public object Condition
        {
            get { return bx_cond.SelectedValue; }
        }

        public int ActivationValue
        {
            get
            {
                int value = 0;
                try
                {
                    value = int.Parse(bx_eventVals.Text);

                }
                catch
                {
                    value = 0; 
                }
                return value;
            }
        }


        public string ArduinoPortName
        {
            get;
            private set;
        }

        public MayduinoReactionConfig()
        {
            InitializeComponent();
        }

        public override void OnLoad()
        {         
            CanSave = true;
            FillSelections();
        }

        public virtual void FillSelections()
        {
            throw new NotImplementedException();
        }

       
    }
}
