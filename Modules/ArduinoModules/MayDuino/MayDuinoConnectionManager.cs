using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArduinoModules.Events;
using MayhemCore;
using MayhemSerial;
using ArduinoModules.Firmata;
using ArduinoModules.MayDuino;

namespace ArduinoModules
{
    public class MayDuinoConnectionManager : ISerialPortDataListener
    {

        private MayduinoEventBase last_event;
        private MayduinoReactionBase last_reaction;
        private MayhemSerialPortMgr serial = MayhemSerialPortMgr.Instance;

        private Dictionary<MayduinoEventBase, MayduinoReactionBase> mayduinoConnections;

        public void EventEnabled(MayduinoEventBase e)
        {
            if (last_reaction == null)
            {
                lock (this)
                {
                    last_event = e;
                }
                if (last_reaction != null)
                    AddConnection();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void EventDisabled(MayduinoEventBase e)
        {
            Logger.WriteLine("EventDisabled -- removing connection"); 
            mayduinoConnections.Remove(e);
        }


        public void ReactionEnabled(MayduinoReactionBase r)
        {
          
            if (last_reaction == null)
            {
                lock (this)
                {
                    last_reaction = r;
                }
                if (last_event != null)
                    AddConnection();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Add a "connection"
        /// </summary>
        private void AddConnection()
        {
            lock (this)
            {
                mayduinoConnections[last_event] = last_reaction;
                last_event = null;
                last_reaction = null;
            }

            Logger.WriteLine("Connection Added");
            UpdateDevice();
        }

        private void UpdateDevice()
        {
            Logger.WriteLine("");
            // TODO: Write Stuff to the Arduino Board 
            Dictionary<string, string> portnames = serial.GetArduinoPortNames();
            if (portnames.Count > 0)
            {
                string portName = portnames.Keys.First();
                if (portName != null)
                {
                    // write out configuration string
                    if (serial.ConnectPort(portName, this, new ArduinoFirmataSerialSettings()))
                    {
                        // reset current connection settings
                        serial.WriteToPort(portName, MayduinoCommands.RESET_CONNECTIONS);

                        List<string> configurationstrings = new List<string>();
                        foreach (KeyValuePair<MayduinoEventBase, MayduinoReactionBase> connection in mayduinoConnections)
                        {
                            MayduinoEventBase mEvent = connection.Key;
                            MayduinoReactionBase mReact = connection.Value;
                            string configString = MayduinoCommands.NEW_CONNECTION + mEvent.EventConfigString + "," + mReact.ReactionConfigString;                          
                            configurationstrings.Add(configString);
                        }

                        // write out the new settings
                        foreach (string s in configurationstrings)
                        {
                            Logger.WriteLine("Writing config to device: " + s);
                            serial.WriteToPort(portName, s);
                        }

                        // tell arduino to save to eeprom
                        serial.WriteToPort(portName, MayduinoCommands.WRITE_CONNECTIONS);
                    }
                }
            }
        }

       

        public MayDuinoConnectionManager()
        {
            mayduinoConnections = new Dictionary<MayduinoEventBase, MayduinoReactionBase>();
        }



        public static MayDuinoConnectionManager instance_;
        public static MayDuinoConnectionManager Instance
        {
            get {
                if (instance_ == null)
                    instance_ = new MayDuinoConnectionManager();

                return instance_;
            }
        }


        public void port_DataReceived(string portName, byte[] buffer, int nBytes)
        {
            //throw new NotImplementedException();
        }
    }
}
