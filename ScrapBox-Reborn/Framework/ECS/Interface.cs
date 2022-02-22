using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Shapes;

namespace ScrapBox.Framework.ECS
{
    public abstract class Interface : Component
    {
        public Transform Transform;

        public override void Awake()
        {
            if (IsAwake)
                return;

            bool success = Dependency(out Transform);
            if (!success)
                return;

            WorldManager.GetSystem<InterfaceSystem>().RegisterInterface(this);
            IsAwake = true;
        }

        public override void Sleep()
        {
            if (!IsAwake)
                return;

            WorldManager.GetSystem<InterfaceSystem>().PurgeInterface(this);
            IsAwake = false;
        }

        internal virtual void Tick()
        {

        }

        internal virtual void Render(Camera mainCamera)
        {

        }
    }
}
