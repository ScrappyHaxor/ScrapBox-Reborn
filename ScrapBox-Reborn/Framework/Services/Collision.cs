using System.Collections.Generic;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.ECS;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.Services
{
    public class CollisionManifold
    {
        public ScrapVector Normal { get; set; }
        public double Depth { get; set; }
        public List<ScrapVector> ContactPoints { get; set; }

        public CollisionManifold()
        {
            Depth = float.MaxValue;
            ContactPoints = new List<ScrapVector>();
        }

        public CollisionManifold(ScrapVector normal, float depth, List<ScrapVector> contactPoints)
        {
            this.Normal = normal;
            this.Depth = depth;
            this.ContactPoints = contactPoints;
        }
    }

    public static class Collision
    {
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

        public static bool ApplyAppropriateAlgorith(Collider a, Collider b, out CollisionManifold manifold)
        {
            manifold = new CollisionManifold();
            bool result = false;

            //This method is probably going to be very ugly but oh well.
            //It needs some serious reworking in the future, this shit is gonna be so hard to maintain

            //Culling
            Camera camera = SceneManager.CurrentScene.MainCamera;
            if (camera != null)
            {
                if (!camera.InView(a.Transform.Position, a.Transform.Dimensions))
                    return false;

                if (!camera.InView(b.Transform.Position, b.Transform.Dimensions))
                    return false;
            }

            if (a.Algorithm == Collider.CollisionAlgorithm.SAT)
            {
                if (b.Algorithm == Collider.CollisionAlgorithm.SAT)
                {
                    result = IntersectPolygons(a.GetVerticies(), b.GetVerticies(), out manifold);
                }   
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
                {
                    result = IntersectPolygonCircle(a.GetVerticies(), b, out manifold);
                }
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
                {
                    if (IntersectPolygons(a.GetVerticies(), b.GetVerticies(), out manifold))
                    {
                        result = IntersectPixels(a, b);
                    }
                }
            }
            else if (a.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
            {
                if (b.Algorithm == Collider.CollisionAlgorithm.SAT)
                {
                    result = IntersectPolygonCircle(b.GetVerticies(), a, out manifold);
                }
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
                {
                    result = IntersectCircleCircle(a, b, out manifold);
                }
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
                {
                    if (IntersectPolygonCircle(b.GetVerticies(), a, out manifold))
                    {
                        result = IntersectPixels(a, b);
                    }
                }
            }
            else if (a.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
            {
                if (b.Algorithm == Collider.CollisionAlgorithm.SAT)
                {
                    if (IntersectPolygons(a.GetVerticies(), b.GetVerticies(), out manifold))
                    {
                        result = IntersectPixels(a, b);
                    }
                }
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
                {
                    if (IntersectPolygonCircle(a.GetVerticies(), b, out manifold))
                    {
                        result = IntersectPixels(a, b);
                    }
                }
                else if (b.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
                {
                    if (IntersectPolygons(a.GetVerticies(), b.GetVerticies(), out manifold))
                    {
                        result = IntersectPixels(a, b);
                    }
                }
            }

            return result;
        }

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

        public static bool IntersectPolygonCircle(ScrapVector[] verts, Collider b, out CollisionManifold manifold)
        {
            manifold = new CollisionManifold();

            ScrapVector aCenter = ScrapMath.FindArithmeticMean(verts);

            ScrapVector axis;
            double minA;
            double maxA;

            double minB;
            double maxB;

            double axisDepth;

            for (int i = 0; i < verts.Length; i++)
            {
                ScrapVector vA = verts[i];
                ScrapVector vB = verts[(i + 1) % verts.Length];

                ScrapVector edge = vA - vB;

                axis = new ScrapVector(-edge.Y, edge.X);
                axis = ScrapMath.Normalize(axis);

                ScrapMath.ProjectVerticies(verts, axis, out minA, out maxA);
                ScrapMath.ProjectCircle(b.Transform.Position, b.Dimensions.X, axis, out minB, out maxB);

                if (minA >= maxB || minB >= maxA)
                    return false;

                axisDepth = ScrapMath.Min(maxB - minA, maxA - minB);

                if (axisDepth < manifold.Depth)
                {
                    manifold.Depth = axisDepth;
                    manifold.Normal = axis;
                }
            }

            int pointIndex = ScrapMath.ClosestPointPolygon(b.Transform.Position, verts);
            if (pointIndex == -1)
            {
                LogService.Log("Collision", "IntersectPolygonCircle", "Point not found on polygon", Severity.ERROR);
                return false;
            }

            ScrapVector point = verts[pointIndex];

            axis = b.Transform.Position - point;
            axis = ScrapMath.Normalize(axis);

            ScrapMath.ProjectVerticies(verts, axis, out minA, out maxA);
            ScrapMath.ProjectCircle(b.Transform.Position, b.Dimensions.X, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
                return false;

            axisDepth = ScrapMath.Min(maxB - minA, maxA - minB);

            if (axisDepth < manifold.Depth)
            {
                manifold.Depth = axisDepth;
                manifold.Normal = axis;
            }

            ScrapVector direction = b.Transform.Position - aCenter;
            if (ScrapMath.Dot(direction, manifold.Normal) < 0)
            {
                manifold.Normal = -manifold.Normal;
            }

            return true;
        }

        public static bool IntersectCircleCircle(Collider a, Collider b, out CollisionManifold manifold)
        {
            manifold = new CollisionManifold();

            double distance = ScrapMath.Distance(a.Transform.Position, b.Transform.Position);
            double combinedRadii = a.Dimensions.X + b.Dimensions.X;

            ScrapVector distanceCenter = b.Transform.Position - a.Transform.Position;
            
            if (distance <= combinedRadii)
            {
                if (distance == 0)
                {
                    manifold.Depth = combinedRadii;
                    manifold.Normal = new ScrapVector(1, 0);
                }
                else
                {
                    manifold.Depth = combinedRadii - distance;
                    manifold.Normal = distanceCenter / distance;
                }

                return true;
            }

            return false;
        }

        public static bool IntersectPointPolygon(ScrapVector p, ScrapVector[] verts)
        {
            return IntersectPolygons(CreateMicroRectangle(p), verts, out CollisionManifold _);
        }

        public static bool IntersectPointCircle(ScrapVector p, Collider b)
        {
            return IntersectPolygonCircle(CreateMicroRectangle(p), b, out CollisionManifold _);
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
                        (int)((x - colliderA.Left + colliderA.Sprite.SourceRectangle.Left)) +
                        (int)((y - colliderA.Top + colliderA.Sprite.SourceRectangle.Top)) *
                        colliderA.Sprite.Texture.Width];

                    Color pixelB = pixelDataB[
                        (int)((x - colliderB.Left + colliderB.Sprite.SourceRectangle.Left)) +
                        (int)((y - colliderB.Top + colliderB.Sprite.SourceRectangle.Top)) *
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
    }
}
