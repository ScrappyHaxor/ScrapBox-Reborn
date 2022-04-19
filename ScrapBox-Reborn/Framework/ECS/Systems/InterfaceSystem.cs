using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.ECS.Systems
{
    public class InterfaceSystem : ComponentSystem
    {
        private readonly List<Interface> interfaces; 

        public InterfaceSystem()
        {
            interfaces = new List<Interface>();
        }

        public void RegisterInterface(Interface inter)
        {
            interfaces.Add(inter);
        }

        public void PurgeInterface(Interface inter)
        {
            interfaces.Remove(inter);
        }

        public override void Reset()
        {
            interfaces.Clear();
        }

        public override void Tick(double dt)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                Interface inter = interfaces[i];
                if (!inter.IsAwake)
                    continue;

                inter.Tick();
            }
        }

        public override void Render(Camera mainCamera)
        {
            for (int i = 0; i < interfaces.Count; i++)
            {
                Interface inter = interfaces[i];
                if (!inter.IsAwake)
                    continue;

                inter.Render(mainCamera);
            }
        }
    }
}
