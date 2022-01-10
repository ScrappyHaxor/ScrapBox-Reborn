using System;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Pathfinding;

namespace ScrapBox.Framework.Level
{
	public abstract class Scene
	{
        public ScrapApp Parent { get; set; }
		public GraphicsDeviceManager Graphics { get; set; }
		public Camera MainCamera { get; set; }
        public NodeMap NodeMap { get; set; }

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
        }

		public virtual void Update(double dt)
        {
            MainCamera.Update(Graphics.GraphicsDevice.Viewport);
            PathingManager.Update();
        }

		public virtual void Draw()
        {
            
        }
	}
}
