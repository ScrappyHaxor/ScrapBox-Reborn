using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Scene;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
    public class Emitter2D : IComponent
    {
        public Entity Owner { get; set; }
        public bool IsAwake { get; set; }

        public Transform Transform { get; set; }
        public List<Sprite2D> Sprites { get; set; }
        public int LifeSpan { get; set; }
        public int Count { get; set; }

        private List<Particle> register;

        private Random rand;
        

        public Emitter2D()
        {
            rand = ScrapMath.GetSeededRandom();

            register = new List<Particle>();
            Sprites = new List<Sprite2D>();
        }

        protected void GenerateParticle()
        {
            Particle newParticle = new Particle();
            newParticle.AddComponent(Sprites[rand.Next(Sprites.Count)]);

            newParticle.Transform.Position = Transform.Position;
            newParticle.Transform.Dimensions = Transform.Dimensions;
            newParticle.Rigidbody.AddForce(new ScrapVector(
                1 * rand.NextDouble() * 2 - 1, 
                1 * rand.NextDouble() * 2 - 1));

            newParticle.LifeSpan = LifeSpan;
            newParticle.Awake();

            register.Add(newParticle);
        }

        public void Awake()
        {
            Transform = Owner.GetComponent<Transform>();
            if (Transform == null)
            {
                LogManager.Log(new LogMessage("Emitter2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
                return;
            }

            if (!Transform.IsAwake)
            {
                LogManager.Log(new LogMessage("Emitter2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
                return;
            }

            if (Sprites.Count == 0)
            {
                LogManager.Log(new LogMessage("Emitter2D", "At least one sprite must be assigned... Aborting...", LogMessage.Severity.ERROR));
                return;
            }

            IsAwake = true;
        }


        public void Sleep()
        {
            IsAwake = false;
        }

        public void Update(double dt)
        {
            if (!IsAwake)
                return;

            if (register.Count - 1 < Count)
            {
                GenerateParticle();
            }

            for (int i = 0; i < register.Count; i++)
            {
                register[i].Update(dt);

                if (register[i].LifeSpan <= 0)
                {
                    register.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!IsAwake)
                return;

            foreach (Particle p in register)
            {
                p.Draw(spriteBatch, camera);
            }
        }
    }
}
