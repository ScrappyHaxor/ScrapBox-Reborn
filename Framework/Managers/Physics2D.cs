using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.ECS;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Utils;



namespace ScrapBox.Framework.Managers
{
    public static class Physics2D
    {
        public const float MAX_ITERATIONS = 20;
        public const float MAX_IMPULSE_ITERATIONS = 1;

        public static ScrapVector Gravity = new ScrapVector(0, 9.14) * 100;

        public static List<RigidBody2D> GetDynamicBodies { get { return DynamicBodies; } }
        public static List<RigidBody2D> GetStaticBodies { get { return StaticBodies; } }

        internal static List<RigidBody2D> DynamicBodies;
        internal static List<RigidBody2D> StaticBodies;

        internal static List<RigidBody2D> CollidingBodiesA;
        internal static List<RigidBody2D> CollidingBodiesB;

        internal static List<CollisionManifold> Manifolds;

        internal static Stopwatch watch;

        static Physics2D()
        {
            DynamicBodies = new List<RigidBody2D>();
            StaticBodies = new List<RigidBody2D>();

            CollidingBodiesA = new List<RigidBody2D>();
            CollidingBodiesB = new List<RigidBody2D>();
            Manifolds = new List<CollisionManifold>();

            watch = new Stopwatch();
        }

        internal static ScrapVector[] CreateMicroRectangle(ScrapVector position)
        {
            List<ScrapVector> verts = new List<ScrapVector>
            {
                position + -ScrapVector.One,
                position + new ScrapVector(1, -1),
                position + ScrapVector.One,
                position + new ScrapVector(-1, 1)
            };

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

                double axisDepth = ScrapMath.Min(maxB - minA, maxA - minB);
                if (axisDepth < manifold.Depth || 
                        (axisDepth == manifold.Depth 
                         && ScrapMath.Abs(ScrapMath.Length(centerA) - axisDepth) < ScrapMath.Abs(ScrapMath.Length(centerA) - manifold.Depth)))
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

                double axisDepth = ScrapMath.Min(maxB - minA, maxA - minB);
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

        public static bool IntersectPixels(Collider colliderA, Collider colliderB)
        {
            if (colliderA.Sprite.Texture == null || colliderB.Sprite.Texture == null) 
                return false;

            Color[] pixelDataA = new Color[colliderA.Sprite.Texture.Width * colliderA.Sprite.Texture.Height];
            Color[] pixelDataB = new Color[colliderB.Sprite.Texture.Width * colliderB.Sprite.Texture.Height];

            colliderA.Sprite.Texture.GetData(pixelDataA);
            colliderB.Sprite.Texture.GetData(pixelDataB);

            double top = ScrapMath.Max(colliderA.Top, colliderB.Top);
            double bottom = ScrapMath.Min(colliderA.Bottom, colliderB.Bottom);
            double left = ScrapMath.Max(colliderA.Left, colliderB.Left);
            double right = ScrapMath.Min(colliderA.Right, colliderB.Right);

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
            foreach (RigidBody2D dynamicBody in DynamicBodies)
            {
                if (!dynamicBody.IsAwake || !dynamicBody.HasCollider)
                    continue;

                Collider colliderA = dynamicBody.Collider;

                RigidBody2D.RigidState state = RigidBody2D.RigidState.FALLING;
                foreach (RigidBody2D staticBody in StaticBodies)
                {
                    if (!staticBody.IsAwake || !staticBody.HasCollider)
                        continue;

                    Collider colliderB = staticBody.Collider;

                    if (IntersectPolygons(colliderA.GetVerticies(), colliderB.GetVerticies(), out CollisionManifold manifold))
                    {
                        if (colliderB.Trigger != Collider.TriggerType.NONE)
                        {
                            colliderB.Triggered?.Invoke(null, new TriggerArgs(colliderA));

                            if (colliderB.Trigger == Collider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        if (colliderB.Trigger != Collider.TriggerType.TRIGGER_ONLY &&
                                IntersectPoint(new ScrapVector(colliderA.Transform.Position.X, colliderA.Bottom), colliderB.GetVerticies()))
                        {
                            if (Gravity != ScrapVector.Zero)
                                state = RigidBody2D.RigidState.REST_STATIC;
                        }

                        CollidingBodiesA.Add(dynamicBody);
                        CollidingBodiesB.Add(staticBody);

                        Manifolds.Add(manifold);
                    }
                }

                dynamicBody.State = state;
            }

            //Detect collisions and apply rigidbody states for dynamic versus dynamic
            for (int i = 0; i < DynamicBodies.Count; i++)
            {
                if (!DynamicBodies[i].IsAwake || !DynamicBodies[i].HasCollider)
                    continue;

                Collider colliderA = DynamicBodies[i].Collider;

                RigidBody2D.RigidState state = DynamicBodies[i].State;
                for (int j = 0; j < DynamicBodies.Count; j++)
                {
                    if (i == j) continue;

                    if (!DynamicBodies[j].IsAwake || !DynamicBodies[j].HasCollider)
                        continue;

                    Collider colliderB = DynamicBodies[j].Collider;

                    if (IntersectPolygons(colliderA.GetVerticies(), colliderB.GetVerticies(), out CollisionManifold manifold) && 
                            IntersectPixels(colliderA, colliderB))
                    {
                        if (colliderB.Trigger != Collider.TriggerType.TRIGGER_ONLY &&
                            IntersectPoint(new ScrapVector(DynamicBodies[i].Transform.Position.X, colliderA.Bottom), colliderB.GetVerticies()))
                        {
                            if (Gravity != ScrapVector.Zero)
                                state = RigidBody2D.RigidState.REST_DYNAMIC;
                        }

                        if (colliderA.Trigger != Collider.TriggerType.NONE)
                        {
                            colliderA.Triggered?.Invoke(null, new TriggerArgs(colliderB));
                            if (colliderA.Trigger == Collider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        if (colliderB.Trigger != Collider.TriggerType.NONE)
                        {
                            colliderB.Triggered?.Invoke(null, new TriggerArgs(colliderA));
                            if (colliderB.Trigger == Collider.TriggerType.TRIGGER_ONLY)
                                continue;
                        }

                        CollidingBodiesA.Add(DynamicBodies[i]);
                        CollidingBodiesB.Add(DynamicBodies[j]);

                        Manifolds.Add(manifold);
                    }
                }

                DynamicBodies[i].State = state;
            }


            //Apply forces
            foreach (RigidBody2D body in DynamicBodies)
            {
                if (body.IsStatic || body.Kinematic)
                    continue;

                body.ApplyForces(dt, 1);
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
