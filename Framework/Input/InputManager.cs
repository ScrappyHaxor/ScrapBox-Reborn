using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.Input
{
    public static class InputManager
    {
        internal static bool[] keyRegister;
        internal static bool[] buttonRegister;

        static InputManager()
        {
            //Register every key in the Keys enum and assign a bool to it.
            keyRegister = new bool[255];
            for (int i = 0; i < 255; i++)
            {
                keyRegister[i] = false;
            }

            buttonRegister = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                buttonRegister[i] = false;
            }
        }

        #region Keyboard
        /// <summary>
        /// Get all keys that are down on the keyboard EXCLUDING the ones already held down, mainly used for user input
        /// </summary>
        /// <returns>A list of keys that have just been pressed</returns>
        public static List<Keys> GetKeysDown()
        {
            List<Keys> returnKeys = new List<Keys>();

            KeyboardState keyState = Keyboard.GetState();
            foreach (Keys key in keyState.GetPressedKeys())
            {
                //Ignore already down keys
                if (IsKeyAlreadyDown(key)) continue;
                keyRegister[(int)key] = true;
                returnKeys.Add(key);
            }

            return returnKeys;
        }

        /// <summary>
        /// Check if the key is pressed and NOT held down
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if it was just pressed otherwise false</returns>
        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key) && !IsKeyAlreadyDown(key))
            {
                keyRegister[(int)key] = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a key is already being held down, mostly used internally
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if it is already down otherwise false</returns>
        public static bool IsKeyAlreadyDown(Keys key)
        {
            return keyRegister[(int)key];
        }

        /// <summary>
        /// Check if the key is held down, standard monogame implementation
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if its held down otherwise false</returns>
        public static bool IsKeyHeld(Keys key)
        {
            KeyboardState keyState = Keyboard.GetState();
            return keyState.IsKeyDown((Microsoft.Xna.Framework.Input.Keys)key);
        }
        #endregion

        #region Mouse
        public static ScrapVector GetMousePosition()
        {
            MouseState state = Mouse.GetState();

            return new ScrapVector(state.X, state.Y);
        }

        public static ScrapVector GetMouseWorldPosition(Camera camera)
        {
            ScrapVector windowPosition = GetMousePosition();

            Matrix inverseTransformMatrix = Matrix.Invert(camera.TransformationMatrix);
            return ScrapVector.Transform(windowPosition, inverseTransformMatrix);
        }

        public static bool IsButtonDown(Button button)
        {
            if (IsButtonHeld(button) && !IsButtonAlreadyDown(button))
            {
                buttonRegister[(int)button] = true;
                return true;
            }

            return false;
        }

        public static bool IsButtonHeld(Button button)
        {
            MouseState state = Mouse.GetState();
            return button switch
            {
                Button.LEFT_MOUSE_BUTTON => state.LeftButton == ButtonState.Pressed,
                Button.MIDDLE_MOUSE_BUTTON => state.MiddleButton == ButtonState.Pressed,
                Button.RIGHT_MOUSE_BUTTON => state.RightButton == ButtonState.Pressed,
                _ => false,
            };
        }

        public static bool IsButtonAlreadyDown(Button button)
        {
            return buttonRegister[(int)button];
        }

        public static List<Button> GetButtonsDown()
        {
            List<Button> returnButtons = new List<Button>();

            for (int i = 0; i < buttonRegister.Length; i++)
            {
                if (IsButtonAlreadyDown((Button)i)) continue;

                if (IsButtonDown((Button)i))
                {
                    buttonRegister[i] = true;
                    returnButtons.Add((Button)i);
                }
            }

            return returnButtons;
        }
        #endregion

        /// <summary>
        /// Resets the state of all keys
        /// </summary>
        internal static void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            for (int i = 0; i < keyRegister.Length; i++)
            {
                if (keyRegister[i] == false) continue;

                if (keyState.IsKeyUp((Microsoft.Xna.Framework.Input.Keys)i))
                {
                    keyRegister[i] = false;
                }
            }

            for (int i = 0; i < buttonRegister.Length; i++)
            {
                if (buttonRegister[i] == false) continue;

                if (!IsButtonHeld((Button)i))
                {
                    buttonRegister[i] = false;
                }
            }
        }
    }
}
