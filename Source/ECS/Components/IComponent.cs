using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Scene;

namespace ScrapBox.ECS.Components
{
	public interface IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public void Awake();
		public void Update(double dt);
		public void Draw(SpriteBatch spriteBatch, Camera camera);
		
	}
}
