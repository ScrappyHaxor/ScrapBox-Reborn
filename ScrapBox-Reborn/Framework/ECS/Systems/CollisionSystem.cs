using System.Collections.Generic;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Systems
{
    public struct IncidentReport
    {
        public readonly RigidBody2D BodyA;
        public readonly RigidBody2D BodyB;
        public readonly CollisionManifold Manifold;

        public IncidentReport(RigidBody2D a, RigidBody2D b, CollisionManifold manifold)
        {
            BodyA = a;
            BodyB = b;
            Manifold = manifold;
        }
    }

    public class RayResult
    {
        public readonly bool hit;
        public readonly Entity other;

        public RayResult(bool hit, Entity other)
        {
            this.hit = hit;
            this.other = other;
        }
    }

    public abstract class Ray
    {
        public abstract RayResult Shoot(Collider other);
    }

    public class PointRay : Ray
    {
        public readonly ScrapVector point;

        public PointRay(ScrapVector p)
        {
            point = p;
        }

        public override RayResult Shoot(Collider other)
        {
            if (other.Algorithm == Collider.CollisionAlgorithm.SAT ||
                other.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
            {
                return new RayResult(Collision.IntersectPointPolygon(point, other.GetVerticies()), other.Owner);
            }
            else if (other.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
            {
                return new RayResult(Collision.IntersectPointCircle(point, other), other.Owner);
            }

            return new RayResult(false, null);
        }
    }

    public class ShapeRay : Ray
    {
        public readonly ScrapVector[] verts;


        public ShapeRay(ScrapVector[] verts)
        {
            this.verts = verts;
        }

        public override RayResult Shoot(Collider other)
        {
            if (other.Algorithm == Collider.CollisionAlgorithm.SAT ||
                other.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
            {
                return new RayResult(Collision.IntersectPolygons(verts, other.GetVerticies(), out CollisionManifold _), other.Owner);
            }

            return new RayResult(false, null);
        }
    }

    public class CollisionSystem : ComponentSystem
    {
        private readonly List<Collider> colliders;
        private readonly List<IncidentReport> reports;
        

        public CollisionSystem()
        {
            colliders = new List<Collider>();
            reports = new List<IncidentReport>();
        }

        public void RegisterCollider(Collider collider)
        {
            colliders.Add(collider);
        }

        public void PurgeCollider(Collider collider)
        {
            colliders.Remove(collider);
        }

        public override void Reset()
        {
            colliders.Clear();
            reports.Clear();
        }

        /// <summary>
        /// Raycasts collider
        /// </summary>
        /// <returns>True if collider intersects with an object</returns>
        public RayResult Raycast(Ray ray)
        {
            RayResult result = new RayResult(false, null);
            foreach (Collider c in colliders)
            {
                result = ray.Shoot(c);
                if (result.hit)
                    return result;
            }

            return result;
        }

        public override void Update(double dt)
        {
            reports.Clear();

            for (int i = 0; i < colliders.Count; i++)
            {
                Collider colliderA = colliders[i];

                if (!colliderA.IsAwake)
                    continue;

                for (int j = i; j < colliders.Count; j++)
                {
                    if (i == j) continue;

                    Collider colliderB = colliders[j];

                    if (!colliderB.IsAwake)
                        continue;

                    if (!colliderA.Rigidbody.IsStatic)
                    {
                        //Calculate state
                        Transform transformA = colliderA.Transform;
                        double bottom = transformA.Position.Y + transformA.Dimensions.Y / 2 + 1;

                        RayResult leftRay = Raycast(new PointRay(new ScrapVector(transformA.Position.X - transformA.Dimensions.X / 2, bottom)));
                        RayResult rightRay = Raycast(new PointRay(new ScrapVector(transformA.Position.X + transformA.Dimensions.X / 2, bottom)));
                        RayResult midRay = Raycast(new PointRay(new ScrapVector(transformA.Position.X, bottom)));

                        if (leftRay.hit || midRay.hit || rightRay.hit)
                        {
                            colliderA.Rigidbody.State = (colliderB.Rigidbody.IsStatic) ? RigidBody2D.RigidState.REST_STATIC : RigidBody2D.RigidState.REST_DYNAMIC;
                        }
                        else
                        {
                            colliderA.Rigidbody.State = RigidBody2D.RigidState.FALLING;
                        }
                    }

                    if (!colliderB.Rigidbody.IsStatic)
                    {
                        Transform transformB = colliderB.Transform;
                        double bottom = transformB.Position.Y + transformB.Dimensions.Y / 2 + 1;

                        RayResult leftRay = Raycast(new PointRay(new ScrapVector(transformB.Position.X - transformB.Dimensions.X / 2 + 2, bottom)));
                        RayResult rightRay = Raycast(new PointRay(new ScrapVector(transformB.Position.X + transformB.Dimensions.X / 2 - 2, bottom)));
                        RayResult midRay = Raycast(new PointRay(new ScrapVector(transformB.Position.X, bottom)));

                        if (leftRay.hit || midRay.hit || rightRay.hit)
                        {
                            colliderB.Rigidbody.State = (colliderA.Rigidbody.IsStatic) ? RigidBody2D.RigidState.REST_STATIC : RigidBody2D.RigidState.REST_DYNAMIC;
                        }
                        else
                        {
                            colliderB.Rigidbody.State = RigidBody2D.RigidState.FALLING;
                        }
                    }

                    if (Collision.ApplyAppropriateAlgorith(colliderA, colliderB, out CollisionManifold manifold))
                    {
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

                        reports.Add(new IncidentReport(colliderA.Rigidbody, colliderB.Rigidbody, manifold));
                    }
                }
            }

            foreach (IncidentReport report in reports)
            {
                for (int j = 0; j < PhysicsSystem.MAX_IMPULSE_ITERATIONS; j++)
                {
                    Collision.ResolveImpulse(report.BodyA, report.BodyB, report.Manifold);
                }

            }
        }

        public override void Draw(Camera mainCamera)
        {

        }
    }
}
