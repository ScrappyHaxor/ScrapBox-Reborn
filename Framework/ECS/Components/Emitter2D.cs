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
        public ScrapVector LinearVelocity { get; set; }
        public ScrapVector AngularVelocity { get; set; }
        public (int, int) MinDeviation { get; set; }
        public (int, int) MaxDeviation { get; set; }

        private List<Particle> register;

        private Random rand;
        

        public Emitter2D()
        {
            rand = ScrapMath.GetSeededRandom();

            register = new List<Particle>();
            Sprites = new List<Sprite2D>();
        }

        protected ScrapVector CalcualteDeviation()
        {
            return new ScrapVector(
                rand.Next(MinDeviation.Item1, MaxDeviation.Item1), 
                rand.Next(MinDeviation.Item2, MaxDeviation.Item2));
        }

        protected void GenerateParticle()
        {
            Texture2D randTexture = Sprites[rand.Next(Sprites.Count)].Texture;
            Particle newParticle = new Particle(Transform.Position, randTexture, LifeSpan);

            newParticle.Rigidbody.AddForce(LinearVelocity + CalcualteDeviation());
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

            if (register.Count < Count)
            {
                GenerateParticle();
            }

            for (int i = 0; i < register.Count; i++)
            {
                register[i].Update(dt);

                if (register[i].Dead)
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
