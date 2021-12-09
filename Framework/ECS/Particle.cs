using System;
using System.Timers;

using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS
{
    internal class Particle : Entity
    {
        public Transform Transform { get; set; }
        public Sprite2D Sprite { get; set; }
        public RigidBody2D Rigidbody { get; set; }

        public bool Dead { get; internal set; }

        private Timer lifeTimer;

        public Particle(ScrapVector position, Texture2D sprite, int lifeSpan)
        {
            lifeTimer = new Timer(lifeSpan);
            lifeTimer.Elapsed += MarkAsDead;

            Transform = new Transform
            {
                Position = position,
                Dimensions = new ScrapVector(sprite.Width, sprite.Height)
            };

            AddComponent(Transform);

            Sprite = new Sprite2D
            {
                Texture = sprite
            };

            AddComponent(Sprite);

            Rigidbody = new RigidBody2D
            {
                Mass = 1,
                Restitution = 0f,
                Drag = 1,
                Friction = 1
            };

            AddComponent(Rigidbody);
        }

        private void MarkAsDead(object o, EventArgs e)
        {
            Dead = true;
        }

        public override void Awake()
        {
            lifeTimer.Start();
            base.Awake();
        }

        public override void Update(double dt)
        {
            if (!IsAwake)
                return;

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
