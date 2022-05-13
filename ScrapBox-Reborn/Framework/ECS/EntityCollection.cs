using System.Collections.Generic;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.ECS
{
    public abstract class EntityCollection
    {
        public abstract List<Entity> Register { get; set; }

        public bool IsAwake { get; set; }

        protected Layer layer;

        protected EntityCollection(Layer layer)
        {
            this.layer = layer;

            Register = new List<Entity>();
        }

        public virtual void Awake()
        {
            if (IsAwake)
                return;

            foreach (Entity e in Register)
            {
                e.Awake();
            }

            layer.RegisterEntityCollection(this);

            IsAwake = true;
        }

        public virtual void Sleep()
        {
            if (!IsAwake)
                return;

            foreach (Entity e in Register)
            {
                e.Sleep();
            }

            layer.PurgeEntityCollection(this);

            IsAwake = false;
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
