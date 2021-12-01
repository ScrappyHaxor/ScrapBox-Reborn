using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.ECS;
using ScrapBox.ECS.Components;
using ScrapBox.SMath;
using ScrapBox.Utils;
using ScrapBox.Args;

using System;
using System.Diagnostics;

namespace ScrapBox.Managers
{
    public static class Physics2D
    {
        public const float MAX_ITERATIONS = 20;
        public const float MAX_IMPULSE_ITERATIONS = 1;

        public static ScrapVector Gravity = new ScrapVector(0, 9.14) * 100;

        public static List<ICollider> GetDynamicBodies { get { return DynamicBodies; } }
        public static List<ICollider> GetStaticBodies { get { return StaticBodies; } }

        internal static List<ICollider> DynamicBodies;
        internal static List<ICollider> StaticBodies;

        internal static List<RigidBody2D> CollidingBodiesA;
        internal static List<RigidBody2D> CollidingBodiesB;

        internal static List<CollisionManifold> Manifolds;

        internal static Stopwatch watch;

        static Physics2D()
        {
            DynamicBodies = new List<ICollider>();
            StaticBodies = new List<ICollider>();

            CollidingBodiesA = new List<RigidBody2D>();
            CollidingBodiesB = new List<RigidBody2D>();
            Manifolds = new List<CollisionManifold>();

            watch = new Stopwatch();
        }

        internal static ScrapVector[] CreateMicroRectangle(ScrapVector position)
        {
            List<ScrapVector> verts = new List<ScrapVector>();
            verts.Add(position + -ScrapVector.One);
            verts.Add(position + new ScrapVector(1, -1));
            verts.Add(position + ScrapVector.One);
            verts.Add(position + new ScrapVector(-1, 1));

            return verts.ToArray();
        }

        //SAT prototype
        public static bool IntersectPolygons(ScrapVector[] vertsA, ScrapVector[] vertsB, out CollisionManifold manifold)
        {
            manifold = new CollisionManifold();

            ScrapVector centerA = ScrapMath.FindArithmeticMean(vertsA);
            ScrapVector centerB = ScrapMath.FindArithmeticMean(vertsB);

            ScrapVector contactA = ScrapVector.Zero;
            ScrapVector contactB = ScrapVector.Zero;
            for (int i = 0; i < vertsA.Length; i++)
            {

                ScrapVector va = vertsA[i];
                ScrapVector vb = vertsA[(i + 1) % vertsA.Length];

                ScrapVector edge = vb - va;
                ScrapVector axis = new ScrapVector(-edge.Y, edge.X);
                axis = ScrapMath.Normalize(axis);

                ScrapMath.ProjectVerticies(vertsA, axis, out double minA, out double maxA);
                ScrapMath.ProjectVerticies(vertsB, axis, out double minB, out double maxB);

                if (minA >= maxB || minB >= maxA)
                    return false;

                double axisDepth = Math.Min(maxB - minA, maxA - minB);
                if (axisDepth < manifold.Depth || 
                        (axisDepth == manifold.Depth 
                         && Math.Abs(ScrapMath.Length(centerA) - axisDepth) < Math.Abs(ScrapMath.Length(centerA) - manifold.Depth)))
                {
                    manifold.Depth = axisDepth;
                    manifold.Normal = axis;

                    contactA = va;
                    contactB = vb;
                }
            }

            for (int i = 0; i < vertsB.Length; i++)
            {

                ScrapVector va = vertsB[i];
                ScrapVector vb = vertsB[(i + 1) % vertsB.Length];

                ScrapVector edge = vb - va;
                ScrapVector axis = new ScrapVector(-edge.Y, edge.X);
                axis = ScrapMath.Normalize(axis);

                ScrapMath.ProjectVerticies(vertsA, axis, out double minA, out double maxA);
                ScrapMath.ProjectVerticies(vertsB, axis, out double minB, out double maxB);

                if (minA >= maxB || minB >= maxA)
                    return false;

                double axisDepth = Math.Min(maxB - minA, maxA - minB);
                if (axisDepth < manifold.Depth)
                {
                    manifold.Depth = axisDepth;
                    manifold.Normal = axis;
                    
                    contactA = va;
                    contactB = vb;
                }
            }

            manifold.ContactPoints.Add(contactA);
            manifold.ContactPoints.Add(contactB);

            ScrapVector direction = centerB - centerA;
            if (ScrapMath.Dot(direction, manifold.Normal) < 0)
            {
                manifold.Normal = -manifold.Normal;
            }

            return true;
        }

        public static bool IntersectPoint(ScrapVector v, ScrapVector[] verts)
        {
            return IntersectPolygons(CreateMicroRectangle(v), verts, out CollisionManifold _);
        }

        public static bool IntersectPixels(ICollider colliderA, ICollider colliderB, 
                Animator2D animatorA = null, Animator2D animatorB = null)
        {
            if (colliderA.Sprite.Texture == null || colliderB.Sprite.Texture == null) 
                return false;

            Color[] pixelDataA = new Color[colliderA.Sprite.Texture.Width * colliderA.Sprite.Texture.Height];
            Color[] pixelDataB = new Color[colliderB.Sprite.Texture.Width * colliderB.Sprite.Texture.Height];

            colliderA.Sprite.Texture.GetData(pixelDataA);
            colliderB.Sprite.Texture.GetData(pixelDataB);

            double top = Math.Max(colliderA.Top, colliderB.Top);
            double bottom = Math.Min(colliderA.Bottom, colliderB.Bottom);
            double left = Math.Max(colliderA.Left, colliderB.Left);
            double right = Math.Min(colliderA.Right, colliderB.Right);

            for (int y = (int)top; y < (int)bottom; y++)
            {
                for (int x = (int)left; x < (int)right; x++)
                {
                    Color pixelA = pixelDataA[
                        (int)((x - colliderA.Left+colliderA.Sprite.SourceRectangle.Left)) + 
                        (int)((y - colliderA.Top+colliderA.Sprite.SourceRectangle.Top)) *
                        colliderA.Sprite.Texture.Width];

                    Color pixelB = pixelDataB[
                        (int)((x - colliderB.Left+colliderB.Sprite.SourceRectangle.Left)) + 
                        (int)((y - colliderB.Top+colliderB.Sprite.SourceRectangle.Top)) *
                            colliderB.Sprite.Texture.Width];

                    if (pixelA.A != 0 && pixelB.A != 0)
                        return true;
                }
            }

            return false;
        }

