using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;
using ScrapBox.Scene;
using ScrapBox.SMath;

namespace ScrapBox.ECS.Components
{
	public class Sprite2D : IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }
		public Texture2D Texture { get; set; }
		public ScrapVector Scale { get { return new ScrapVector(Texture.Width / Transform.Dimensions.X, Texture.Height / Transform.Dimensions.Y); } }	
		public Rectangle SourceRectangle { get; set; }
		public Color TintColor { get; set; }
		public SpriteEffects Effects { get; set; }
		public float Depth { get; set; }

		public Sprite2D()
		{
			TintColor = Color.White;
		}

		public virtual void Awake()
		{
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("Sprite2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("Sprite2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}
			if (SourceRectangle == default)
				SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);

			IsAwake = true;
		}

		public virtual void Update(double dt)
		{		
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake)
				return;			
				
			Renderer2D.RenderSprite(this, camera);	
		}
	}
}
