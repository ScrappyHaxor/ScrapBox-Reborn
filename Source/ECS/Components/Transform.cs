using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScrapBox.ECS.Components
{
	public class Transform : IComponent
	{
		public virtual Entity Owner { get; set; }
		public virtual bool IsAwake { get; set; }

		public Vector2 Position { get; set; }
		public Vector2 Dimensions { get; set; }
		public float Depth { get; set; }
		public float Rotation { get; set; }
		
		public virtual void Awake()
		{
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
	}

}
