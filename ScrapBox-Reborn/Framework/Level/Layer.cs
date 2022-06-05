using Microsoft.Xna.Framework.Graphics;
using ScrapBox.Framework.ECS;
using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Level
{
    public class Layer
    {
        public readonly string Name;

        private readonly List<Entity> entities;
        private readonly List<EntityCollection> collections;
        private readonly List<ComponentSystem> systems;

        private bool active;

        public Layer(string name = "layer")
        {
            this.Name = name;

            entities = new List<Entity>();
            collections = new List<EntityCollection>();
            systems = new List<ComponentSystem>();

            SpriteSystem spriteSystem = new SpriteSystem();
            RegisterSystem(spriteSystem);

            PhysicsSystem physicsSystem = new PhysicsSystem();
            RegisterSystem(physicsSystem);

            CollisionSystem collisionSystem = new CollisionSystem();
            RegisterSystem(collisionSystem);

            //ParticleSystem particleSystem = new ParticleSystem();
            //RegisterSystem(particleSystem);

            InterfaceSystem interfaceSystem = new InterfaceSystem();
            RegisterSystem(interfaceSystem);
        }

        public void RegisterEntity<T>(T entity) where T : Entity
        {
            entities.Add(entity);
        }

        public void PurgeEntity<T>(T entity) where T : Entity
        {
            entities.Remove(entity);
        }

        public void RegisterEntityCollection<T>(T collection) where T : EntityCollection
        {
            collections.Add(collection);
        }

        public void PurgeEntityCollection<T>(T collection) where T : EntityCollection
        {
            collections.Remove(collection);
        }

        public bool HasEntity<T>() where T : Entity
        {
            foreach (Entity entity in entities)
            {
                if (entity.GetType().Equals(typeof(T)) || entity.GetType().IsSubclassOf(typeof(T)))
                    return true;
            }

            return false;
        }

        public T GetEntity<T>() where T :Entity
        {
            foreach (Entity entity in entities)
            {
                if (entity.GetType().Equals(typeof(T)) || entity.GetType().IsSubclassOf(typeof(T)))
                    return (T)entity;
            }

            LogService.Log("Layer", "GetEntity", "Tried to get non-existant entity.", Severity.WARNING);
            return default;
        }

        public void RegisterSystem<T>(T system) where T : ComponentSystem
        {
            systems.Add(system);
        }

        public void PurgeSystem<T>(T system) where T : ComponentSystem
        {
            systems.Remove(system);
        }

        public bool HasSystem<T>() where T : ComponentSystem
        {
            foreach (ComponentSystem system in systems)
            {
                if (system.GetType().Equals(typeof(T)) || system.GetType().IsSubclassOf(typeof(T)))
                    return true;
            }

            return false;
        }

        public T GetSystem<T>() where T : ComponentSystem
        {
            foreach (ComponentSystem system in systems)
            {
                if (system.GetType().Equals(typeof(T)) || system.GetType().IsSubclassOf(typeof(T)))
                    return (T)system;
            }

            LogService.Log("WorldManager", "GetSystem", "Tried to get non-existant system.", Severity.WARNING);
            return default;
        }

        internal void Purge()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity e = entities[i];
                if (e == null)
                    continue;

                e.Sleep();
            }

            entities.Clear();

            for (int i = 0; i < collections.Count; i++)
            {
                EntityCollection c = collections[i];
                if (c == null)
                    continue;

                c.Sleep();
            }

            collections.Clear();
        }

        internal void Reset()
        {
            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Reset();
            }
        }

        internal void Update(double dt)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PreLayerTick(dt);
            }

            for (int i = 0; i < collections.Count; i++)
            {
                collections[i].PreLayerTick(dt);
            }

            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Tick(dt);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PostLayerTick(dt);
            }

            for (int i = 0; i < collections.Count; i++)
            {
                collections[i].PostLayerTick(dt);
            }
        }

        internal void Render(Camera camera)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PreLayerRender(camera);
            }

            for (int i = 0; i < collections.Count; i++)
            {
                collections[i].PreLayerRender(camera);
            }

            for (int i = 0; i < systems.Count; i++)
            {
                systems[i].Render(camera);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].PostLayerRender(camera);
            }

            for (int i = 0; i < collections.Count; i++)
            {
                collections[i].PostLayerRender(camera);
            }
        }
    }
}
