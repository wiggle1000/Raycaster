using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal static class InputManager
    {

        public static KeyboardState ks;
        public static KeyboardState prevKS;

        public static MouseState ms;
        public static MouseState prevMS;

        public static int MouseX = 0;
        public static int MouseY = 0;
        public static Point MousePos = Point.Zero;
        public static float ScrollDelta = 0;

        public static bool KeyJustPressed(Keys key)
        {
            return ks.IsKeyDown(key) && !prevKS.IsKeyDown(key);
        }
        public static bool KeyJustReleased(Keys key)
        {
            return !ks.IsKeyDown(key) && prevKS.IsKeyDown(key);
        }
        public static bool KeyDown(Keys key)
        {
            return ks.IsKeyDown(key);
        }
        public static bool ShiftDown()
        {
            return KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift);
        }
        public static bool CtrlDown()
        {
            return KeyDown(Keys.LeftControl) || KeyDown(Keys.RightControl);
        }
        public static bool AltDown()
        {
            return KeyDown(Keys.LeftAlt) || KeyDown(Keys.RightAlt);
        }

        private static bool _getButtonState(MouseState ms, MouseButton button)
        {
            if (!Game.instance.IsActive) return false;
            switch (button)
            {
                case MouseButton.Left:
                    return ms.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return ms.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return ms.MiddleButton == ButtonState.Pressed;
                case MouseButton.Forward:
                    return ms.XButton1 == ButtonState.Pressed;
                case MouseButton.Back:
                    return ms.XButton2 == ButtonState.Pressed;
            }
            return false;
        }

        public static bool IsMouseJustPressed(MouseButton button)
        {
            return _getButtonState(ms, button) && !_getButtonState(prevMS, button);
        }
        public static bool IsMouseJustReleased(MouseButton button)
        {
            return !_getButtonState(ms, button) && _getButtonState(prevMS, button);
        }
        public static bool IsMouseDown(MouseButton button)
        {
            return _getButtonState(ms, button);
        }

        public static void Update(float dt)
        {
            prevKS = ks;
            prevMS = ms;
            ks = Keyboard.GetState();
            ms = Mouse.GetState();


            if (!Game.instance.IsActive) return;
            MouseX = ms.X;
            MouseY = ms.Y;
            MousePos = ms.Position;
            ScrollDelta = (ms.ScrollWheelValue - prevMS.ScrollWheelValue)/120f;

        }
    }

    public enum MouseButton
    {
        Left = 0, Right = 1, Middle = 2, Forward = 3, Back = 4
    }
}
