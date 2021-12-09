using Microsoft.Xna.Framework.Graphics;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS
{
    internal class Particle : Entity
    {
        public Transform Transform { get; set; }
        public Sprite2D Sprite { get; set; }
        public RigidBody2D Rigidbody { get; set; }

        public int LifeSpan { get; set; }

        public Particle()
        {
            Transform = new Transform();
            AddComponent(Transform);

            Rigidbody = new RigidBody2D
            {
                Mass = 1,
                Restitution = 0f,
                Drag = 0.9,
                Friction = 0.9
        };

            AddComponent(Rigidbody);
        }

        public override void Update(double dt)
        {
            if (!IsAwake)
                return;

            LifeSpan--;
            base.Update(dt);
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!IsAwake)
                return;

            base.Draw(spriteBatch, camera);
        }
    }
}
