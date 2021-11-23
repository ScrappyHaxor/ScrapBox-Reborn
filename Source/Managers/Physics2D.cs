using System.Collections.Generic;

using Microsoft.Xna.Framework;

using ScrapBox.ECS;
using ScrapBox.ECS.Components;
using ScrapBox.SMath;
using ScrapBox.Utils;

using System;

namespace ScrapBox.Managers
{
    public static class Physics2D
    {
        public const float MAX_ITERATIONS = 6;

        public static ScrapVector Gravity = new ScrapVector(0, 9.14) * 100;
        public static float AirFriction = 0.5f;

        internal static List<ICollider> Colliders;

        internal static List<RigidBody2D> CollidingBodiesA;
        internal static List<RigidBody2D> CollidingBodiesB;

        internal static List<CollisionManifold> Manifolds;

        static Physics2D()
        {
            Colliders = new List<ICollider>();
            CollidingBodiesA = new List<RigidBody2D>();
            CollidingBodiesB = new List<RigidBody2D>();
            Manifolds = new List<CollisionManifold>();
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
                    Color pixelA;
                    if (animatorA != null)
                    {
                        pixelA = pixelDataA[(x - (int)colliderA.Left+animatorA.CurrentCell.Left) + (y - (int)colliderA.Top+animatorA.CurrentCell.Top) 
                            * (int)colliderA.Sprite.Texture.Width];
                    }
                    else
                    {
                        pixelA = pixelDataA[(x - (int)colliderA.Left) + (y - (int)colliderA.Top) * (int)colliderA.Sprite.Texture.Width];
                    }

                    Color pixelB;
                    if (animatorB != null)
                    {
                        pixelB = pixelDataB[(x - (int)colliderB.Left+animatorB.CurrentCell.Left) + (y - (int)colliderB.Top+animatorB.CurrentCell.Top) 
                            * (int)colliderB.Sprite.Texture.Width];
                    }
                    else
                    {
                        pixelB = pixelDataB[(x - (int)colliderB.Left) + (y - (int)colliderB.Top) * (int)colliderB.Sprite.Texture.Width];
                    }

                    if (pixelA.A != 0 && pixelB.A != 0)
                        return true;
                }
            }

            return false;
        }

        public static void ResolveImpulse(RigidBody2D a, RigidBody2D b, CollisionManifold manifold, GameTime gameTime)
        {
            ScrapVector relativeVelocity = b.Velocity - a.Velocity;

            if (ScrapMath.Dot(relativeVelocity, manifold.Normal) > 0)
                return;

            double j = -(1 + a.Restitution) * ScrapMath.Dot(relativeVelocity, manifold.Normal);
            j /= a.InverseMass + b.InverseMass;

            double percent = 0.05f;
            double slop = 0.01f;
            ScrapVector correct = Math.Max(manifold.Depth - slop, 0.0) / (a.InverseMass + b.InverseMass) * percent * manifold.Normal;

            if (!a.IsStatic)
            {
                a.Transform.Position -= a.InverseMass * correct;
            }
            
            if (!b.IsStatic)
            {
                b.Transform.Position += b.InverseMass * correct;
            }

            
            a.Velocity -= manifold.Normal * j * a.InverseMass;
            b.Velocity += manifold.Normal * j * b.InverseMass;
        }

        internal static void Update(GameTime gameTime)
        {
            CollidingBodiesA.Clear();
            CollidingBodiesB.Clear();
            Manifolds.Clear();

            //Detect colllisions
            for (int i = 0; i < Colliders.Count; i++)
            {
                if (Colliders[i].RigidBody.IsStatic)
                    continue;

                for (int j = 0; j < Colliders.Count; j++)
                {
                    if (i == j)
                        continue;
                    
                    if (IntersectPolygons(Colliders[i].GetVerticies(), Colliders[j].GetVerticies(), out CollisionManifold manifold))
                    {
                        if (!IntersectPixels(Colliders[i], Colliders[j])) continue;
                        CollidingBodiesA.Add(Colliders[i].RigidBody);
                        CollidingBodiesB.Add(Colliders[j].RigidBody);

                        Manifolds.Add(manifold);
                    }
                }
            }


            //Apply forces
            foreach (ICollider collider in Colliders)
            {
                if (collider.RigidBody.IsStatic)
                    continue;

                collider.RigidBody.ApplyForces(gameTime);
            }

            //Apply velocities
            foreach (ICollider collider in Colliders)
            {
                if (collider.RigidBody.IsStatic)
                    continue;

                collider.RigidBody.ApplyVelocities(gameTime);
            }

            //Iterative impulse resolution
            for (int k = 0; k < MAX_ITERATIONS; k++)
            {
                for (int i = 0; i < Manifolds.Count; i++)
                {

                    ResolveImpulse(CollidingBodiesA[i], CollidingBodiesB[i], Manifolds[i], gameTime);
                }
            }
        }

    }
}
