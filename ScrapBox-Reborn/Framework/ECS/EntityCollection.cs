using System.Collections.Generic;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.ECS
{
    public abstract class EntityCollection
    {
        public abstract List<Entity> Register { get; set; }

        protected EntityCollection()
        {
            Register = new List<Entity>();
        }

        public virtual void Awake()
        {
            WorldManager.RegisterEntityCollection(this);

            foreach (Entity e in Register)
            {
                e.Awake();
            }
        }

        public virtual void Sleep()
        {
            WorldManager.PurgeEntityCollection(this);

            foreach (Entity e in Register)
            {
                e.Sleep();
            }
        }

        public virtual void Update(double dt)
        {

        }

        public virtual void Draw(Camera mainCamera)
        {

        }
    }
}
