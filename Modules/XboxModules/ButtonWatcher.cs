using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace XboxModules
{
    class ButtonWatcher
    {
        public delegate void KeyCombinationHandler();
        private Dictionary<Buttons, List<KeyCombinationHandler>> keyCombinationHandlerMap;

        private Buttons buttons_down;

        private ButtonEvents events;

        #region Singleton

        private static ButtonWatcher _instance;

        public static ButtonWatcher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ButtonWatcher();

                return _instance;
            }
        }

        ButtonWatcher()
        {
            keyCombinationHandlerMap = new Dictionary<Buttons, List<KeyCombinationHandler>>();
            events = ButtonEvents.Instance;
            events.OnButtonDown += new ButtonEvents.ButtonDownHandler(events_OnButtonDown);
            events.OnButtonUp += new ButtonEvents.ButtonUpHandler(events_OnButtonUp);
        }

        #endregion

        public void AddCombinationHandler(Buttons buttons, KeyCombinationHandler handler)
        {
            List<KeyCombinationHandler> listHandlers = null;
            foreach (Buttons b in keyCombinationHandlerMap.Keys)
            {
                if (b.Equals(buttons))
                {
                    listHandlers = keyCombinationHandlerMap[b];
                    break;
                }
            }
            if (listHandlers == null)
            {
                listHandlers = new List<KeyCombinationHandler>();
                keyCombinationHandlerMap[buttons] = listHandlers;
            }
            if (!listHandlers.Contains(handler))
            {
                listHandlers.Add(handler);
            }

            events.AddRef();
        }

        public void RemoveCombinationHandler(Buttons buttons, KeyCombinationHandler handler)
        {
            foreach (Buttons b in keyCombinationHandlerMap.Keys)
            {
                if (b.Equals(buttons))
                {
                    keyCombinationHandlerMap[b].Remove(handler);
                    break;
                }
            }

            events.RemoveRef();
        }

        private void CheckCombinations()
        {
            foreach (Buttons b in keyCombinationHandlerMap.Keys)
            {
                if (b.Equals(buttons_down))
                {
                    List<KeyCombinationHandler> listHandlers = keyCombinationHandlerMap[b];
                    for (int i = 0; i < listHandlers.Count; i++)
                    {
                        listHandlers[i]();
                    }
                    break;
                }
            }
        }

        private void events_OnButtonDown(Buttons button)
        {
            if (!buttons_down.HasFlag(button))
            {
                buttons_down |= button;
                CheckCombinations();
            }
        }

        private void events_OnButtonUp(Buttons button)
        {
            // remove button from buttons_down
            buttons_down &= ~button;            
        }
    }
}
