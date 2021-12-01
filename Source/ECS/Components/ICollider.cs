using System;

using ScrapBox.SMath;
using ScrapBox.Args;

namespace ScrapBox.ECS.Components
{
	public interface ICollider : IComponent
	{
		public enum ColliderType
		{
			SAT,
			CIRCLE,
			PIXEL_PERFECT_SAT,
			PIXEL_PERFECT_CIRCLE
		}

		public enum TriggerType
		{
			NONE,
			TRIGGER_ONLY,
			HYBRID
		}

		public Transform Transform { get; set; }
		public Sprite2D Sprite { get; set; }
		public RigidBody2D RigidBody { get; set; }
		public ScrapVector Dimensions { get; set; }
		public ColliderType TypeOfCollider { get; set; }
		public TriggerType Trigger { get; set; }
		public EventHandler<TriggerArgs> Triggered { get; set; }

		public double Top { get; }
		public double Bottom { get; }
		public double Left { get; }
		public double Right { get; }

		public abstract ScrapVector[] GetVerticies();
	}
}
