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
		public Action Init;
		public Action<double> Renew;
		public Action<double> Render;

		public GraphicsDeviceManager Graphics;

		public InternalGame()
        {
			Graphics = new GraphicsDeviceManager(this)
			{
				PreferMultiSampling = true,
				SynchronizeWithVerticalRetrace = true
            };

            Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

        protected override void Initialize()
        {
			Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
			Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
			Graphics.ApplyChanges();

			
			Init();
			base.Initialize();
		}

        protected override void LoadContent()
        {
			base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
			Renew(ScrapMath.Deltafy(gameTime));
			base.Update(gameTime);
		}

        protected override void Draw(GameTime gameTime)
        {
            
			Render(ScrapMath.Deltafy(gameTime));
			base.Draw(gameTime);
		}
    }

	public abstract class ScrapApp
	{
		public const string Version = "ScrapBox v.1.8.0";

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
		internal void Init()
        {
			Initialize();
        }

		internal void Update(double dt)
		{
			InputManager.Update();
			WorldManager.Update(dt);
		}

		internal void Draw(double dt)
		{
			WorldManager.Draw(dt);
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
