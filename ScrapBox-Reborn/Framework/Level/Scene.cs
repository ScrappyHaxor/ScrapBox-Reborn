using System;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Pathfinding;
using System.Collections.Generic;

namespace ScrapBox.Framework.Level
{
	public abstract class Scene
	{
        public ScrapApp Parent { get; set; }
		public GraphicsDeviceManager Graphics { get; set; }
		public Camera MainCamera { get; set; }
        public NodeMap NodeMap { get; set; }
        public LayerStack Stack { get; set; }

        protected Scene(ScrapApp app)
        {
            Parent = app;
            Graphics = app.Graphics;

            NodeMap = new NodeMap();
        }

		public virtual void Initialize()
        {
            MainCamera = new Camera(Graphics.GraphicsDevice.Viewport)
            {
                Zoom = 1
            };

            Stack = new LayerStack();
            Stack.InsertAt(0, new Layer("Background"));
            Stack.InsertAt(1, new Layer("Foreground"));
            Stack.InsertAt(2, new Layer("UI"));
        }

        public virtual void LoadAssets()
        {
            //TODO: Fixme.
            //AssetManager.LoadInternalAssets(Parent.Content);
        }

        public virtual void UnloadAssets()
        {
            AssetManager.Unload(Parent.Content);
        }

		public virtual void Load(params object[] args)
        {
            PathingManager.map = NodeMap;
        }

        public virtual void Unload()
        {
            Renderer.PostProcessing = null;
            NodeMap.Purge();
            Stack.Reset();
            Stack.Purge();
        }

		public virtual void PreStackTick(double dt)
        {
            MainCamera.Update(Graphics.GraphicsDevice.Viewport);
            PathingManager.Update();
            Stack.Update(dt);
        }

        public virtual void PostStackTick(double dt)
        {

        }

		public virtual void PreStackRender()
        {
            Stack.Render(MainCamera);
        }

        public virtual void PostStackRender()
        {

        }
	}
}
