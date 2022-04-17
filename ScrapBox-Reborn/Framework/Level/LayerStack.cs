using ScrapBox.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Level
{
    public class LayerStack
    {
        public List<Layer> Stack { get { return stack; } }

        private readonly List<Layer> stack;

        public LayerStack()
        {
            stack = new List<Layer>();
        }

        public void InsertAt(int priority, Layer layer)
        {
            stack.Insert(priority, layer);
        }

        public Layer Fetch(int priority)
        {
            return stack[priority];
        }

        public Layer Fetch(DefaultLayers priority)
        {
            return stack[(int)priority];
        }

        internal void Purge()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Purge();
            }

            stack.Clear();
        }

        internal void Reset()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Reset();
            }
        }

        internal void Update(double dt)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Update(dt);
            }
        }

        internal void Render(Camera camera)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Render(camera);
            }
        }
    }
}
