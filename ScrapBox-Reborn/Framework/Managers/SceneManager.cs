using System.Collections.Generic;
using System.Threading;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.ECS;
using ScrapBox.Framework.Diagnostics;
using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.Managers
{
    public enum DefaultLayers
    {
        BACKGROUND = 0,
        FOREGROUND = 1,
        UI = 2
    }

    public static class SceneManager
    {
        public static Scene CurrentScene;

        public static DateTime Started;

        internal static Dictionary<string, Scene> Scenes;

        internal static bool Debug;
        internal static SpriteFont DebugFont;

        internal static bool currentSceneBusy;
        internal static bool swappingScene;

        static SceneManager()
        {
            Started = DateTime.Now;

            Scenes = new Dictionary<string, Scene>();
        }

        public static void RegisterScene(string name, Scene scene)
        {
            Scenes.Add(name, scene);
        }

        public static void SwapScene(string name, params object[] args)
        {
            Thread t = new Thread(delegate ()
            {
                while (currentSceneBusy)
                {
                    Thread.Sleep(1);
                }

                Scene tempScene;
                bool success = Scenes.TryGetValue(name, out tempScene);
                if (!success)
                {
                    LogService.Log("WorldManager", "SwapScene", $"Scene \"{name}\" does not exist.", Severity.ERROR);
                    return;
                }

                swappingScene = true;
                if (CurrentScene != null)
                {
                    CurrentScene.Unload();
                    CurrentScene.UnloadAssets();
                }

                CurrentScene = tempScene;

                CurrentScene.Initialize();
                CurrentScene.LoadAssets();
                CurrentScene.Load(args);

                swappingScene = false;
            });

            t.Start();

        }

        public static void EnableDebug(SpriteFont debugFont)
        {
            Debug = true;
            DebugFont = debugFont;

            RenderDiagnostics.Font = DebugFont;
            PhysicsDiagnostics.Font = DebugFont;
        }


        internal static void Update(double dt)
        {
            if (CurrentScene == null || swappingScene)
                return;

            currentSceneBusy = true;

            CurrentScene.PreStackTick(dt);
            CurrentScene.PostStackTick(dt);
        }

        internal static void Draw(double dt)
        {
            if (CurrentScene == null || swappingScene)
                return;

            Renderer.BeginSceneRender();

            CurrentScene.PreStackRender();
            CurrentScene.PostStackRender();

            Renderer.EndSceneRender();

            if (Debug)
            {
                RenderDiagnostics.Draw(new ScrapVector(5, 5));
                PhysicsDiagnostics.Draw(new ScrapVector(5, 85));
            }

            RenderDiagnostics.Calls = 0;
            PhysicsDiagnostics.FPS = ScrapMath.Round(1 / dt);
            currentSceneBusy = false;
        }
    }
}
