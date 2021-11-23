using ScrapBox.SMath;

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

		public Transform Transform { get; set; }
		public Sprite2D Sprite { get; set; }
		public RigidBody2D RigidBody { get; set; }
		public ScrapVector Dimensions { get; set; }
		public ColliderType TypeOfCollider { get; set; }

		public double Top { get; }
		public double Bottom { get; }
		public double Left { get; }
		public double Right { get; }

		public ScrapVector[] GetVerticies();
	}
}
