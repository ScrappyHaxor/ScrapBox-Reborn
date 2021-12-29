using System.Collections.Generic;

using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.ECS
{
    public abstract class ComponentSystem
    {
        public abstract void Update(double dt);
        public abstract void Draw(Camera mainCamera);
    }
}
