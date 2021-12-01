using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScrapBox.Scene
{
	public interface IScene
	{
		public GraphicsDeviceManager Graphics { get; set; }
		public Camera MainCamera { get; set; }

		public void Initialize(GraphicsDeviceManager graphics);
		public void PostInitialize();
		public void Update(double dt);
		public void Draw(SpriteBatch spriteBatch);
	}
}
