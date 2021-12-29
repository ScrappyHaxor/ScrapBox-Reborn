using ScrapBox.Framework.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.ECS.Components
{
    public class CircleCollider : Collider
    {
        public override string Name => "CircleCollider";

        public double Radius { get { return Dimensions.X; } set { Dimensions = new ScrapVector(value); } }

        public override ScrapVector[] GetVerticies()
        {
            return default;
        }

        public override void Awake()
        {
            Algorithm = CollisionAlgorithm.SAT_CIRCLE;

            base.Awake();
        }
    }
}
