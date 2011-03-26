using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections;

namespace ArduinoModules.Arduino_Interface
{


    /// <summary>
    /// Basically wraps serial port communication with the board
    /// </summary>
    public class ArduinoBase
    {
        // Meta Data
        public static string description = "Arduino Base Class"; 

        // Serial Port 
        protected SerialPort port;
        public static int DEFAULT_BAUDRADE = 115200;
        public static Parity DEFAULT_PARITY = Parity.None;
        public static int DEFAULT_BITS = (int) StopBits.One;

        public void InitializeWithSerialPort(string port_str)
        {
            port = new SerialPort(
                    port_str,
                    DEFAULT_BAUDRADE,
                    DEFAULT_PARITY,
                    DEFAULT_BITS
                );

            port.DataReceived += new SerialDataReceivedEventHandler(port_dataReceived);


                        
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
        public virtual void process_response(string[] lines)
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

        public DigitalIO[] digital_pins = new DigitalIO[16];
        public AnalogInput[] analog_inputs = new AnalogInput[6];
        public AnalogOutput[] analog_outputs = new AnalogOutput[6]; 



        public override void port_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<String> string_lines = new List<string>();
            while (port.BytesToRead > 0)
            {
                string_lines.Add(port.ReadLine());
            }

            process_response(string_lines.ToArray());
        }

        // command
        public override void process_response(string[] lines)
        {
            throw (new NotImplementedException());
        }



      
    }

}
