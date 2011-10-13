﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArduinoModules.Events;
using MayhemCore;
using MayhemSerial;

namespace ArduinoModules
{
    public class MayDuinoConnectionManager
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

            // TODO: Write Stuff to the Arduino Board 
            Dictionary<string,string> portnames = serial.GetArduinoPortNames();
            if (portnames.Count > 0)
            {

                string portName =  portnames.Keys.First();
                List<string> configurationstrings = new List<string>();

                foreach (KeyValuePair<MayduinoEventBase, MayduinoReactionBase> connection in mayduinoConnections)
                {
                    MayduinoEventBase mEvent = connection.Key;
                    MayduinoReactionBase mReact = connection.Value;

                    //int ePin = mEvent.

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

    }
}
