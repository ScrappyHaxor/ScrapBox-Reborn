using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ScrapBox.Framework.Managers
{
    public static class InputManager
    {
        //Internal
        private static bool[] keyRegister;


        static InputManager()
        {
            //Register every key in the Keys enum and assign a bool to it.
            keyRegister = new bool[255];
            for (int i = 0; i < 255; i++)
            {
                keyRegister[i] = false;
            }
        }

        /// <summary>
        /// Resets the state of all keys
        /// </summary>
        internal static void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            for (int i = 0; i < keyRegister.Length; i++)
            {
                if (keyRegister[i] == false) continue;

                if (keyState.IsKeyUp((Keys)i))
                {
                    keyRegister[i] = false;
                }
            }
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
        /// Check if the key is held down, standard monogame implementation
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if its held down otherwise false</returns>
        public static bool IsKeyHeld(Keys key)
        {
            KeyboardState keyState = Keyboard.GetState();
            return keyState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if the key is pressed and NOT held down
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if it was just pressed otherwise false</returns>
        public static bool IsKeyDown(Keys key)
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(key) && !IsKeyAlreadyDown(key))
            {
                keyRegister[(int)key] = true;
                return true;
            }

            return false;
        }
    }
}
