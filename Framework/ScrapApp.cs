using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Input;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Diagnostics;

namespace ScrapBox.Framework
{
	internal class InternalGame : Game
    {
		public EventHandler Init;
		public EventHandler Renew;
		public EventHandler Render;
		public double dt;

		public GraphicsDeviceManager Graphics;

		public InternalGame()
        {
			Graphics = new GraphicsDeviceManager(this);
			Graphics.PreferMultiSampling = true;
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

        protected override void Initialize()
        {
			Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
			Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
			Graphics.ApplyChanges();

			base.Initialize();
			Init?.Invoke(this, null);
        }

        protected override void LoadContent()
        {
			base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
			Renew?.Invoke(this, null);
			dt = ScrapMath.Deltafy(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
			Render?.Invoke(this, null);
        }
    }

	public abstract class ScrapApp
	{
		public GraphicsDeviceManager Graphics { get { return internalGame.Graphics; } }
		public SpriteBatch Batch { get; set; }
		public ContentManager Content { get { return internalGame.Content; } }
		public GameWindow Window { get { return internalGame.Window; } }

		private readonly InternalGame internalGame;

		protected ScrapApp()
		{
			internalGame = new InternalGame();
			
			internalGame.Init += Init;
			internalGame.Renew += Update;
			internalGame.Render += Draw;

			AppDomain.CurrentDomain.ProcessExit += new EventHandler(Exit);
		}

		public void Run()
        {
			internalGame.Run();
		}

		//Sneaky way to hide default monogame methods
		internal void Init(object o, EventArgs e)
        {
			Initialize();
        }

		internal void Update(object o, EventArgs e)
		{
			InputManager.Update();
			

			WorldManager.Update(internalGame.dt);
			PhysicsDiagnostics.FPS = (int)(1 / internalGame.dt);
		}

		internal void Draw(object o, EventArgs e)
		{
			Graphics.GraphicsDevice.Clear(WorldManager.BackColor);
			WorldManager.Draw();
		}

		protected virtual void Exit(object o, EventArgs e)
        {
			LogService.GenerateLog();
        }

		protected virtual void Initialize()
		{
			Batch = new SpriteBatch(Graphics.GraphicsDevice);
			Renderer.Initialize(Batch);
			PostInitialize();
		}

		protected virtual void PostInitialize()
		{

		}
	}
}
