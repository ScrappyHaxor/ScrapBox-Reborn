using System.Collections.Generic;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.ECS
{
    public abstract class EntityCollection
    {
        public abstract List<Entity> Register { get; set; }

        protected readonly Layer layer;

        protected EntityCollection(Layer layer)
        {
            this.layer = layer;

            Register = new List<Entity>();
        }

        public virtual void Awake()
        {
            layer.RegisterEntityCollection(this);

            foreach (Entity e in Register)
            {
                e.Awake();
            }
        }

        public virtual void Sleep()
        {
            layer.PurgeEntityCollection(this);

            foreach (Entity e in Register)
            {
                e.Sleep();
            }
        }

        public virtual void PreLayerTick(double dt)
        {

        }

        public virtual void PostLayerTick(double dt)
        {

        }

        public virtual void PreLayerRender(Camera mainCamera)
        {

        }

        public virtual void PostLayerRender(Camera mainCamera)
        {

        }
    }
}
