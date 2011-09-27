using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace XboxModules
{
    public static class ButtonExtensions
    {
        public static Buttons AsButtons(this GamePadButtons buttons)
        {
            Buttons original = new Buttons();
            
            if (buttons.A == ButtonState.Pressed)
                original = original | Buttons.A;
            if (buttons.B == ButtonState.Pressed)
                original = original | Buttons.B;
            if (buttons.Back == ButtonState.Pressed)
                original = original | Buttons.Back;
            if (buttons.BigButton == ButtonState.Pressed)
                original = original | Buttons.BigButton;
            if (buttons.LeftShoulder == ButtonState.Pressed)
                original = original | Buttons.LeftShoulder;
            if (buttons.LeftStick == ButtonState.Pressed)
                original = original | Buttons.LeftStick;
            if (buttons.RightShoulder == ButtonState.Pressed)
                original = original | Buttons.RightShoulder;
            if (buttons.RightStick == ButtonState.Pressed)
                original = original | Buttons.RightStick;
            if (buttons.Start == ButtonState.Pressed)
                original = original | Buttons.Start;
            if (buttons.X == ButtonState.Pressed)
                original = original | Buttons.X;
            if (buttons.Y == ButtonState.Pressed)
                original = original | Buttons.Y;

            return original;
        }

        public static string ButtonString(this Buttons buttons)
        {
            List<string> list = new List<string>();

            if(buttons.HasFlag(Buttons.A)) 
                list.Add("A");
            if(buttons.HasFlag(Buttons.B))
                list.Add("B");
            if(buttons.HasFlag(Buttons.X))
                list.Add("X");
            if(buttons.HasFlag(Buttons.Y))
                list.Add("Y");
            if (buttons.HasFlag(Buttons.Back))
                list.Add("Back");
            if (buttons.HasFlag(Buttons.Start))
                list.Add("Start");
            if (buttons.HasFlag(Buttons.BigButton))
                list.Add("Big Button");
            if (buttons.HasFlag(Buttons.RightShoulder))
                list.Add("Right Shoulder");
            if (buttons.HasFlag(Buttons.LeftShoulder))
                list.Add("Left Shoulder");
            if (buttons.HasFlag(Buttons.RightStick))
                list.Add("Right Stick");
            if (buttons.HasFlag(Buttons.LeftStick))
                list.Add("Left Stick");

            return string.Join(", ", list);
        }
    }
}
