using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using DefaultModules.KeypressHelpers;
using System.Diagnostics;

namespace DefaultModules.Actions
{
    // Because of it's support for shifts and stuff, it won't work in CLI mode.

    public class Keypress : ActionBase
    {
        public const string TAG = "[Key Press]";

        private HashSet<System.Windows.Forms.Keys> monitor_keys_down = null;

        private HashSet<System.Windows.Forms.Keys> keys_down = new HashSet<System.Windows.Forms.Keys>();
        private InterceptKeys.KeyDownHandler keyDownHandler = null;
        private InterceptKeys.KeyUpHandler keyUpHandler = null;

        public Keypress()
            : base("Key press", "This trigger fires on a predefined key press") {
            hasConfig = true;

            keyDownHandler = new InterceptKeys.KeyDownHandler(Intercept_key_down);
            keyUpHandler = new InterceptKeys.KeyUpHandler(Intercept_key_up);
        }

        private void Intercept_key_down(object sender, System.Windows.Forms.KeyEventArgs e) {
            keys_down.Add(e.KeyCode);

            if (Keysets_Equal() && Enabled) {
                OnActionActivated();
            }

        }

        private bool Keysets_Equal() {
            if (keys_down.Count == monitor_keys_down.Count) {
                foreach (System.Windows.Forms.Keys k in monitor_keys_down) {
                    bool foundEqiv = false;
                    foreach (System.Windows.Forms.Keys l in keys_down) {
                        if (l == k) { foundEqiv = true; break; }
                    }

                    if (foundEqiv == false)
                        return false;
                }
                return true;
            } else {
                return false;
            }
        }

        private void Intercept_key_up(object sender, System.Windows.Forms.KeyEventArgs e) {
            keys_down.Remove(e.KeyCode);
        }
    }
}
