using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.ECS.Systems;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Diagnostics;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
	public class Sprite2D : Component
	{
        public override string Name => "Sprite2D";

		public Transform Transform;
		public ScrapVector Position { get { if (Transform == null) return default; return Transform.Position; } }
		public double Rotation { get { if (Transform == null) return default; return Transform.Rotation; } }
		public Texture2D Texture { get; set; }
		public ScrapVector Scale { get { return new ScrapVector(Texture.Width / Transform.Dimensions.X, Texture.Height / Transform.Dimensions.Y); } }	
		public Rectangle SourceRectangle { get; set; }
		public Color TintColor { get; set; }
		public SpriteEffects Effects { get; set; }
		public float Depth { get; set; }
		public Effect Shader { get; set; }

		public Sprite2D()
		{
			TintColor = Color.White;
		}

		public override void Awake()
		{
			bool success = Dependency(out Transform);
			if (!success)
				return;

			if (Texture == null)
            {
				LogService.Log(Name, "Awake", "Texture is null.", Severity.ERROR);
				return;
            }

			if (SourceRectangle == default)
				SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);

			SpriteSystem spriteSystem = (SpriteSystem)WorldManager.GetSystem<SpriteSystem>();
			spriteSystem.RegisterSprite(this);
			IsAwake = true;
		}

		public override void Sleep()
        {
			SpriteSystem spriteSystem = (SpriteSystem)WorldManager.GetSystem<SpriteSystem>();
			spriteSystem.PurgeSprite(this);
			IsAwake = false;
        }
	}
}
