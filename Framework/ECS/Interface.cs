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

        private InterfaceSystem interfaceSystem;

        public override void Awake()
        {
            bool success = Dependency(out Transform);
            if (!success)
                return;

            interfaceSystem = (InterfaceSystem)WorldManager.GetSystem<InterfaceSystem>();
            interfaceSystem.RegisterInterface(this);

            IsAwake = true;
        }

        public override void Sleep()
        {
            interfaceSystem.PurgeInterface(this);
            IsAwake = false;
        }

        public virtual void Tick()
        {

        }

        public virtual void Render(Camera mainCamera)
        {

        }
    }
}
