using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using Microsoft.Xna.Framework.Audio;

namespace ScrapBox.Framework.Managers
{
    public static class AssetManager
    {
        private const string TEXTURE2D_NAME = "Texture2D";
        private const string FONT_NAME = "Font";
        private const string SHADER_NAME = "Shader";
        private const string SOUND_NAME = "Audio";

        private static string[] internalResourceFile;

        private readonly static Dictionary<string, Texture2D> textureRegister;
        private readonly static Dictionary<string, SpriteFont> fontRegister;
        private readonly static Dictionary<string, Effect> shaderRegister;
        private readonly static Dictionary<string, SoundEffect> soundRegister;

        static AssetManager()
        {
            internalResourceFile = new string[]
            {
                "internal:Font",
                "lights:Shader"
            };

            textureRegister = new Dictionary<string, Texture2D>();
            fontRegister = new Dictionary<string, SpriteFont>();
            shaderRegister = new Dictionary<string, Effect>();
            soundRegister = new Dictionary<string, SoundEffect>();
        }

        private static bool ParseArgs(string[] args, ContentManager content)
        {
            if (args[1] == TEXTURE2D_NAME)
            {
                return LoadTexture(args[0], content);
            }
            else if (args[1] == FONT_NAME)
            {
                return LoadFont(args[0], content);
            }
            else if (args[1] == SHADER_NAME)
            {
                return LoadShader(args[0], content);
            }
            else if (args[1] == SOUND_NAME)
            {
                return LoadAudio(args[0], content);
            }

            LogService.Log("AssetManager", "ParseArgs", "Unknown resource type.", Severity.ERROR);
            return false;
        }

        internal static void LoadInternalAssets(ContentManager content)
        {
            foreach (string line in internalResourceFile)
            {
                string[] args = line.Split(":");
                ParseArgs(args, content);
            }
        }

        public static void AddSimpleTexture(string name, int width, int height, GraphicsDevice device, ContentManager content)
        {
            Texture2D newTex = new Texture2D(device, width, height);

            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Color.White;
            }

            newTex.SetData(data);
            textureRegister.Add(name, newTex);
        }

        public static void LoadResourceFile(string name, ContentManager content)
        {
            LogService.Log("AssetManager", "LoadResourceFile", $"Loading resource file {name}", Severity.INFO);

            if (!File.Exists($"{name}.sResource"))
             {
                 LogService.Log("AssetManager", "LoadResourceFile", $"Resource file: {name}.sResource could not be found.", 
                             Severity.CRITICAL);
                 return;
             }

             int entryNumber = 0;
             string[] buffer = File.ReadAllLines($"{name}.sResource");
             foreach (string line in buffer)
             {
                entryNumber++;
                string[] args = line.Split(":");
                bool success = ParseArgs(args, content);
                if (!success)
                {
                    LogService.Log("AssetManager", "LoadResourceFile", $"Error in resource file {name} at line {entryNumber}. Skipping...",
                        Severity.WARNING);
                    continue;
                }
             } 
             
             LogService.Log("AssetManager", "LoadResourceFile", $"Finished loading resource file {name}. {entryNumber} assets loaded successfully.", Severity.INFO);
        }

        public static bool LoadTexture(string name, ContentManager content)
        {
            try
            {
                Texture2D texture = content.Load<Texture2D>(name);
                textureRegister.Add(name, texture);
                return true;
            }
            catch
            {
                LogService.Log("AssetManager", "LoadTexture", "Texture is not built. Check the Monogame pipeline.", Severity.ERROR);
                return false;
            }

        }

        public static bool LoadFont(string name, ContentManager content)
        {
            try
            {
                SpriteFont font = content.Load<SpriteFont>(name);
                fontRegister.Add(name, font);
                return true;
            }
            catch
            {
                LogService.Log("AssetManager", "LoadFont", "Font is not built. Check the Monogame pipeline.", Severity.ERROR);
                return false;
            }
        }

        public static bool LoadAudio(string name, ContentManager content)
        {
            try
            {
                SoundEffect effect = content.Load<SoundEffect>(name);
                soundRegister.Add(name, effect);
                return true;
            }
            catch
            {
                LogService.Log("AssetManager", "LoadAudio", "Audio is not built. Check the Monogame pipeline.", Severity.ERROR);
                return false;
            }
        }

        public static bool LoadShader(string name, ContentManager content)
        {
            try
            {
                Effect shader = content.Load<Effect>(name);
                shaderRegister.Add(name, shader);
                return true;
            }
            catch
            {
                LogService.Log("AssetManager", "LoadShader", "Shader is not built. Check the Monogame pipeline.", Severity.ERROR);
                return false;
            }

        }

        public static void Unload(ContentManager content)
        {
            textureRegister.Clear();
            fontRegister.Clear();
            shaderRegister.Clear();
            soundRegister.Clear();
            content.Unload();
        }

        public static Texture2D FetchTexture(string name)
        {
            if (!textureRegister.ContainsKey(name))
            {
                LogService.Log("AssetManager", "FetchTexture", $"Tried to fetch texture that doesn't exist. ({name})", Severity.WARNING);
                return default;
            }

            return textureRegister.GetValueOrDefault(name);
        }

        public static SpriteFont FetchFont(string name)
        {
            if (!fontRegister.ContainsKey(name))
            {
                LogService.Log("AssetManager", "FetchFont", $"Tried to fetch font that doesn't exist. ({name})", Severity.WARNING);
                return default;
            }

            return fontRegister.GetValueOrDefault(name);
        }

        public static Effect FetchShader(string name)
        {
            if (!shaderRegister.ContainsKey(name))
            {
                LogService.Log("AssetManager", "FetchShader", $"Tried to fetch shader that doesn't exist. ({name})", Severity.WARNING);
                return default;
            }

            return shaderRegister.GetValueOrDefault(name);
        }

        public static SoundEffect FetchAudio(string name)
        {
            if (!soundRegister.ContainsKey(name))
            {
                LogService.Log("AssetManager", "FetchAudio", $"Tried to fetch audio that doesn't exist. ({name})", Severity.WARNING);
                return default;
            }

            return soundRegister.GetValueOrDefault(name);
        }
    }
}
