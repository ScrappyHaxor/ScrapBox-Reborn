using System;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.ECS.Components;

namespace ScrapBox.Framework.ECS
{
	public class TriggerArgs : EventArgs
	{
		public Collider Other { get; set; }

		public TriggerArgs(Collider other)
		{
			Other = other;
		}
	}

	public abstract class Collider : Component
	{
		public enum CollisionAlgorithm
		{
			SAT,
			SAT_CIRCLE,
			SAT_PIXEL
		}

		public enum TriggerType
		{
			NONE,
			TRIGGER_ONLY,
			HYBRID
		}

		public override string Name => "Collider";

		public Transform Transform;
		public RigidBody2D Rigidbody;
		public Sprite2D Sprite;

		public ScrapVector Dimensions { get; set; }
		public CollisionAlgorithm Algorithm { get; set; }
		public TriggerType Trigger { get; set; }
		public EventHandler<TriggerArgs> Triggered { get; set; }

		public double Top { get { return Transform.Position.Y - Dimensions.Y / 2; } }
		public double Bottom { get { return Transform.Position.Y + Dimensions.Y / 2; } }
		public double Left { get { return Transform.Position.X - Dimensions.X / 2; } }
		public double Right { get { return Transform.Position.X + Dimensions.X / 2; } }

        public abstract ScrapVector[] GetVerticies();

        public override void Awake()
        {
			if (IsAwake)
				return;

			bool success = Dependency(out Transform);
			if (!success)
				return;

			success = Dependency(out Rigidbody);
			if (!success)
				return;

			WorldManager.GetSystem<CollisionSystem>().RegisterCollider(this);
			IsAwake = true;
        }

		public override void Sleep()
		{
			if (!IsAwake)
				return;

			WorldManager.GetSystem<CollisionSystem>().PurgeCollider(this);
			IsAwake = false;
		}
    }
}
