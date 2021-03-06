//using System;
//using System.Timers;
//using System.Collections.Generic;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//using ScrapBox.Framework.Level;
//using ScrapBox.Framework.ECS.Components;
//using ScrapBox.Framework.Math;

//namespace ScrapBox.Framework.ECS.Systems
//{
//    internal class Particle : Entity
//    {
//        public override string Name => "Particle";

//        public Transform Transform { get; set; }
//        public Sprite2D Sprite { get; set; }
//        public RigidBody2D Rigidbody { get; set; }

//        public bool Dead { get; internal set; }

//        private readonly Timer lifeTimer;

//        public Particle(ScrapVector position, ScrapVector size, Color tintColor, Texture2D sprite, int lifeSpan)
//        {
//            lifeTimer = new Timer(lifeSpan);
//            lifeTimer.Elapsed += MarkAsDead;

//            Transform = new Transform
//            {
//                Position = position,
//                Dimensions = size
//            };

//            RegisterComponent(Transform);

//            Sprite = new Sprite2D
//            {
//                Texture = sprite,
//                TintColor = tintColor
//            };

//            RegisterComponent(Sprite);

//            Rigidbody = new RigidBody2D
//            {
//                Mass = 1,
//                Restitution = 0f,
//                Drag = 1,
//                Friction = 1
//            };

//            RegisterComponent(Rigidbody);
//        }

//        private void MarkAsDead(object o, EventArgs e)
//        {
//            Dead = true;
//        }

//        public override void Awake()
//        {
//            lifeTimer.Start();

//            base.Awake();
//        }
//    }

//    public class ParticleSystem : ComponentSystem
//    {
//        private readonly List<Emitter2D> emitters;

//        public ParticleSystem()
//        {
//            emitters = new List<Emitter2D>();
//        }

//        public void RegisterEmitter(Emitter2D emitter)
//        {
//            emitters.Add(emitter);
//        }

//        public void PurgeEmitter(Emitter2D emitter)
//        {
//            emitters.Remove(emitter);
//        }

//        public override void Reset()
//        {
//            emitters.Clear();
//        }

//        public override void Update(double dt)
//        {
//            for (int i = 0; i < emitters.Count; i++)
//            {
//                Emitter2D emitter = emitters[i];
//                if (!emitter.IsAwake)
//                    continue;

//                emitter.Tick(dt);
//            }
//        }

//        public override void Draw(Camera mainCamera)
//        {

//        }
//    }
//}
