using System.Collections.Generic;

using ScrapBox.Framework.Level;

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
            foreach (Entity e in Register)
            {
                e.Awake();
            }
        }

        public virtual void Sleep()
        {
            foreach (Entity e in Register)
            {
                e.Sleep();
            }
        }

        public virtual void Update(double dt)
        {
            foreach (Entity e in Register)
            {
                e.Update(dt);
            }
        }

        public virtual void Draw(Camera mainCamera)
        {
            foreach (Entity e in Register)
            {
                e.Draw(mainCamera);
            }
        }
    }
}
