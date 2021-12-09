using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ScrapBox.Framework.Managers
{
    public static class AssetManager
    {
        private const string TEXTURE2D_NAME = "Texture2D";
        private const string FONT_NAME = "Font";
        
        private readonly static Dictionary<string, Texture2D> textureRegister;
        private readonly static Dictionary<string, SpriteFont> fontRegister;

        static AssetManager()
        {
            textureRegister = new Dictionary<string, Texture2D>();
            fontRegister = new Dictionary<string, SpriteFont>();
        }

        public static void LoadResourceFile(string name, ContentManager content)
        {
            LogManager.Log(new LogMessage("AssetManager", $"Begun loading resource file {name}.sResource.", LogMessage.Severity.VERBOSE));

             if (!File.Exists($"{name}.sResource"))
             {
                 LogManager.Log(new LogMessage("AssetManager", $"Resource file at: {name}.sResource could not be found.", 
                             LogMessage.Severity.CRITICAL));
                 return;
             }

             int entryNumber = 0;
             string[] buffer = File.ReadAllLines($"{name}.sResource");
             foreach (string line in buffer)
             {
                 entryNumber++;
                 string[] args = line.Split(":");
                 if (args.Length > 2)
                 {
                     LogManager.Log(new LogMessage("AssetManager", $"Error in resource file {name}.sResource at line {entryNumber}. Skipping...", 
                                 LogMessage.Severity.WARNING));
                     continue;
                 }

                 if (args[1] == TEXTURE2D_NAME)
                 {
                     textureRegister.Add(args[0], content.Load<Texture2D>(args[0]));
                     LogManager.Log(new LogMessage("AssetManager", $"Loaded texture {args[0]} successfully.", LogMessage.Severity.VERBOSE));
                 }
                 else if (args[1] == FONT_NAME)
                 {
                     fontRegister.Add(args[0], content.Load<SpriteFont>(args[0]));
                     LogManager.Log(new LogMessage("AssetManager", $"Loaded font {args[0]} successfully.", LogMessage.Severity.VERBOSE));
                 }
                 else
                 {
                     LogManager.Log(new LogMessage("AssetManager", $"Error in resource file {name}.sResource at line {entryNumber}. Skipping...",
                                LogMessage.Severity.WARNING));
                     continue;
                 }
             } 
             
             LogManager.Log(new LogMessage("AssetManager", $"Finished loading resource file {name}.sResource. {entryNumber} lines loaded.", LogMessage.Severity.VERBOSE));
        }

        public static Texture2D FetchTexture(string name)
        {
            if (!textureRegister.ContainsKey(name))
            {
                LogManager.Log(new LogMessage("AssetManager", $"Tried to fetch texture that doesn't exist. ({name})", LogMessage.Severity.WARNING));
                return default;
            }

            return textureRegister.GetValueOrDefault(name);
        }

        public static SpriteFont FetchFont(string name)
        {
            if (!fontRegister.ContainsKey(name))
            {
                LogManager.Log(new LogMessage("AssetManager", $"Tried to fetch font that doesn't exist. ({name})", LogMessage.Severity.WARNING));
                return default;
            }

            return fontRegister.GetValueOrDefault(name);
        }
    }
}
