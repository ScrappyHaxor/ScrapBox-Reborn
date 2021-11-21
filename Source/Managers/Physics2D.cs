using System.Collections.Generic;

using Microsoft.Xna.Framework;

using ScrapBox.ECS;
using ScrapBox.ECS.Components;

using System;

namespace ScrapBox.Managers
{
    public static class Physics2D
    {
        public static IReadOnlyList<ICollider> RegisteredColliders { get { return colliderRegister.AsReadOnly(); } }
        private static List<ICollider> colliderRegister;

        public static Vector2 Gravity = new Vector2(0, 9.82f);

        static Physics2D()
        {
            colliderRegister = new List<ICollider>();
        }

        public static void RegisterCollider(ICollider collider)
        {
            colliderRegister.Add(collider);
        }

        internal static void ResolveImpulse(Entity a, Entity b)
        {

        }

        internal static void Update(GameTime gameTime)
        {
            foreach (ICollider collider in colliderRegister)
            {
                if (!collider.IsAwake) continue;
                foreach (ICollider other in colliderRegister)
                {
                    if (collider == other) continue;
                    if (!other.IsAwake) continue;
                    if (collider.Intersects(other))
                    {
                        ResolveImpulse(collider.Owner, other.Owner);
                    }
                }
            }
        }
    }
}