        public static void ResolveImpulse(RigidBody2D a, RigidBody2D b, CollisionManifold manifold)
        {
            ScrapVector relativeVelocity = b.LinearVelocity - a.LinearVelocity;

            if (ScrapMath.Dot(relativeVelocity, manifold.Normal) > 0)
                return;

            double j = -(1 + a.Restitution) * ScrapMath.Dot(relativeVelocity, manifold.Normal);
            j /= a.InverseMass + b.InverseMass;

            double percent = 0.5;
            ScrapVector correct = manifold.Depth / (a.InverseMass + b.InverseMass) * percent * manifold.Normal;

            if (!a.IsStatic && !a.Kinematic)
            {
                a.Transform.Position -= a.InverseMass * correct;      
            }

            if (!b.IsStatic && !b.Kinematic)
            {
                b.Transform.Position += b.InverseMass * correct;    
            }

            a.LinearVelocity -= manifold.Normal * j * a.InverseMass;
            b.LinearVelocity += manifold.Normal * j * b.InverseMass;
        }

        internal static void Update(double dt)
        {
            watch.Restart();
            CollidingBodiesA.Clear();
            CollidingBodiesB.Clear();
            Manifolds.Clear();

            //Detect colllisions and apply rigidbody states for dynamic versus static
            foreach (ICollider dynamicB in DynamicBodies)
            {
                RigidBody2D.RigidState state = RigidBody2D.RigidState.FALLING;
                foreach (ICollider staticB in StaticBodies)
                {
                    if (!dynamicB.IsAwake || !staticB.IsAwake)
                        return;

                    if (IntersectPolygons(dynamicB.GetVerticies(), staticB.GetVerticies(), out CollisionManifold manifold))
                    {
                        if (staticB.Trigger != ICollider.TriggerType.NONE)
                        {
                            staticB.Triggered?.Invoke(null, new TriggerArgs(dynamicB));

                            if (staticB.Trigger == ICollider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        if (staticB.Trigger != ICollider.TriggerType.TRIGGER_ONLY &&
                                IntersectPoint(new ScrapVector(dynamicB.Transform.Position.X, dynamicB.Bottom), staticB.GetVerticies()))
                        {
                            state = RigidBody2D.RigidState.REST_STATIC;
                        }

                        CollidingBodiesA.Add(dynamicB.RigidBody);
                        CollidingBodiesB.Add(staticB.RigidBody);

                        Manifolds.Add(manifold);
                    }
                }

                dynamicB.RigidBody.State = state;
            }

            //Detect collisions and apply rigidbody states for dynamic versus dynamic
            for (int i = 0; i < DynamicBodies.Count; i++)
            {
                RigidBody2D.RigidState state = DynamicBodies[i].RigidBody.State;
                for (int j = 0; j < DynamicBodies.Count; j++)
                {
                    if (i == j) continue;

                    if (!DynamicBodies[i].IsAwake || !DynamicBodies[j].IsAwake)
                        return;

                    if (IntersectPolygons(DynamicBodies[i].GetVerticies(), DynamicBodies[j].GetVerticies(), out CollisionManifold manifold) && 
                            IntersectPixels(DynamicBodies[i], DynamicBodies[j]))
                    {
                        if (DynamicBodies[j].Trigger != ICollider.TriggerType.TRIGGER_ONLY &&
                            IntersectPoint(new ScrapVector(DynamicBodies[i].Transform.Position.X, DynamicBodies[i].Bottom), DynamicBodies[j].GetVerticies()))
                        {
                            state = RigidBody2D.RigidState.REST_DYNAMIC;
                        }

                        if (DynamicBodies[i].Trigger != ICollider.TriggerType.NONE)
                        {
                            DynamicBodies[i].Triggered?.Invoke(null, new TriggerArgs(DynamicBodies[j]));
                            if (DynamicBodies[i].Trigger == ICollider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        if (DynamicBodies[j].Trigger != ICollider.TriggerType.NONE)
                        {
                            DynamicBodies[j].Triggered?.Invoke(null, new TriggerArgs(DynamicBodies[i]));
                            if (DynamicBodies[j].Trigger == ICollider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        CollidingBodiesA.Add(DynamicBodies[i].RigidBody);
                        CollidingBodiesB.Add(DynamicBodies[j].RigidBody);

                        Manifolds.Add(manifold);
                    }
                }

                DynamicBodies[i].RigidBody.State = state;
            }


            //Apply forces
            foreach (ICollider collider in DynamicBodies)
            {
                if (collider.RigidBody.IsStatic || collider.RigidBody.Kinematic)
                    continue;

                collider.RigidBody.ApplyForces(dt, 1);
            }

            for (int i = 0; i < Manifolds.Count; i++)
            {
                for (int j = 0; j < MAX_IMPULSE_ITERATIONS; j++)
                {
                    ResolveImpulse(CollidingBodiesA[i], CollidingBodiesB[i], Manifolds[i]);
                }
    
            }

            watch.Stop();
        }
    }
}
