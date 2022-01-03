using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.Managers
{
    public static class AssetManager
    {
        private const string TEXTURE2D_NAME = "Texture2D";
        private const string FONT_NAME = "Font";
        private const string SHADER_NAME = "Shader";
        
        private readonly static Dictionary<string, Texture2D> textureRegister;
        private readonly static Dictionary<string, SpriteFont> fontRegister;
        private readonly static Dictionary<string, Effect> shaderRegister;

        static AssetManager()
        {
            textureRegister = new Dictionary<string, Texture2D>();
            fontRegister = new Dictionary<string, SpriteFont>();
            shaderRegister = new Dictionary<string, Effect>();
        }

        public static void LoadResourceFile(string name, ContentManager content)
        {
            LogService.Log("AssetManager", "LoadResourceFile", $"Begun loading resource file {name}.sResource.", Severity.INFO);

             if (!File.Exists($"{name}.sResource"))
             {
                 LogService.Log("AssetManager", "LoadResourceFile", $"Resource file at: {name}.sResource could not be found.", 
                             Severity.CRITICAL);
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
                     LogService.Log("AssetManager", "LoadResourceFile", $"Error in resource file {name}.sResource at line {entryNumber}. Skipping...", 
                                 Severity.WARNING);
                     continue;
                 }

                 if (args[1] == TEXTURE2D_NAME)
                 {
                     textureRegister.Add(args[0], content.Load<Texture2D>(args[0]));
                     LogService.Log("AssetManager", "LoadResourceFile", $"Loaded texture {args[0]} successfully.", Severity.INFO);
                 }
                 else if (args[1] == FONT_NAME)
                 {
                     fontRegister.Add(args[0], content.Load<SpriteFont>(args[0]));
                     LogService.Log("AssetManager", "LoadResourceFile", $"Loaded font {args[0]} successfully.", Severity.INFO);
                 }
                 else if (args[1] == SHADER_NAME)
                 {
                    shaderRegister.Add(args[0], content.Load<Effect>(args[0]));
                    LogService.Log("AssetManager", "LoadResourceFile", $"Loaded shader {args[0]} successfully.", Severity.INFO);
                 }
                 else
                 {
                     LogService.Log("AssetManager", "LoadResourceFile", $"Error in resource file {name}.sResource at line {entryNumber}. Skipping...",
                                Severity.WARNING);
                     continue;
                 }
             } 
             
             LogService.Log("AssetManager", "LoadResourceFile", $"Finished loading resource file {name}.sResource. {entryNumber} lines loaded.", Severity.INFO);
        }

        public static void Unload(ContentManager content)
        {
            textureRegister.Clear();
            fontRegister.Clear();
            shaderRegister.Clear();
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
    }
}
