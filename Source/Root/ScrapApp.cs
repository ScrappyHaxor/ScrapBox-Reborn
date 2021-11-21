using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;

using System;

namespace ScrapBox.Root
{
	public class ScrapApp : Game
	{
		public GraphicsDeviceManager Graphics { get { return graphics; }}
		public SpriteBatch Batch { get { return spriteBatch; } }

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
			InputManager.Update();
			Physics2D.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}
}
