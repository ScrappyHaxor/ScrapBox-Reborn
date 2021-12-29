using System.Diagnostics;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Diagnostics;
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
            


        }

        public virtual void LoadAssets()
        {

        }

        public virtual void UnloadAssets()
        {
            AssetManager.Unload(Parent.Content);
        }

		public virtual void Load()
        {
            

            MainCamera = new Camera(Graphics.GraphicsDevice.Viewport)
            {
                Zoom = 1
            };
        }

        public virtual void Unload()
        {
            MainCamera = null;
        }

		public virtual void Update(double dt)
        {
            MainCamera.Update(Graphics.GraphicsDevice.Viewport);
        }

		public virtual void Draw()
        {
            RenderDiagnostics.Calls = 0;
        }
	}
}
