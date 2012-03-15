/*
 * ArduinoFirmata.cs
 * 
 * Manages and reads the state of an Arduino running the Firmata Firmware
 * 
 * For more info see http://firmata.org/wiki/Main_Page --> firmata test 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * TODO: Rewrite the parser for release
 * Author: Sven Kratz
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemSerial;
using System.IO.Ports;
using System.Diagnostics;


namespace ArduinoModules.ArduinoFirmata
{
    public enum PIN_MODE
    {
        INPUT   = 0x00,
        OUTPUT  = 0x01,
        ANALOG  = 0x02,
        PWM     = 0x03,
        SERVO   = 0x04,
        SHIFT   = 0x05,
        I2C     = 0x06
    }

    // use a struct instead of enum, to avoid casting later
    public  struct MSG
    {
        public static readonly byte START_SYSEX = 0xF0; // start a MIDI Sysex message
        public static readonly byte END_SYSEX = 0xF7; // end a MIDI Sysex message
        public static readonly byte PIN_MODE_QUERY = 0x72; // ask for current and supported pin modes
        public static readonly byte PIN_MODE_RESPONSE = 0x73; // reply with current and supported pin modes
        public static readonly byte PIN_STATE_QUERY = 0x6D;
        public static readonly byte PIN_STATE_RESPONSE = 0x6E;
        public static readonly byte CAPABILITY_QUERY = 0x6B;
        public static readonly byte CAPABILITY_RESPONSE = 0x6C;
        public static readonly byte ANALOG_MAPPING_QUERY = 0x69;
        public static readonly byte ANALOG_MAPPING_RESPONSE = 0x6A;
        public static readonly byte REPORT_FIRMWARE = 0x79; // report name and version of the firmware

        // message reports
        public static readonly byte ANALOG_IO_MESSAGE = 0xE0;
        public static readonly byte DIGITAL_IO_MESSAGE = 0x90;
        public static readonly byte REPORT_ANALOG_PIN = 0xC0;
        public static readonly byte REPORT_DIGITAL_PORT = 0xD0;

       
    }

    public struct Pin
    {
        public PIN_MODE mode;
        public byte analog_channel;
        public UInt64 supported_modes;
        public UInt32 value;
    }

    public class ArduinoFirmata : ISerialPortDataListener
    {

        private string portName = null;             // name of the serial port
        public static readonly string TAG = "[ArduinoFirmata] : ";
        private MayhemSerialPortMgr mSerial = MayhemSerialPortMgr.instance;
        public bool initialized = false;

        // parsing
        private byte[] parse_buf = new byte[4096];
        int parse_command_len = 0;
        int parse_count = 0;

        private string firmata_name = null; 
       // int parse_position = 0; 

        // pin information 
        private Pin[] pin_info = new Pin[128];

        // state change events
        public event Action<Pin> OnPinAdded;
        public event Action<Pin> OnAnalogPinChanged;
        public event Action<Pin> OnDigitalPinChanged;

        public ArduinoFirmata(string serialPortName)
        {

            if (mSerial.ConnectPort(serialPortName, this,  new ARDUINO_FIRMATA_SETTINGS()))
            {
                portName = serialPortName;
                InitializeFirmata(); 
            };

        }

        /// <summary>
        /// Sends initial Firmata message and initailized the mcu state
        /// </summary>
        private void InitializeFirmata()
        {
            byte[] buf = new byte[3] { MSG.START_SYSEX, MSG.REPORT_FIRMWARE, MSG.END_SYSEX };
            mSerial.WriteToPort(portName, buf, 3);
        }

        /// <summary>
        /// Notfication from the serial port manager when serial data is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine(TAG + "port_DataReceived");

            SerialPort p = sender as SerialPort; 

            int nBytes = p.BytesToRead;
            if (nBytes > 0) 
            {

                //byte[] data = new byte[nBytes];
                //p.Read(data, 0, nBytes);

                p.Read(parse_buf, parse_count, nBytes);

              
                // determine the message type
                for (int i = parse_count; i < parse_count + nBytes; i++)
                {
                    byte msn = (byte) (parse_buf[i] &  0xf0) ;
                    if (msn == MSG.ANALOG_IO_MESSAGE || msn == MSG.DIGITAL_IO_MESSAGE || parse_buf[i] == (byte)0xf9)
                    {
                        parse_command_len = 3;
                        parse_count = 0; 
                    }
                    else if (msn == MSG.REPORT_ANALOG_PIN || msn == MSG.REPORT_DIGITAL_PORT)
                    {
                        parse_command_len = 2;
                        parse_count = 0;
                    }
                    else if (parse_buf[i] == MSG.START_SYSEX)
                    {
                        parse_count = 0;
                        parse_command_len = parse_buf.Length;
                    }
                    else if (parse_buf[i] == MSG.END_SYSEX)
                    {
                        parse_command_len = parse_count + 1; 
                    }
                    else if ((parse_buf[i] & (byte)(0x80)) == 1)
                    {
                        parse_command_len = 1;
                        parse_count = 0; 
                    }

                    // ? 
                    if (parse_count < parse_buf.Length)
                    {
                        parse_buf[parse_count++] = parse_buf[i];
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
            Debug.WriteLine(TAG + "ProcessMessage bytes" + parse_count + " cmd " + cmd);

            if (cmd == MSG.ANALOG_IO_MESSAGE && parse_count == 3) 
            {
		        int analog_ch = (parse_buf[0] & 0x0F);
		        int analog_val = parse_buf[1] | (parse_buf[2] << 7);
		        for (int pin=0; pin<128; pin++) {
			        if (pin_info[pin].analog_channel == analog_ch) {
				        pin_info[pin].value = (UInt32) analog_val;
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
	        if (cmd == MSG.DIGITAL_IO_MESSAGE && parse_count == 3) {
		        int port_num = (parse_buf[0] & 0x0F);
		        int port_val = parse_buf[1] | (parse_buf[2] << 7);
		        int pin = port_num * 8;
		        //printf("port_num = %d, port_val = %d\n", port_num, port_val);
		        for (int mask=1; (mask & 0xFF) == 1; mask <<= 1, pin++) {
			        if (pin_info[pin].mode == PIN_MODE.INPUT) {
				        UInt32 val = (port_val & mask) == 1 ? (UInt32) 1 : 0;
				        if (pin_info[pin].value != val) {
					        //printf("pin %d is %d\n", pin, val);
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


	if (parse_buf[0] == MSG.START_SYSEX && parse_buf[parse_count-1] == MSG.END_SYSEX) 
    {
		// Sysex message
		if (parse_buf[1] == MSG.REPORT_FIRMWARE) {
			char[] name = new char[140];
			int len=0;
			for (int i=4; i < parse_count-2; i+=2) 
            {
				name[len++] = Convert.ToChar(((parse_buf[i] & 0x7F)
				  | ( (byte) (parse_buf[i+1] & (byte)(0x7F)) << 7)));
			}
			name[len++] = '-';
			name[len++] = Convert.ToChar(parse_buf[2]+ '0');
			name[len++] = '.';
			name[len++] = Convert.ToChar(parse_buf[3] + '0');
			name[len++] = Convert.ToChar(0);
			firmata_name = new String(name);
			// query the board's capabilities only after hearing the
			// REPORT_FIRMWARE message.  For boards that reset when
			// the port open (eg, Arduino with reset=DTR), they are
			// not ready to communicate for some time, so the only
			// way to reliably query their capabilities is to wait
			// until the REPORT_FIRMWARE message is heard.
			byte[] buf = new byte[80];
			len=0;
			buf[len++] = MSG.START_SYSEX;
			buf[len++] = MSG.ANALOG_MAPPING_QUERY; // read analog to pin # info
			buf[len++] = MSG.END_SYSEX;
			buf[len++] = MSG.START_SYSEX;
			buf[len++] = MSG.CAPABILITY_QUERY; // read capabilities
			buf[len++] = MSG.END_SYSEX;
			for (int i=0; i<16; i++) {
				buf[len++] = (byte) (0xC0 | i);  // report analog
				buf[len++] = 1;
				buf[len++] =(byte) (0xD0 | i);  // report digital
				buf[len++] = 1;
			}
			
            //port.Write(buf, len);

            mSerial.WriteToPort(portName, buf, len);
			//tx_count += len;
		} else if (parse_buf[1] == MSG.CAPABILITY_RESPONSE) {
			int pin, i, n;
			for (pin=0; pin < 128; pin++) {
				pin_info[pin].supported_modes = 0;
			}
			for (i=2, n=0, pin=0; i<parse_count; i++) {
				if (parse_buf[i] == 127) {
					pin++;
					n = 0;
					continue;
				}
				if (n == 0) {
					// first byte is supported mode
					pin_info[pin].supported_modes |= ((ulong) 1<<parse_buf[i]);
				}
				n = n ^ 1;
			}
			// send a state query for for every pin with any modes
			for (pin=0; pin < 128; pin++) {
				byte[] buf = new byte[512];
				int len=0;
				if (pin_info[pin].supported_modes > 0) {
					buf[len++] = MSG.START_SYSEX;
					buf[len++] = MSG.PIN_STATE_QUERY;
					buf[len++] = (byte) pin;
					buf[len++] = MSG.END_SYSEX;
				}
				//port.Write(buf, len);

                mSerial.WriteToPort(portName, buf, len);


				//tx_count += len;
			}
		} else if (parse_buf[1] == MSG.ANALOG_MAPPING_RESPONSE) {
			int pin=0;
			for (int i=2; i<parse_count-1; i++) {
				pin_info[pin].analog_channel = parse_buf[i];
				pin++;
			}
			return;
		} else if (parse_buf[1] == MSG.PIN_STATE_RESPONSE && parse_count >= 6) {
			int pin = parse_buf[2];
			pin_info[pin].mode = (PIN_MODE) parse_buf[3];
			pin_info[pin].value = parse_buf[4];
			if (parse_count > 6) pin_info[pin].value |= (byte)  (parse_buf[5] << 7);
			if (parse_count > 7) pin_info[pin].value |= (byte) (parse_buf[6] << 14);
			//add_pin(pin);
      
            Debug.WriteLine(TAG + "Added Pin! " + pin + " " + pin_info[pin].mode + " " + pin_info[pin].value);
            if (this.OnPinAdded != null)
            {
                this.OnPinAdded(pin_info[pin]);
            }
		}
		return;
	    }
       }


        public void port_DataReceived(string portName, byte[] buffer, int nBytes)
        {
            throw new NotImplementedException();
        }
    }
}
