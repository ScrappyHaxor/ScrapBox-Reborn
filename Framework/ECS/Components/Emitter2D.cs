using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.ECS.Systems;

namespace ScrapBox.Framework.ECS.Components
{
    public class Emitter2D : Component
    {
        public override string Name => "Emitter2D";

        public Transform Transform;

        public List<Sprite2D> Sprites { get; set; }
        public int LifeSpan { get; set; }
        public int MaxParticles { get; set; }
        public ScrapVector LinearVelocity { get; set; }
        public ScrapVector AngularVelocity { get; set; }
        public (int, int) MinDeviation { get; set; }
        public (int, int) MaxDeviation { get; set; }

        internal readonly List<Particle> Particles;

        private readonly Random rand;
        

        public Emitter2D()
        {
            rand = ScrapMath.GetSeededRandom();

            Particles = new List<Particle>();
            Sprites = new List<Sprite2D>();
        }

        internal ScrapVector CalcualteDeviation()
        {
            return new ScrapVector(
                rand.Next(MinDeviation.Item1, MaxDeviation.Item1), 
                rand.Next(MinDeviation.Item2, MaxDeviation.Item2));
        }

        internal void GenerateParticle()
        {
            Texture2D randTexture = Sprites[rand.Next(Sprites.Count)].Texture;
            Particle newParticle = new Particle(Transform.Position, randTexture, LifeSpan);

            newParticle.Rigidbody.AddForce(LinearVelocity + CalcualteDeviation());
            newParticle.Awake();

            Particles.Add(newParticle);
        }

        public override void Awake()
        {
            bool success = Dependency(out Transform);
            if (!success)
                return;

            if (Sprites.Count == 0)
            {
                LogService.Log(Name, "Awake", "At least one sprite must be assigned to texture pool.", Severity.ERROR);
                return;
            }

            ParticleSystem particleSystem = (ParticleSystem)WorldManager.GetSystem<ParticleSystem>();
            particleSystem.RegisterEmitter(this);
            IsAwake = true;
        }


        public override void Sleep()
        {
            Particles.Clear();
            //Improve this by adding auto remove of sleeping components to World
            ParticleSystem particleSystem = (ParticleSystem)WorldManager.GetSystem<ParticleSystem>();
            particleSystem.PurgeEmitter(this);
            IsAwake = false;
        }

        internal void Tick(double dt)
        {
            if (Particles.Count < MaxParticles)
            {
                GenerateParticle();
            }

            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update(dt);

                if (Particles[i].Dead)
                {
                    Particles[i].Sleep();
                    Particles.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
