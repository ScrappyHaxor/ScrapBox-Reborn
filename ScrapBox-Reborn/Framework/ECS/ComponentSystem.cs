using System.Collections.Generic;

using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.ECS
{
    public abstract class ComponentSystem
    {
        public abstract void Tick(double dt);
        public abstract void Render(Camera mainCamera);
        public abstract void Reset();
    }
}
