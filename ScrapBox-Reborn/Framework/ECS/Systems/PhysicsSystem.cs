using System.Diagnostics;

using System.Collections.Generic;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Systems
{
    public class PhysicsSystem : ComponentSystem
    {
        public const float MAX_ITERATIONS = 20;
        public const float MAX_IMPULSE_ITERATIONS = 1;

        public static ScrapVector Gravity = new ScrapVector(0, 9.14) * 100;

        public static int Static;
        public static int Dynamic;
        public static double TimeTaken;

        private readonly Stopwatch watch;

        private readonly List<RigidBody2D> DynamicBodies;
        private readonly List<RigidBody2D> StaticBodies;

        public PhysicsSystem()
        {
            watch = new Stopwatch();

            DynamicBodies = new List<RigidBody2D>();
            StaticBodies = new List<RigidBody2D>();
        }

        public void RegisterBody(RigidBody2D body)
        {
            if (body.IsStatic)
            {
                StaticBodies.Add(body);
                Static++;
            }
            else
            {
                DynamicBodies.Add(body);
                Dynamic++;
            }
        }

        public void PurgeBody(RigidBody2D body)
        {
            if (body.IsStatic)
            {
                StaticBodies.Remove(body);
                Static--;
            }
            else
            {
                DynamicBodies.Remove(body);
                Dynamic--;
            }
        }

        public override void Reset()
        {
            StaticBodies.Clear();
            DynamicBodies.Clear();
        }

        public override void Update(double dt)
        {
            watch.Restart();

            //Apply forces

            for (int i = 0; i < DynamicBodies.Count; i++)
            { 
                RigidBody2D body = DynamicBodies[i];
                if (!body.IsAwake)
                    continue;

                body.ApplyForces(dt, 1);
            }

            TimeTaken = watch.ElapsedMilliseconds / 1000;
        }

        public override void Draw(Camera mainCamera)
        {

        }
    }
}
