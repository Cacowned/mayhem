﻿/*
 * ArduinoFirmata.cs
 * 
 * Manages and reads the state of an Arduino running the Firmata Firmware.
 * Serial communications backend is MayhemSerial.
 * 
 * More info on Firmata:                http://firmata.org/wiki/Main_Page  
 * More info on the Firmata Procol:     http://www.firmata.org/wiki/V2.2ProtocolDetails
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MayhemCore;
using MayhemSerial;
using Timer = System.Timers.Timer;


namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Digital pin write modes for use by Mayhem
    /// </summary>
    public enum DIGITAL_WRITE_MODE
    {
        TOGGLE,         // pin toggles, initially set to 0
        HIGH,           // pull pin high
        LOW,            // pull low
        PULSE_ON,       // pull pin high for a short interval
        PULSE_OFF       // pull pin low for a short interval
    }

    /// <summary>
    /// Types of digital pin changes that can be detected
    /// </summary>
    public enum DIGITAL_PIN_CHANGE
    {
        HIGH,
        LOW,
        RISING,
        FALLING
    }

    public enum ANALOG_PIN_CHANGE
    {
        EQUALS,
        GREATER,
        LOWER
    }

    /// <summary>
    /// Describes the PIN MODE and also if it is analog or not used
    /// </summary>
    public enum PIN_MODE
    {
        INPUT   = 0x00,
        OUTPUT  = 0x01,
        ANALOG  = 0x02,
        PWM     = 0x03,
        SERVO   = 0x04,
        SHIFT   = 0x05,
        I2C     = 0x06,
        UNASSIGNED = 0xff
    }

    // use a struct instead of enum, to avoid casting later
    public  struct FIRMATA_MSG
    {
        public static readonly byte START_SYSEX = 0xF0; // start a MIDI Sysex message
        public static readonly byte END_SYSEX = 0xF7; // end a MIDI Sysex message
        public static readonly byte PIN_MODE_SET = 0xF4; // change pin mode
        public static readonly byte PIN_MODE_QUERY = 0x72; // ask for current and supported pin modes
        public static readonly byte PIN_MODE_RESPONSE = 0x73; // reply with current and supported pin modes
        public static readonly byte PIN_STATE_QUERY = 0x6D;
        public static readonly byte PIN_STATE_RESPONSE = 0x6E;
        public static readonly byte CAPABILITY_QUERY = 0x6B;
        public static readonly byte CAPABILITY_RESPONSE = 0x6C;
        public static readonly byte ANALOG_MAPPING_QUERY = 0x69;
        public static readonly byte ANALOG_MAPPING_RESPONSE = 0x6A;
        public static readonly byte REPORT_FIRMWARE = 0x79; // report name and version of the firmware
        public static readonly byte SAMPLING_INTERVAL = 0x7A; // used to set the sampling interval

        // message reports
        public static readonly byte ANALOG_IO_MESSAGE = 0xE0;
        public static readonly byte DIGITAL_IO_MESSAGE = 0x90;
        public static readonly byte REPORT_ANALOG_PIN = 0xC0;
        public static readonly byte REPORT_DIGITAL_PORT = 0xD0;     
    }

    /// <summary>
    /// Represents an  arduino pin
    /// </summary>
    public class Pin
    {
        public PIN_MODE mode;
        public byte analog_channel;
        public UInt64 supported_modes;
        public int value;
        public int id;
        public bool flagged = false;                // in use by a module
    }
    

    /// <summary>Extension methods for EventHandler-type delegates.</summary>
    public static class EventExtensions
    {
        /// <summary>Raises the event (on the UI thread if available).</summary>
        /// <param name="multicastDelegate">The event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <returns>The return value of the event invocation or null if none.</returns>
        public static object Raise(this MulticastDelegate multicastDelegate, object sender, EventArgs e)
        {
            object retVal = null;

            MulticastDelegate threadSafeMulticastDelegate = multicastDelegate;
            if (threadSafeMulticastDelegate != null)
            {
                foreach (Delegate d in threadSafeMulticastDelegate.GetInvocationList())
                {
                    var synchronizeInvoke = d.Target as ISynchronizeInvoke;
                    if ( (synchronizeInvoke != null) && synchronizeInvoke.InvokeRequired)
                    {
                        retVal = synchronizeInvoke.EndInvoke(synchronizeInvoke.BeginInvoke(d, new[] { sender, e }));
                    }
                    else
                    {
                        retVal = d.DynamicInvoke(new[] { sender, e });
                    }
                }
            }

            return retVal;
        }
    }

    public class ArduinoFirmata : ISerialPortDataListener
    {


        private static Dictionary<string, ArduinoFirmata> instances = new Dictionary<string, ArduinoFirmata>();

        private string portName_ = null;             // name of the serial port
        public string portName { get { return portName_; } }

        private static MayhemSerialPortMgr mSerial = MayhemSerialPortMgr.Instance;
        public bool initialized = false;

        // parsing
        private byte[] parse_buf = new byte[8192];
        int parse_command_len = 0;
        int parse_count = 0;

        private string firmata_name = null; 
       // int parse_position = 0; 

        // pin information 
        private Pin[] pin_info = new Pin[128];

        // state change events
        public event EventHandler OnInitialized; 
        public event Action<Pin> OnPinAdded;
        public event Action<Pin> OnAnalogPinChanged;
        public event Action<Pin> OnDigitalPinChanged;

        private AsyncOperation operation;

        private Timer readPinsTimer = new Timer(20);


        

        private ArduinoFirmata(string serialPortName)
        {
            operation = AsyncOperationManager.CreateOperation(null);

            if (mSerial.ConnectPort(serialPortName, this,  new ArduinoFirmataSettings()))
            {
                portName_ = serialPortName;
                InitializeFirmata(); 
            }
        }

        /// <summary>
        /// Returns true if an initialized Arduino Instance is already connected to the port with serialPortName. 
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <returns></returns>
        public static bool InstanceExists(string serialPortName)
        {
            if (instances.Keys.Contains(serialPortName) && 
                instances[serialPortName] != null && 
                instances[serialPortName].initialized)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Registers an Arduino Event Listener
        /// </summary>
        /// <param name="l"></param>
        public void RegisterListener(IArduinoEventListener l)
        {
            // subtract listeners first to suppress multi-listener registration
            OnAnalogPinChanged -= l.Arduino_OnAnalogPinChanged;
            OnDigitalPinChanged -= l.Arduino_OnDigitalPinChanged;
            OnInitialized -= l.Arduino_OnInitialized;
            OnPinAdded -= l.Arduino_OnPinAdded; 

            OnAnalogPinChanged += l.Arduino_OnAnalogPinChanged;
            OnDigitalPinChanged += l.Arduino_OnDigitalPinChanged;
            OnInitialized += l.Arduino_OnInitialized;
            OnPinAdded += l.Arduino_OnPinAdded; 
        }

        /// <summary>
        /// Deregisters an Arduino Event Listener
        /// </summary>
        /// <param name="l"></param>
        public void DeregisterListener(IArduinoEventListener l)
        {
            // subtract listeners first to suppress multi-listener registration
            OnAnalogPinChanged -= l.Arduino_OnAnalogPinChanged;
            OnDigitalPinChanged -= l.Arduino_OnDigitalPinChanged;
            OnInitialized -= l.Arduino_OnInitialized;
            OnPinAdded -= l.Arduino_OnPinAdded; 
        }


        /// <summary>
        /// Allow only one instance per port name, generated by this factory method.
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <returns>Initialized instance of Arduino bound to specified port name.</returns>
        public static ArduinoFirmata InstanceForPortname(string serialPortName)
        {
            Logger.WriteLine("InstanceForPortname");
            if (!string.IsNullOrEmpty(serialPortName))
            {                        

                if (instances.Keys.Contains(serialPortName) && instances[serialPortName] != null)
                {
                    return instances[serialPortName];
                }
                else
                {
                    Dictionary<string, string> portNames = mSerial.GetArduinoPortNames();

                    if (portNames.Keys.Contains(serialPortName))
                    {

                        Logger.WriteLine("creating new instance for port " + serialPortName);
                        instances[serialPortName] = new ArduinoFirmata(serialPortName);
                        return instances[serialPortName];
                    }
                }
            }          
            return null;
        }

        /// <summary>
        /// Sends initial Firmata message and initialize the mcu state
        /// </summary>
        private void InitializeFirmata()
        {
            // initialize pin info
            for (int i = 0; i < 128; i++)
            {
                pin_info[i] = new Pin();
                pin_info[i].mode = PIN_MODE.UNASSIGNED;
                pin_info[i].analog_channel = 127;
                pin_info[i].supported_modes = 0;
                pin_info[i].value = 0;
                pin_info[i].id = i;
                pin_info[i].flagged = false;
            }

            // send handshake
            byte[] buf = new byte[3] { FIRMATA_MSG.START_SYSEX, FIRMATA_MSG.REPORT_FIRMWARE, FIRMATA_MSG.END_SYSEX };
            mSerial.WriteToPort(portName_, buf, 3);

            SetSamplingInterval(100);

           // readPinsTimer.Elapsed += new System.Timers.ElapsedEventHandler(readPinsTimer_Elapsed);
            //readPinsTimer.Enabled = true;
        }

        void readPinsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            QueryPins(32);
        }




        public void SetSamplingInterval(UInt16 interval)
        {
            byte[] ms = new byte[2];
            ms[0] =(byte)  (interval & (UInt16) 0xff);
            ms[1] = (byte) ((interval) >> 8);

            byte[] message = new byte[5] { FIRMATA_MSG.START_SYSEX, FIRMATA_MSG.SAMPLING_INTERVAL, ms[0], ms[1], FIRMATA_MSG.END_SYSEX };
            mSerial.WriteToPort(portName_, message, message.Length);
        }

        #region Outgoing commands
      
        /// <summary>
        /// Public Exposure of QueryPins
        /// </summary>
        public void QueryPins()
        {
            QueryPins(64);
        }

        /// <summary>
        /// Queries pins upto certain index
        /// </summary>
        /// <param name="upto"></param>
        private void QueryPins(int upto)
        {
            int pin = 0; 
            // send a state query for for every pin with any modes
            for (pin = 0; pin < upto; pin++)
            {
                byte[] buf = new byte[512];
                int len = 0;
                if (pin_info[pin].supported_modes > 0)
                {
                    buf[len++] = FIRMATA_MSG.START_SYSEX;
                    buf[len++] = FIRMATA_MSG.PIN_STATE_QUERY;
                    buf[len++] = (byte)pin;
                    buf[len++] = FIRMATA_MSG.END_SYSEX;
                }
                //port.Write(buf, len);

                mSerial.WriteToPort(portName_, buf, len);

                //tx_count += len;
            }
        }


        /// <summary>
        /// Public DigitalWrite exposure (for the time being) 
        /// </summary>
        /// <param name="pinIndex"></param>
        /// <param name="val"></param>
        public void DigitalWrite(int pinIndex, int val)
        {
            // retrieve the pin to write to from the index
            if (pinIndex >= 0 && pinIndex <= 128)
            {
                Pin writePin = pin_info[pinIndex];
                DigitalWrite(writePin, val);
            }
        }


        /// <summary>
        /// Sets the logic value of the pin 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="value"></param>
        private void DigitalWrite(Pin p, int value)
        {
            pin_info[p.id].value = value;
            byte[] buf = new byte[3];
            int portNumber = (p.id >> 3) & 0x0F;
            UInt16 port_val = 0;       
            for (int i = 0; i < 8; i++)
            {
                int idx = portNumber * 8 + i; 
                if (p.mode == PIN_MODE.OUTPUT || p.mode == PIN_MODE.INPUT)
                {
                   
                    if (pin_info[i].value>0)
                    {
                        port_val |= (UInt16) (1<< i);
                    }
                }
            }

            buf[0] = (byte)(0x90 | portNumber);
            buf[1] = (byte)(port_val & 0x7f);
            buf[2] = (byte)((port_val >> 7) & 0x7f);

            mSerial.WriteToPort(portName_, buf, 3);
        }

        /// <summary>
        /// Set the pin mode to one of the supported modes
        /// The pin mode change message consists of three bytes 
        /// --- 
        /// 0xF4 pin change command
        /// pin byte
        /// mode byte
        /// ---
        /// </summary>
        /// <param name="p"></param>
        /// <param name="mode"></param>
        public void SetPinMode(Pin p, PIN_MODE mode)
        {
            pin_info[p.id].mode = mode;
            // flag this pin as an output pin (?)
            if (mode == PIN_MODE.OUTPUT)
                pin_info[p.id].flagged = true;
            byte[] buf = new byte[3] { FIRMATA_MSG.PIN_MODE_SET, (byte)p.id, (byte) mode };
            mSerial.WriteToPort(portName_, buf, buf.Length);
        }

#endregion


        /// <summary>
        /// Notfication from the serial port manager when serial data is available
        /// </summary>
        public void port_DataReceived(string portName, byte[] buffer, int numBytes)
        {
            // Logger.WriteLine("port_DataReceived");

            // make a local deep copy of the RX buffer
            byte[] buf = new byte[numBytes];
            Array.Copy(buffer, buf, numBytes);

            int nBytes = numBytes;
            if (nBytes > 0) 
            {
                
                    // determine the message type
                    for (int i = 0; i < nBytes; i++)
                    {
                        byte msn = (byte)(buf[i] & 0xf0);
                        if (msn == FIRMATA_MSG.ANALOG_IO_MESSAGE || msn == FIRMATA_MSG.DIGITAL_IO_MESSAGE || buf[i] == (byte)0xf9)
                        {
                            parse_command_len = 3;
                            parse_count = 0;
                        }
                        else if (msn == FIRMATA_MSG.REPORT_ANALOG_PIN || msn == FIRMATA_MSG.REPORT_DIGITAL_PORT)
                        {
                            parse_command_len = 2;
                            parse_count = 0;
                        }
                        else if (buf[i] == FIRMATA_MSG.START_SYSEX)
                        {
                            parse_count = 0;
                            parse_command_len = parse_buf.Length;
                        }
                        else if (buf[i] == FIRMATA_MSG.END_SYSEX)
                        {
                            parse_command_len = parse_count + 1;
                        }
                        else if ((buf[i] & (byte)(0x80)) == 1)
                        {
                            parse_command_len = 1;
                            parse_count = 0;
                        }

                        // ? 
                        if (parse_count < parse_buf.Length)
                        {
                             parse_buf[parse_count++] = buf[i];
                        }
                        else
                        {
                            break;
                        }

                        if (parse_count == parse_command_len)
                        {
                            // parse the message
                            ProcessMessage();
                            parse_count = 0;
                            parse_command_len = 0;
                        }
                    }
            }         
        }

        private void ProcessMessage()
        {
            byte cmd = (byte)(parse_buf[0] & (byte)0xF0);
            //Logger.WriteLine("ProcessMessage bytes" + parse_count + " cmd " + cmd);

            if (cmd == FIRMATA_MSG.ANALOG_IO_MESSAGE && parse_count == 3) 
            {
		        int analog_ch = (parse_buf[0] & 0x0F);
		        int analog_val = parse_buf[1] | (parse_buf[2] << 7);
		        for (int pin=0; pin<128; pin++) {
			        if (pin_info[pin].analog_channel == analog_ch) {
				        pin_info[pin].value =  analog_val;
				        //printf("pin %d is A%d = %d\n", pin, analog_ch, analog_val);
                        // event callbacks
                        if (this.OnAnalogPinChanged != null)
                        {                        
                            OnAnalogPinChanged(pin_info[pin]);
                        }

				        return;
                }
		      }		    
	        }
	        if (cmd == FIRMATA_MSG.DIGITAL_IO_MESSAGE /*&& parse_count == 3*/) {
		        int port_num = (parse_buf[0] & (byte) 0x0F);
		        int port_val = parse_buf[1] | (parse_buf[2] << 7);
		        int pin = port_num * 8;
		       // Logger.WriteLine(String.Format("PIN: {0}, port_num = {1}, port_val = {2}", pin,port_num, port_val));

                // basically: go through the bits in the register and mask with 1
		        for (; pin <  128; pin++) {                   
			        if (pin_info[pin].mode == PIN_MODE.INPUT) {
				       
                        int val = 0;
                        if ( ((port_val >> pin) & 1) == 1)
                        {
                            val = 1;
                        }
                        else
                        {
                            val = 0;
                        }

                        Logger.WriteLine("pin " + pin + " val " + val);
				        if (pin_info[pin].value != val) {
					       
                            
					        pin_info[pin].value = val;
                            // event callbacks
                            if (this.OnDigitalPinChanged != null)
                            {
                                OnDigitalPinChanged(pin_info[pin]);
                            }
				        }
			        }
                   
		        }
		        return;
	}


            if (parse_buf[0] == FIRMATA_MSG.START_SYSEX && parse_buf[parse_count - 1] == FIRMATA_MSG.END_SYSEX)
            {
                // Sysex message
                if (parse_buf[1] == FIRMATA_MSG.REPORT_FIRMWARE)
                {
                    char[] name = new char[parse_count - 5];
                    int len = 0;
                    for (int i = 4; i < parse_count - 2; i += 2)
                    {
                        name[len++] = Convert.ToChar(((parse_buf[i] & 0x7F)
                          | ((byte)(parse_buf[i + 1] & (byte)(0x7F)) << 7)));
                    }
                    name[len++] = '-';
                    name[len++] = Convert.ToChar(parse_buf[2] + '0');
                    name[len++] = '.';
                    name[len++] = Convert.ToChar(parse_buf[3] + '0');
                    name[len++] = Convert.ToChar(0);
                    firmata_name = new String(name);
                    //consider Arduino initialized if it has a new name

                    this.initialized = true;
                    if (this.OnInitialized != null)
                    {
                        OnInitialized(this, null);
                    }

                    // query the board's capabilities only after hearing the
                    // REPORT_FIRMWARE message.  For boards that reset when
                    // the port open (eg, Arduino with reset=DTR), they are
                    // not ready to communicate for some time, so the only
                    // way to reliably query their capabilities is to wait
                    // until the REPORT_FIRMWARE message is heard.
                    byte[] buf = new byte[80];
                    len = 0;
                    buf[len++] = FIRMATA_MSG.START_SYSEX;
                    buf[len++] = FIRMATA_MSG.ANALOG_MAPPING_QUERY; // read analog to pin # info
                    buf[len++] = FIRMATA_MSG.END_SYSEX;
                    buf[len++] = FIRMATA_MSG.START_SYSEX;
                    buf[len++] = FIRMATA_MSG.CAPABILITY_QUERY; // read capabilities
                    buf[len++] = FIRMATA_MSG.END_SYSEX;
                    for (int i = 0; i < 16; i++)
                    {
                        buf[len++] = (byte)(0xC0 | i);  // report analog
                        buf[len++] = 1;
                        buf[len++] = (byte)(0xD0 | i);  // report digital
                        buf[len++] = 1;
                    }

                    mSerial.WriteToPort(portName_, buf, len);
                    //tx_count += len;
                }
                else if (parse_buf[1] == FIRMATA_MSG.CAPABILITY_RESPONSE)
                {
                    int pin, i, n;
                    for (pin = 0; pin < 128; pin++)
                    {
                        pin_info[pin].supported_modes = 0;
                    }
                    for (i = 2, n = 0, pin = 0; i < parse_count; i++)
                    {
                        if (parse_buf[i] == 127)
                        {
                            pin++;
                            n = 0;
                            continue;
                        }
                        if (n == 0)
                        {
                            // first byte is supported mode
                            pin_info[pin].supported_modes |= ((ulong)1 << parse_buf[i]);
                        }
                        n = n ^ 1;
                    }
                    // send a state query for for every pin with any modes
                    for (pin = 0; pin < 128; pin++)
                    {
                        byte[] buf = new byte[512];
                        int len = 0;
                        if (pin_info[pin].supported_modes > 0)
                        {
                            buf[len++] = FIRMATA_MSG.START_SYSEX;
                            buf[len++] = FIRMATA_MSG.PIN_STATE_QUERY;
                            buf[len++] = (byte)pin;
                            buf[len++] = FIRMATA_MSG.END_SYSEX;
                        }
                        //port.Write(buf, len);

                        mSerial.WriteToPort(portName_, buf, len);


                        //tx_count += len;
                    }
                }
                else if (parse_buf[1] == FIRMATA_MSG.ANALOG_MAPPING_RESPONSE)
                {
                    int pin = 0;
                    for (int i = 2; i < parse_count - 1; i++)
                    {
                        pin_info[pin].analog_channel = parse_buf[i];
                        pin++;
                    }
                    return;
                }
                else if (parse_buf[1] == FIRMATA_MSG.PIN_STATE_RESPONSE && parse_count >= 6)
                {
                    int pin = parse_buf[2];
                    pin_info[pin].mode = (PIN_MODE)parse_buf[3];
                    pin_info[pin].value = parse_buf[4];
                    if (parse_count > 6) pin_info[pin].value |= (byte)(parse_buf[5] << 7);
                    if (parse_count > 7) pin_info[pin].value |= (byte)(parse_buf[6] << 14);
                    Logger.WriteLine("Added Pin! " + pin + " " + pin_info[pin].mode + " " + pin_info[pin].value);

                    /////////////////////// post asynchronous event on main thread
                    if (this.OnPinAdded != null)
                    {
                        OnPinAdded(pin_info[pin]);
                    }
                    //////////////////////

                }
                return;
            }
        }
    }
}
