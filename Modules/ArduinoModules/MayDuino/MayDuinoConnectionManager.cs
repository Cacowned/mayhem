using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArduinoModules.Events;

namespace ArduinoModules
{
    public class MayDuinoConnectionManager
    {

        private MayduinoEventBase last_event;
        private MayduinoReactionBase last_reaction;

        public void EventEnabled(MayduinoEventBase e)
        {
            if (last_reaction == null)
            {
                last_event= e;
                if (last_reaction != null)
                    AddConnection();
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        public void ReactionEnabled(MayduinoReactionBase r)
        {
            if (last_reaction == null)
            {
                last_reaction = r;
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
            mayduinoConnections[last_event] = last_reaction;
        }

        public Dictionary<MayduinoEventBase, MayduinoReactionBase> mayduinoConnections;

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
