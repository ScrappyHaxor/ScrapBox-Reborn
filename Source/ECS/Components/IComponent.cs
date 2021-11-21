using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScrapBox.ECS.Components
{
	public interface IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public void Awake();
		public void Update(GameTime gameTime);
		public void Draw(SpriteBatch spriteBatch);
		
	}
}
