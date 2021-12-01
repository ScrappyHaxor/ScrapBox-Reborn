using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.SMath;
using ScrapBox.Scene;

namespace ScrapBox.ECS.Components
{
	public class Transform : IComponent
	{
		public virtual Entity Owner { get; set; }
		public virtual bool IsAwake { get; set; }

		public ScrapVector Position { get; set; }
		public ScrapVector Dimensions { get; set; }
		public float Depth { get; set; }
		public float Rotation { get; set; }
		
		public virtual void Awake()
		{
			IsAwake = true;
		}

		public virtual void Update(double dt)
		{
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake)
				return;
		}
	}

}
