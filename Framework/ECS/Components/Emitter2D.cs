using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.ECS.Systems;

namespace ScrapBox.Framework.ECS.Components
{
    public enum EmitterType
    {
        CONE,
        CIRCLE
    }

    public class Emitter2D : Component
    {
        public override string Name => "Emitter2D";

        public Transform Transform;

        public List<Sprite2D> Sprites { get; set; }
        public int LifeSpan { get; set; }
        public int MaxParticles { get; set; }
        public double LinearVelocity { get; set; }
        public double AngularVelocity { get; set; }
        public (int, int) MinDeviation { get; set; }
        public (int, int) MaxDeviation { get; set; }
        public ScrapVector ParticleSize { get; set; }

        public EmitterType TypeOfEmitter { get; set; }

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
            Sprite2D randSprite = Sprites[rand.Next(Sprites.Count)];
            Texture2D randTexture = randSprite.Texture;
            Particle newParticle = new Particle(Transform.Position, ParticleSize, randSprite.TintColor, randTexture, LifeSpan);

            if (TypeOfEmitter == EmitterType.CONE)
            {
                newParticle.Rigidbody.AddForce(ScrapMath.RotatePoint(new ScrapVector(LinearVelocity, 0) + CalcualteDeviation(), Transform.Rotation));
            }
            else
            {
                double random = rand.Next(0, 360);

                newParticle.Rigidbody.AddForce(LinearVelocity * new ScrapVector(ScrapMath.Cos(random), ScrapMath.Sin(random)));
            }

            
            newParticle.Awake();

            Particles.Add(newParticle);
        }

        public override void Awake()
        {
            if (IsAwake)
                return;

            bool success = Dependency(out Transform);
            if (!success)
                return;

            if (Sprites.Count == 0)
            {
                LogService.Log(Name, "Awake", "At least one sprite must be assigned to texture pool.", Severity.ERROR);
                return;
            }

            //if (ParticleSize == ScrapVector.Zero)
            //{
            //    ParticleSize = Transform.Dimensions;
            //}

            WorldManager.GetSystem<ParticleSystem>().RegisterEmitter(this);
            IsAwake = true;
        }


        public override void Sleep()
        {
            if (!IsAwake)
                return;

            Particles.Clear();
            //Improve this by adding auto remove of sleeping components to World
            WorldManager.GetSystem<ParticleSystem>().PurgeEmitter(this);
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
