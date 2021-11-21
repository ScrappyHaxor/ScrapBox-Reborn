using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;

using System;

namespace ScrapBox.ECS.Components
{
	public class BoxCollider2D : ICollider
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }
		
		public Transform Transform { get; set; }
		public Vector2 Dimensions { get; set; }
		public ICollider.ColliderType TypeOfCollider { get; set; }
		
		public float Top { get { return Transform.Position.Y - Dimensions.Y / 2; } }
		public float Bottom { get { return Transform.Position.Y + Dimensions.Y / 2; } }
		public float Left { get { return Transform.Position.X - Dimensions.X / 2; } }
		public float Right { get { return Transform.Position.X + Dimensions.X / 2; } }

		public virtual void Awake()
		{
			TypeOfCollider = ICollider.ColliderType.AABB;
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("BoxCollider2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("BoxCollider2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}
			
			Physics2D.RegisterCollider(this);
			IsAwake = true;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsAwake)
				return;
		}

		public virtual bool Intersects(ICollider other)
		{
			if (!IsAwake)
				return false;

			if (other.TypeOfCollider == ICollider.ColliderType.AABB)
			{
				return Right > other.Transform.Position.X - other.Dimensions.X / 2 && Left < other.Transform.Position.X + other.Dimensions.X / 2
					&& Bottom > other.Transform.Position.Y - other.Dimensions.Y / 2 && Top < other.Transform.Position.Y + other.Dimensions.Y / 2;
			}
			else if (other.TypeOfCollider == ICollider.ColliderType.CIRCLE)
			{
				throw new NotImplementedException();
			}
			else if (other.TypeOfCollider == ICollider.ColliderType.PIXEL_PERFECT_AABB)
			{
				throw new NotImplementedException();
			}

			return false;
		}
	}
}
