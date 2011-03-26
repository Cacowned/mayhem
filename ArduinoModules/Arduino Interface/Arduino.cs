using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections;
using System.Diagnostics;

namespace ArduinoModules.Arduino_Interface
{


    /// <summary>
    /// Basically wraps serial port communication with the board
    /// </summary>
    public class ArduinoBase
    {
        // Meta Data
        public static string description = "Arduino Base Class";


        // Pins
        public virtual DigitalIO[] digital_pins; 
        public virtual AnalogInput[] analog_inputs;
        public virtual AnalogOutput[] analog_outputs ;

        // Serial Port 
        protected SerialPort port;
        public static int DEFAULT_BAUDRADE = 115200;
        public static Parity DEFAULT_PARITY = Parity.None;
        public static int DEFAULT_BITS = (int) StopBits.One;
        public readonly bool port_connected {
                                                get 
                                                {
                                                    if (port != null)
                                                    {
                                                        return port.IsOpen;
                                                    }
                                                    else
                                                    {
                                                        return false; 
                                                    }
                                                }
            
                                            }


        // Event handling 
        public delegate void PortConnectedHandler (ArduinoBase sender, EventArgs e);
        public event PortConnectedHandler OnPortConnected;

        public delegate void StateUpdatedHandler(ArduinoBase sender, EventArgs e);
        public event StateUpdatedHandler OnStateUpdated; 

        
         

        /// <summary>
        /// Initialize the serial port, and send event to listeners
        /// </summary>
        /// <param name="port_str"></param>
        public virtual void InitializeWithSerialPort(string port_str)
        {
            try
            {

                port = new SerialPort(
                        port_str,
                        DEFAULT_BAUDRADE,
                        DEFAULT_PARITY,
                        DEFAULT_BITS
                    );

                if (OnPortConnected != null)
                {
                    OnPortConnected(this, null);
                }

                port.DataReceived += new SerialDataReceivedEventHandler(port_dataReceived);
                Debug.WriteLine("Port Connected Successfully");

            }
            catch
            {
                // event receiver should try to find out if the port 
                // connected successfully 
                OnPortConnected(this, null);
                Debug.WriteLine("Problem with Port Connection"); 
            }
 
        }

        public virtual void port_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // implement handler here
            throw (new NotImplementedException());
        }

        /// <summary>
        /// Process Response From the Arduino
        /// </summary>
        /// <param name="s">response string</param>
        public virtual void ProcessResponse(string[] lines)
        {
            
        }

    }


    // PWM: 3, 5, 6, 9, 10, and 11. Provide 8-bit PWM output with the analogWrite() function.


    /// <summary>
    /// The Standard Arduino, DuemilaNove with ATMega328
    /// </summary>
    public class Arduino_DuemilaNove_ATMega328 : ArduinoBase
    {


        // Metadata
        public string description = "Arduino DuelmilaNove with ATMega 328";

        public override DigitalIO[] digital_pins = new DigitalIO[16];
        public override AnalogInput[] analog_inputs = new AnalogInput[6];
        public override AnalogOutput[] analog_outputs = new AnalogOutput[6];


        public override void InitializeWithSerialPort(string port_str)
        {
            // additional init code here
            // +++ todo +++

            // start up the port, notify listeners
            base.InitializeWithSerialPort(port_str);
         
        }


        public override void port_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<String> string_lines = new List<string>();
            while (port.BytesToRead > 0)
            {
                string_lines.Add(port.ReadLine());
            }

            ProcessResponse(string_lines.ToArray());
        }

        /// <summary>
        /// Process reponses from the Core running on the chip
        /// This basically means updating the current state and notifying listeners
        /// </summary>
        /// <param name="lines"></param>
        public override void ProcessResponse(string[] lines)
        {
            throw new NotImplementedException();
        }

      //  public SetDigitalPin

        /// <summary>
        /// Set digital output to a value
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        public override void SetDigitalOutput(DigitalIO pin, PinMode value)
        {
            throw new NotFiniteNumberException();
        }

        /// <summary>
        /// Set an analog output
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        public override void SetAnalogOutput(DigitalIO pin, UInt16 value)
        {
            throw new NotImplementedException();
        }
      
    }

}
