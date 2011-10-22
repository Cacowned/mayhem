using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Firmata
{
    public struct FirmataMsg
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
}
