using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.UserControls;
using X10Modules.Insteon;

namespace X10Modules.Wpf
{
    /// <summary>
    /// User interface logic for the InsteonX10Reaction Module
    /// </summary>
    public partial class InsteonX10ReactionConfig : WpfConfiguration
    {
        private readonly MayhemSerialPortMgr serial;
        private X10Controller x10;

        public X10HouseCode SelectedHousecode
        {
            get { return (X10HouseCode)Enum.Parse(typeof(X10HouseCode), (string)houseID.SelectedValue); }
        }

        public X10UnitCode SelectedUnitcode
        {
            get { return (X10UnitCode)Enum.Parse(typeof(X10UnitCode), (string)unitID.SelectedValue); }
        }

        public X10CommandCode SelectedCommandcode
        {
            get
            {
                return (X10CommandCode)Enum.Parse(typeof(X10CommandCode), (string)commandID.SelectedValue);
            }
        }

        public string SelectedPortName
        {
            get { return (string)deviceList.SelectedValue; }
        }

        public InsteonX10ReactionConfig()
        {
            serial = MayhemSerialPortMgr.Instance;
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// Initialize the config
        /// </summary>
        public void Init()
        {
            serial.UpdatePortList();
            // TODO: make this auto-detect the X10 Module, like for the Arduino Uno
            // deviceList.ItemsSource = serial.serialPortNames;

            Dictionary<string, string> portList = serial.GetInsteonPortNames(new InsteonUsbModemSerialSettings());

            if (portList.Count > 0)
            {
                deviceList.ItemsSource = portList;
                deviceList.DisplayMemberPath = "Value";
                deviceList.SelectedValuePath = "Key";
                deviceList.SelectedIndex = 0;
            }

            List<string> houseCodes = Enum.GetNames(typeof(X10HouseCode)).ToList(); houseCodes.Sort();
            houseID.ItemsSource = houseCodes;

            List<string> unitCodes = Enum.GetNames(typeof(X10UnitCode)).ToList(); unitCodes.Sort();
            unitID.ItemsSource = unitCodes;

            List<string> commandCodes = new List<string>(); // Enum.GetNames(typeof(X10CommandCode)).ToList(); commandCodes.Sort();

            commandCodes.Add("ON");
            commandCodes.Add("OFF");
            commandCodes.Add("TOGGLE");

            commandID.ItemsSource = commandCodes;

            deviceList.SelectedIndex = 0;
            houseID.SelectedIndex = 0;
            unitID.SelectedIndex = 0;
            commandID.SelectedIndex = 0;

            CanSave = true; 
        }

        /// <summary>
        /// Tests Actions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // test x10 module
            string portname = (string)deviceList.SelectedValue;
            if (x10 == null)
                x10 =  X10Controller.ControllerForPortName(portname);
            if (x10.Initialized)
            {
                Logger.WriteLine("initialized");
                // send a test command
                // x10.X10SendCommand(0x6, 0x6, 0x2);
                // start the sending thread explicitly

                // start threaded as x10 might require some send repeats, etc.. 
                // TODO: delegate to handle the send result
                X10HouseCode houseC = SelectedHousecode;
                X10UnitCode unitC = SelectedUnitcode;
                X10CommandCode commandC = SelectedCommandcode;
                new Thread(new System.Threading.ThreadStart(() => x10.X10SendCommand(houseC, unitC, commandC))).Start();
            }
        }

        /// <summary>
        /// called when the module config save button is clicked s
        /// </summary>
        /// <returns></returns>
        public override void OnSave()
        {
            if (x10 != null)
                x10.Dispose();
            Logger.WriteLine("OnSave");
        }

        public override string Title
        {
            get
            {
                return "X10 Reaction";
            }
        }
    }
}
