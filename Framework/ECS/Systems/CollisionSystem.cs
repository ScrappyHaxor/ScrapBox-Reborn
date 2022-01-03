﻿using System.Collections.Generic;

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

    public abstract class Ray
    {
        public abstract bool Shoot(Collider other);
    }

    public class PointRay : Ray
    {
        public readonly ScrapVector point;

        public PointRay(ScrapVector p)
        {
            point = p;
        }

        public override bool Shoot(Collider other)
        {
            if (other.Algorithm == Collider.CollisionAlgorithm.SAT ||
                other.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
            {
                return Collision.IntersectPointPolygon(point, other.GetVerticies());
            }
            else if (other.Algorithm == Collider.CollisionAlgorithm.SAT_CIRCLE)
            {
                return Collision.IntersectPointCircle(point, other);
            }

            return false;
        }
    }

    public class ShapeRay : Ray
    {
        public readonly ScrapVector[] verts;


        public ShapeRay(ScrapVector[] verts)
        {
            this.verts = verts;
        }

        public override bool Shoot(Collider other)
        {
            if (other.Algorithm == Collider.CollisionAlgorithm.SAT ||
                other.Algorithm == Collider.CollisionAlgorithm.SAT_PIXEL)
            {
                return Collision.IntersectPolygons(verts, other.GetVerticies(), out CollisionManifold _);
            }

            return false;
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
        public bool Raycast(Ray ray)
        {
            bool result;
            foreach (Collider c in colliders)
            {
                result = ray.Shoot(c);
                if (result)
                    return true;
            }

            return false;
        }

        public override void Update(double dt)
        {
            reports.Clear();

            for (int i = 0; i < colliders.Count; i++)
            {
                Collider colliderA = colliders[i];

                if (!colliderA.IsAwake)
                    continue;

                RigidBody2D.RigidState state = colliders[i].Rigidbody.State;
                for (int j = i; j < colliders.Count; j++)
                {
                    if (i == j) continue;

                    Collider colliderB = colliders[j];

                    if (!colliderB.IsAwake)
                        continue;

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

                colliderA.Rigidbody.State = state;
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