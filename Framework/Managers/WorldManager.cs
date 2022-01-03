using System.Collections.Generic;
using System.Threading;

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
    public static class WorldManager
    {
        public static Color BackColor;
        public static Scene CurrentScene;

        internal static Dictionary<string, Scene> Scenes;
        internal static List<ComponentSystem> Systems;
        internal static List<Entity> Entities;

        internal static bool Debug;
        internal static SpriteFont DebugFont;

        internal static bool currentSceneBusy;
        internal static bool swappingScene;

        static WorldManager()
        {
            BackColor = Color.Black;

            Scenes = new Dictionary<string, Scene>();
            Systems = new List<ComponentSystem>();

            SpriteSystem spriteSystem = new SpriteSystem();
            RegisterSystem(spriteSystem);

            PhysicsSystem physicsSystem = new PhysicsSystem();
            RegisterSystem(physicsSystem);

            CollisionSystem collisionSystem = new CollisionSystem();
            RegisterSystem(collisionSystem);

            ParticleSystem particleSystem = new ParticleSystem();
            RegisterSystem(particleSystem);

            InterfaceSystem interfaceSystem = new InterfaceSystem();
            RegisterSystem(interfaceSystem);

            Entities = new List<Entity>();
        }

        public static void RegisterScene(string name, Scene scene)
        {
            Scenes.Add(name, scene);
        }

        public static void SwapScene(string name)
        {
            Thread t = new Thread(delegate ()
            {
                while (currentSceneBusy)
                {
                    Thread.Sleep(1);
                }

                swappingScene = true;
                if (CurrentScene != null)
                {
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        Entity e = Entities[i];
                        e.Sleep();
                    }
                    Entities.Clear();

                    foreach (ComponentSystem system in Systems)
                    {
                        system.Reset();
                    }

                    CurrentScene.Unload();
                    CurrentScene.UnloadAssets();
                }

                bool success = Scenes.TryGetValue(name, out CurrentScene);
                if (!success)
                {
                    LogService.Log("WorldManager", "SwapScene", $"Scene \"{name}\" does not exist.", Severity.ERROR);
                    return;
                }

                CurrentScene.Initialize();
                CurrentScene.LoadAssets();
                CurrentScene.Load();

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

        public static void RegisterSystem<T>(T system) where T : ComponentSystem
        {
            Systems.Add(system);
        }

        public static void PurgeSystem<T>(T system) where T : ComponentSystem
        {
            Systems.Remove(system);
        }

        public static bool HasSystem<T>() where T : ComponentSystem
        {
            foreach (ComponentSystem system in Systems)
            {
                if (system.GetType().Equals(typeof(T)) || system.GetType().IsSubclassOf(typeof(T)))
                    return true;
            }

            return false;
        }

        public static ComponentSystem GetSystem<T>() where T : ComponentSystem
        {
            foreach (ComponentSystem system in Systems)
            {
                if (system.GetType().Equals(typeof(T)) || system.GetType().IsSubclassOf(typeof(T)))
                    return system;
            }

            LogService.Log("WorldManager", "GetSystem", "Tried to get non-existant system.", Severity.WARNING);
            return default;
        }

        public static void RegisterEntity<T>(T entity) where T : Entity
        {
            Entities.Add(entity);
        }

        public static void PurgeEntity<T>(T entity) where T : Entity
        {
            Entities.Remove(entity);
        }

        public static bool HasEntity<T>() where T : Entity
        {
            foreach (Entity entity in Entities)
            {
                if (entity.GetType().Equals(typeof(T)) || entity.GetType().IsSubclassOf(typeof(T)))
                    return true;
            }

            return false;
        }

        public static Entity GetEntity<T>() where T : Entity
        {
            foreach (Entity entity in Entities)
            {
                if (entity.GetType().Equals(typeof(T)) || entity.GetType().IsSubclassOf(typeof(T)))
                    return entity;
            }

            LogService.Log("WorldManager", "GetEntity", "Tried to get non-existant entity.", Severity.WARNING);
            return default;
        }

        internal static void Update(double dt)
        {
            if (CurrentScene == null || swappingScene)
                return;

            currentSceneBusy = true;

            foreach (ComponentSystem system in Systems)
            {
                system.Update(dt);
            }

            foreach (Entity entity in Entities)
            {
                entity.Update(dt);
            }

            CurrentScene.Update(dt);
        }

        internal static void Draw()
        {
            if (CurrentScene == null || swappingScene)
                return;

            foreach (ComponentSystem system in Systems)
            {
                system.Draw(CurrentScene.MainCamera);
            }

            foreach (Entity entity in Entities)
            {
                entity.Draw(CurrentScene.MainCamera);
            }

            if (Debug)
            {
                RenderDiagnostics.Draw(new ScrapVector(5, 5));
                PhysicsDiagnostics.Draw(new ScrapVector(5, 85));
            }

            CurrentScene.Draw();

            currentSceneBusy = false;
        }
    }
}
