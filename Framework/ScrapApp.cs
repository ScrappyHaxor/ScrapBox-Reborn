using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework
{
	public class ScrapApp : Game
	{
		public GraphicsDeviceManager Graphics { get { return graphics; } set { graphics = value; } }
		public SpriteBatch Batch { get { return spriteBatch; } set { spriteBatch = value; Renderer2D.Initialize(spriteBatch); } }

		private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

		public ScrapApp()
		{
			graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			LoadAssets();
		}

		protected virtual void LoadAssets()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Renderer2D.Initialize(spriteBatch);
			PostInitialize();
		}

		protected virtual void PostInitialize()
		{

		}

		protected override void Update(GameTime gameTime)
		{
			Update(ScrapMath.Deltafy(gameTime));
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			Draw(ScrapMath.Deltafy(gameTime));
			base.Draw(gameTime);
		}

		protected virtual void Update(double dt)
		{
			InputManager.Update();
			Physics2D.Update(dt);
		}

		protected virtual void Draw(double dt)
		{

		}
	}
}
