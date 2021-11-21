using Microsoft.Xna.Framework;

namespace ScrapBox.ECS.Components
{
	public interface ICollider : IComponent
	{
		public enum ColliderType
		{
			AABB,
			CIRCLE,
			PIXEL_PERFECT_AABB
		}

		public Transform Transform { get; set; }
		public Vector2 Dimensions { get; set; }
		public ColliderType TypeOfCollider { get; set; }

		public bool Intersects(ICollider other);
	}
}
