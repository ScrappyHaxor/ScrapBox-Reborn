using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS
{
	public interface IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public void Awake();
		public void Sleep();
		public void Update(double dt);
		public void Draw(SpriteBatch spriteBatch, Camera camera);
		
	}
}
